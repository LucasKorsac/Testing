import numpy as np
import pandas as pd

from config import UpliftConfig


class CupedTransformer:
    """CUPED-преобразование. theta и среднее X обучаются только в fit()."""
    def __init__(self, config: UpliftConfig):
        if not config.pre_experiment:
            raise ValueError("Для CUPED нужно указать pre_experiment")
        self.config = config
        self.theta = None
        self.x_mean = None

    def fit(self, data: pd.DataFrame):
        y = data[self.config.target].to_numpy(dtype=float)
        x = data[self.config.pre_experiment].to_numpy(dtype=float)
        self.x_mean = float(np.mean(x))
        variance = float(np.var(x))
        if variance <= 1e-12:
            self.theta = 0.0
        else:
            covariance = float(np.mean((y - np.mean(y)) * (x - self.x_mean)))
            self.theta = covariance / variance
        return self

    def transform(self, data: pd.DataFrame) -> pd.DataFrame:
        if self.theta is None or self.x_mean is None:
            raise RuntimeError("CUPED ещё не обучен: сначала вызовите fit()")
        result = data.copy()
        x = result[self.config.pre_experiment].to_numpy(dtype=float)
        result[f"{self.config.target}_cuped"] = (
            result[self.config.target].to_numpy(dtype=float)
            - self.theta * (x - self.x_mean)
        )
        return result

    def fit_transform(self, data: pd.DataFrame) -> pd.DataFrame:
        return self.fit(data).transform(data)
