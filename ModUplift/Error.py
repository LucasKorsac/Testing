#Error.py - проверка данных на полность, пропуски, бинарность
import pandas as pd

# ѕроверка наличи€ об€зательных колонок
def Columns(data: pd.DataFrame, required: list):
    missing = set(required) - set(data.columns)

    if missing:
        raise ValueError(f'ќтсутствуют об€зательные колонки: {missing}')

# ѕроверка наличи€ пропусков в данных
def Missing(data: pd.DataFrame):

    if data.isnull().any().any():
        raise ValueError('¬ данных присутствуют пропущенные значени€')

# ѕроверка колонки на бинарность (0/1)
def Binary(data: pd.DataFrame, col: str):

    unique = set(data[col].unique())
    if not unique.issubset({0, 1}):
        raise ValueError(f' олонка {col} не €вл€етс€ бинарной')



