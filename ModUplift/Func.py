# Func.py - содержит основные функции: чтение данных, очистка данных, преобразование данных, а также метрические значения для обучения

import pandas as pd
import numpy as np

n = 5           # Количество фолдов
s = 6           # Фиксация значений
est=300         # Количество деревьев
dep=5           # Глубина деревьев
r=0.03          # Скорость обучения
iter=1000       # Количество итераций оптимизации

# Метрики
target = 'retention_7'      # Целевая метрика
exper = 'sum_gamerounds'    # Преэкспериментальная метрика

# признаки для uplift-моделей
features = ['sum_gamerounds', 'retention_1']

# Загрузка данных
def Data():
    data = pd.read_csv('uplift_game_dataset.txt')
    return data

# Корректировка данных
def Prepare(data):
    data = data.copy()      # Копия данных

    # Удаление дубликатов
    data = data.drop_duplicates(subset='userid')

    #Заполнение пропусков средним
    data['sum_gamerounds'] = data['sum_gamerounds'].fillna(data['sum_gamerounds'].median())

    #Удаление отрицательных значений
    data = data[data['sum_gamerounds'] >= 0]
    return data

# Трансформация данных
def TransformData(data):

    data = data.copy()

    # Очистка version
    data['version'] = data['version'].str.strip()

    # Преобразование булевых значений
    data['retention_1'] = data['retention_1'].astype(int)
    data['retention_7'] = data['retention_7'].astype(int)

    # gate_40 — эксперимент, gate_30 — контроль
    data['treatment'] = (data['version'] == 'gate_40').astype(int)

    #Типы данных
    data['userid'] = data['userid'].astype(int)
    data['sum_gamerounds'] = data['sum_gamerounds'].astype(float)
    return data

# Проверка наличия информации
def CheckFeatures(data):

    missing = [f for f in features if f not in data.columns]    # Список отсутствующих признаков
    if len(missing) > 0:
        raise ValueError(f"Missing features: {missing}")

# Подписи графиков
labelstext = {# Общие подписи
    "cv_start": "Запуск кросс-валидации...", "error_auc": "Площадь под кривой ошибки",

    # AUUC
    "auuc_title": "График AUUC", "auuc_xlabel": "Доля пользователей", "auuc_ylabel": "Накопленный uplift",

    # Qini
    "qini_title": "График Qini", "qini_xlabel": "Доля пользователей", "qini_ylabel": "Qini",

    # Ошибки
    "error_title": "График ошибок", "error_xlabel": "Доля пользователей", "error_ylabel": "Среднеквадратичная ошибка",

    # Динамика ошибки
    "learn_step":"Шаг обучения", "squar_err":"Квадратичная ошибка", "dynam_error_train":"Динамика ошибки при обучении моделей",

    # Распределение uplift
    "model":"модель", "indiv_uplift":"Индивидуальный uplift", "distrib_individ_uplift_mod":"Распределение индивидуального uplift","count":"Пользователи",

    # Калибровка
    "calibration_title": "Калибровка uplift модели", "calibration_xlabel": "Предсказанный uplift", "calibration_ylabel": "Фактический uplift", "calibration_ideal": "Идеал"}