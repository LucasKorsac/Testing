from dataclasses import dataclass
from typing import Optional

import pandas as pd

from config import UpliftConfig


@dataclass
class ExperimentData:
    data: pd.DataFrame
    test_id: Optional[str] = None


class DataPreprocessor:

    def __init__(self, config: UpliftConfig):
        self.config = config

    def prepare(
        self,
        data: pd.DataFrame,
        test_id: Optional[str] = None
    ) -> ExperimentData:

        data = data.copy()

        # Удаляем дубликаты пользователей

        if self.config.user_id in data.columns:

            data = data.drop_duplicates(
                subset=self.config.user_id
            )

        # Если treatment не передан,
        # пробуем получить его из version

        if self.config.treatment not in data.columns:

            if self.config.version not in data.columns:
                raise ValueError(
                    "Не найдены treatment или version"
                )

            versions = (
                data[self.config.version]
                .astype(str)
                .str.strip()
            )

            unique_versions = (
                versions
                .dropna()
                .unique()
                .tolist()
            )

            if len(unique_versions) != 2:

                raise ValueError(
                    "Для A/B теста необходимо ровно "
                    "2 варианта"
                )

            mapping = {
                unique_versions[0]: 0,
                unique_versions[1]: 1
            }

            data[self.config.treatment] = (
                versions
                .map(mapping)
                .astype(int)
            )

        # Проверяем обязательные колонки

        required = [
            self.config.user_id,
            self.config.treatment,
            self.config.target
        ]

        required += self.config.features

        missing = [
            column
            for column in required
            if column not in data.columns
        ]

        if missing:

            raise ValueError(
                f"Отсутствуют колонки: {missing}"
            )

        # Приводим числовые данные

        numeric_columns = set(
            self.config.features
            + [
                self.config.target,
                self.config.treatment
            ]
        )

        for column in numeric_columns:

            data[column] = pd.to_numeric(
                data[column],
                errors="coerce"
            )

        # Убираем отрицательные game rounds

        if self.config.pre_experiment in data.columns:

            data = data[
                data[self.config.pre_experiment] >= 0
            ]

        # Удаление пропусков

        data = data.dropna(
            subset=required
        )

        data = data.reset_index(
            drop=True
        )

        # Проверка treatment

        unique_treatment = set(
            data[self.config.treatment].unique()
        )

        if not unique_treatment.issubset({0, 1}):

            raise ValueError(
                "treatment должен содержать "
                "только 0 и 1"
            )

        if data[self.config.treatment].nunique() != 2:

            raise ValueError(
                "В данных должны присутствовать "
                "и контрольная, и экспериментальная группы"
            )

        return ExperimentData(
            data=data,
            test_id=test_id
        )