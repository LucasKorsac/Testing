from abc import ABC, abstractmethod

import numpy as np
import pandas as pd
from sklearn.ensemble import GradientBoostingRegressor
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import StratifiedKFold


class UpliftModel(ABC):
    def __init__(self, config):
        self.config = config

    def create_regressor(self):
        return GradientBoostingRegressor(
            n_estimators=self.config.n_estimators,
            max_depth=self.config.max_depth,
            learning_rate=self.config.learning_rate,
            random_state=self.config.random_state,
        )

    def get_target(self, data):
        adjusted = f"{self.config.target}_cuped"
        return adjusted if adjusted in data.columns else self.config.target

    def _fit_outcome_models(self, X, y, treatment):
        control = treatment == 0
        treated = treatment == 1
        if control.sum() < 2 or treated.sum() < 2:
            raise ValueError("Для каждой группы нужно не менее двух наблюдений")
        self.mu0 = self.create_regressor().fit(X.loc[control], y.loc[control])
        self.mu1 = self.create_regressor().fit(X.loc[treated], y.loc[treated])
        return control, treated

    def _crossfit_nuisance(self, X, y, treatment):
        """OOF оценки mu0, mu1 и propensity для построения pseudo-outcome."""
        counts = treatment.value_counts()
        n_splits = min(5, int(counts.min()))
        if n_splits < 2:
            raise ValueError("Недостаточно наблюдений в одной из групп для cross-fitting")
        mu0_oof = np.full(len(X), np.nan)
        mu1_oof = np.full(len(X), np.nan)
        e_oof = np.full(len(X), np.nan)
        cv = StratifiedKFold(n_splits=n_splits, shuffle=True, random_state=self.config.random_state)
        for train_idx, valid_idx in cv.split(X, treatment):
            tr_t = treatment.iloc[train_idx]
            tr_X = X.iloc[train_idx]
            tr_y = y.iloc[train_idx]
            model0 = self.create_regressor().fit(tr_X.loc[tr_t == 0], tr_y.loc[tr_t == 0])
            model1 = self.create_regressor().fit(tr_X.loc[tr_t == 1], tr_y.loc[tr_t == 1])
            mu0_oof[valid_idx] = model0.predict(X.iloc[valid_idx])
            mu1_oof[valid_idx] = model1.predict(X.iloc[valid_idx])
            prop = LogisticRegression(max_iter=self.config.logistic_max_iter)
            prop.fit(tr_X, tr_t)
            e_oof[valid_idx] = prop.predict_proba(X.iloc[valid_idx])[:, 1]
        e_oof = np.clip(e_oof, self.config.min_propensity, self.config.max_propensity)
        return mu0_oof, mu1_oof, e_oof

    def _fit_propensity(self, X, treatment):
        self.propensity = LogisticRegression(max_iter=self.config.logistic_max_iter)
        self.propensity.fit(X, treatment)

    def _predict_propensity(self, X):
        return np.clip(self.propensity.predict_proba(X)[:, 1], self.config.min_propensity, self.config.max_propensity)

    @abstractmethod
    def fit(self, data: pd.DataFrame):
        raise NotImplementedError

    @abstractmethod
    def predict(self, data: pd.DataFrame) -> np.ndarray:
        raise NotImplementedError


class TLearner(UpliftModel):
    def fit(self, data):
        X = data[self.config.features]
        y = data[self.get_target(data)]
        t = data[self.config.treatment]
        self._fit_outcome_models(X, y, t)
        return self

    def predict(self, data):
        X = data[self.config.features]
        return self.mu1.predict(X) - self.mu0.predict(X)


class XLearner(UpliftModel):
    def fit(self, data):
        X = data[self.config.features]
        y = data[self.get_target(data)]
        t = data[self.config.treatment]
        control, treated = self._fit_outcome_models(X, y, t)
        d1 = y.loc[treated].to_numpy() - self.mu0.predict(X.loc[treated])
        d0 = self.mu1.predict(X.loc[control]) - y.loc[control].to_numpy()
        self.tau1 = self.create_regressor().fit(X.loc[treated], d1)
        self.tau0 = self.create_regressor().fit(X.loc[control], d0)
        self._fit_propensity(X, t)
        return self

    def predict(self, data):
        X = data[self.config.features]
        e = self._predict_propensity(X)
        return e * self.tau0.predict(X) + (1 - e) * self.tau1.predict(X)


class RLearner(UpliftModel):
    def fit(self, data):
        X = data[self.config.features]
        y = data[self.get_target(data)].astype(float)
        t = data[self.config.treatment].astype(int)
        # OOF nuisance estimates reduce in-sample leakage in the pseudo-outcome.
        mu0, mu1, e = self._crossfit_nuisance(X, y, t)
        m = e * mu1 + (1 - e) * mu0
        residual_t = t.to_numpy(dtype=float) - e
        weights = residual_t ** 2
        keep = weights > 1e-8
        if keep.sum() < 2:
            raise ValueError("Недостаточная вариативность treatment для R-learner")
        pseudo = (y.to_numpy(dtype=float) - m) / np.where(np.abs(residual_t) > 1e-6, residual_t, np.nan)
        keep &= np.isfinite(pseudo)
        if keep.sum() < 2:
            raise ValueError("Не удалось построить pseudo-outcome для R-learner")
        self.effect_model = self.create_regressor().fit(X.iloc[np.flatnonzero(keep)], pseudo[keep], sample_weight=weights[keep])
        self._fit_outcome_models(X, y, t)
        self._fit_propensity(X, t)
        return self

    def predict(self, data):
        return self.effect_model.predict(data[self.config.features])


class DRLearner(UpliftModel):
    def fit(self, data):
        X = data[self.config.features]
        y = data[self.get_target(data)].astype(float)
        t = data[self.config.treatment].astype(int)
        mu0, mu1, e = self._crossfit_nuisance(X, y, t)
        tv = t.to_numpy(dtype=float)
        yv = y.to_numpy(dtype=float)
        pseudo = (mu1 - mu0) + tv * (yv - mu1) / e - (1 - tv) * (yv - mu0) / (1 - e)
        self.effect_model = self.create_regressor().fit(X, pseudo)
        self._fit_outcome_models(X, y, t)
        self._fit_propensity(X, t)
        return self

    def predict(self, data):
        return self.effect_model.predict(data[self.config.features])


MODEL_REGISTRY = {"T": TLearner, "X": XLearner, "R": RLearner, "DR": DRLearner}


def create_model(name: str, config):
    key = name.upper()
    if key not in MODEL_REGISTRY:
        raise ValueError(f"Неизвестная модель '{name}'. Доступны: {sorted(MODEL_REGISTRY)}")
    return MODEL_REGISTRY[key](config)
