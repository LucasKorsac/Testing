import numpy as np
import pandas as pd

from config import UpliftConfig


def validate_and_prepare(data: pd.DataFrame, config: UpliftConfig) -> pd.DataFrame:
    """Проверяет данные и нормализует treatment к 0/1 для моделей."""
    config.validate()
    if data.empty:
        raise ValueError("Датасет пуст")
    if data.columns.duplicated().any():
        raise ValueError("В датасете есть повторяющиеся названия колонок")

    required = {config.target, config.treatment, *config.features}
    if config.user_id:
        required.add(config.user_id)
    if config.version:
        required.add(config.version)
    if config.pre_experiment:
        required.add(config.pre_experiment)
    missing = sorted(required - set(data.columns))
    if missing:
        raise ValueError(f"В датасете отсутствуют колонки: {missing}")

    result = data.copy()
    if config.user_id:
        if result[config.user_id].isna().any():
            raise ValueError("В колонке ID пользователя есть пропуски")
        if result[config.user_id].duplicated().any():
            raise ValueError("Ожидается одна строка на пользователя в рамках эксперимента")

    observed = set(result[config.treatment].dropna().unique().tolist())
    expected = {config.control_value, config.treatment_value}
    if result[config.treatment].isna().any():
        raise ValueError("В treatment есть пропуски")
    if observed != expected:
        raise ValueError(
            f"treatment должен содержать обе группы {sorted(expected)}; получено {sorted(observed, key=str)}"
        )
    result[config.treatment] = result[config.treatment].map(
        {config.control_value: 0, config.treatment_value: 1}
    ).astype(int)

    numeric = list(dict.fromkeys([config.target, *config.features] + ([config.pre_experiment] if config.pre_experiment else [])))
    for column in numeric:
        result[column] = pd.to_numeric(result[column], errors="raise")
        if result[column].isna().any():
            raise ValueError(f"В колонке '{column}' есть пропуски")
        if not np.isfinite(result[column].to_numpy(dtype=float)).all():
            raise ValueError(f"Колонка '{column}' содержит inf или -inf")
    if not config.features:
        raise ValueError("features не может быть пустым: моделям нужны признаки")
    if result[config.treatment].value_counts().min() < 2:
        raise ValueError("В каждой группе должно быть не менее двух наблюдений")
    return result
