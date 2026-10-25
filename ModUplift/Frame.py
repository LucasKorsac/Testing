# FrameLearner.py - кросс-валидация uplift-моделей с выбором cuped
import numpy as np
import pandas as pd
from sklearn.model_selection import KFold
import Cuped
import Func
import T_Learner
import X_Learner
import R_Learner
import DR_Learner

# split: количество фолдов, rand: случайность, uc: применять ли CUPED к таргету
def CrossVal(model, split=Func.n, rand=Func.s, uc=True):
    # Загрузка и подготовка данных
    rawdt = Cuped.DataLoad()
    rawdt = Cuped.ErrorsCheck(rawdt)

    if uc:
        rawdt = Cuped.Cuped(rawdt)  # Если выбран cuped

    # Сброс индекса
    rawdt = rawdt.reset_index(drop=True)

    # Массив для OOF предсказаний
    uplift = np.zeros(len(rawdt))

    # Разбиение
    kf = KFold(n_splits=split, shuffle=True, random_state=rand)

    # K-Fold обучение
    for fold, (trainidx, validx) in enumerate(kf.split(rawdt)):
        print(f"Fold {fold + 1}/{split}")

        # Формирование выборок
        train = rawdt.iloc[trainidx].copy()
        val = rawdt.iloc[validx].copy()

        # Проверка данных
        train = model.ErrorsSet(train, uc)
        val = model.ErrorsSet(val, uc)

        # Обучение и предсказание uplift
        val = model.Learn(train, val, uc)

        # Сохранение OOF предсказания
        uplift[validx] = val['uplift'].values

    # Финальный результат с OOF uplift
    result = rawdt.copy()
    result['uplift_oof'] = uplift
    return result

# Запуск
if __name__ == "__main__":
    # Пример запуска для всех моделей с CUPED и без CUPED
    for uc in [True, False]:
        modetxt = "CUPED" if uc else "no CUPED"
        print(f"\n Кросс-валидация ({modetxt}) ")

        #Список моделей
        for model in [T_Learner, X_Learner, R_Learner, DR_Learner]:
            print(f"\n {model.__name__}")
            result = CrossVal(model, uc=uc) #Кросс-валидация
            #Список колонок
            cols = [Func.target, f"{Func.target}_cuped", "treatment"] + Func.features + ['uplift_oof']

            #Если cuped не используется, вывод оригинального таргета
            if not uc:
                cols.remove(f"{Func.target}_cuped")
                cols.append(Func.target)

            print(result[cols].head())