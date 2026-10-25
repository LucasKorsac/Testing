from dataclasses import dataclass, field
from typing import Optional


@dataclass
class UpliftConfig:
    """Общие настройки uplift-анализа."""
    target: str
    treatment: str = "treatment"
    user_id: Optional[str] = "userid"
    version: Optional[str] = None
    features: list[str] = field(default_factory=list)
    pre_experiment: Optional[str] = None
    control_value: int = 0
    treatment_value: int = 1
    n_estimators: int = 100
    max_depth: int = 3
    learning_rate: float = 0.05
    random_state: int = 6
    n_splits: int = 5
    logistic_max_iter: int = 1000
    min_propensity: float = 0.01
    max_propensity: float = 0.99

    def validate(self) -> None:
        for name in ("target", "treatment"):
            value = getattr(self, name)
            if not isinstance(value, str) or not value.strip():
                raise ValueError(f"Не указана колонка {name}")
        if self.target == self.treatment:
            raise ValueError("target и treatment должны быть разными колонками")
        if self.n_splits < 2:
            raise ValueError("n_splits должен быть не меньше 2")
        if self.n_estimators < 1 or self.max_depth < 1:
            raise ValueError("n_estimators и max_depth должны быть положительными")
        if not (0 < self.min_propensity < self.max_propensity < 1):
            raise ValueError("Неверные границы propensity score")
        if self.control_value == self.treatment_value:
            raise ValueError("Контрольная и экспериментальная группы должны различаться")
        if len(self.features) != len(set(self.features)):
            raise ValueError("В features есть дубликаты")
        forbidden = {self.target, self.treatment}
        if forbidden.intersection(self.features):
            raise ValueError("target и treatment нельзя включать в features")
        if self.user_id and self.user_id in self.features:
            raise ValueError("ID пользователя нельзя использовать как признак модели")
        if self.pre_experiment in {self.target, self.treatment}:
            raise ValueError("pre_experiment должен быть отдельным признаком")
