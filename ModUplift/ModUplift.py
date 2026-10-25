"""Flask API для универсального uplift-анализа.

Запуск из папки проекта:
    flask --app ModUplift run --host 0.0.0.0 --port 8000
"""
from __future__ import annotations

import logging
from typing import Any

import numpy as np
import pandas as pd
from flask import Flask, jsonify, request

from config import UpliftConfig
from service import UpliftService

app = Flask(__name__)
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)
ALLOWED_MODELS = {"T", "X", "R", "DR"}


def _json_safe(value: Any) -> Any:
    if isinstance(value, dict):
        return {str(k): _json_safe(v) for k, v in value.items()}
    if isinstance(value, (list, tuple)):
        return [_json_safe(v) for v in value]
    if isinstance(value, (np.integer,)):
        return int(value)
    if isinstance(value, (np.floating,)):
        number = float(value)
        return number if np.isfinite(number) else None
    if isinstance(value, (np.bool_,)):
        return bool(value)
    if value is None or isinstance(value, (str, int, float, bool)):
        return value
    if pd.isna(value):
        return None
    if hasattr(value, "isoformat"):
        return value.isoformat()
    return str(value)


def _error(message: str, status: int = 400):
    return jsonify({"success": False, "error": message}), status


@app.get("/api/health")
def health():
    return jsonify({"success": True, "service": "uplift"})


@app.post("/api/uplift/analyze")
def analyze_uplift():
    payload = request.get_json(silent=True)
    if not isinstance(payload, dict):
        return _error("Тело запроса должно быть JSON-объектом")

    rows = payload.get("data")
    if not isinstance(rows, list) or not rows or not all(isinstance(row, dict) for row in rows):
        return _error("'data' должен быть непустым массивом JSON-объектов")

    target = payload.get("targetColumn")
    if not isinstance(target, str) or not target.strip():
        return _error("Не указана обязательная колонка targetColumn")
    treatment_col = payload.get("treatmentColumn", "treatment")
    user_col = payload.get("userIdColumn", "userid")
    features = payload.get("features", [])
    cuped_col = payload.get("cupedColumn")
    model_name = str(payload.get("model", "DR")).upper()
    use_cuped = payload.get("cuped", False)
    cross_validate = payload.get("crossValidate", True)
    test_id = payload.get("testId")

    if not isinstance(treatment_col, str) or not treatment_col.strip():
        return _error("treatmentColumn должен быть непустой строкой")
    if user_col is not None and (not isinstance(user_col, str) or not user_col.strip()):
        return _error("userIdColumn должен быть непустой строкой или null")
    if not isinstance(features, list) or not all(isinstance(c, str) and c.strip() for c in features):
        return _error("features должен быть массивом названий колонок")
    if len(features) != len(set(features)):
        return _error("В features есть повторяющиеся названия колонок")
    if model_name not in ALLOWED_MODELS:
        return _error(f"Неизвестная модель '{model_name}'. Доступны: {sorted(ALLOWED_MODELS)}")
    if not isinstance(use_cuped, bool) or not isinstance(cross_validate, bool):
        return _error("cuped и crossValidate должны быть boolean")
    if use_cuped and (not isinstance(cuped_col, str) or not cuped_col.strip()):
        return _error("При cuped=true необходимо передать cupedColumn")

    try:
        df = pd.DataFrame(rows)
        if df.columns.duplicated().any():
            return _error("В датасете есть дублирующиеся названия колонок")
        config = UpliftConfig(
            target=target,
            treatment=treatment_col,
            user_id=user_col,
            version=payload.get("versionColumn"),
            features=features,
            pre_experiment=cuped_col if use_cuped else None,
            control_value=payload.get("controlValue", 0),
            treatment_value=payload.get("treatmentValue", 1),
            n_splits=int(payload.get("nSplits", 5)),
            random_state=int(payload.get("randomState", 6)),
        )
        config.validate()
        result_df, metrics = UpliftService(config).analyze(
            data=df,
            model_name=model_name,
            use_cuped=use_cuped,
            cross_validate=cross_validate,
            test_id=test_id,
        )
        return jsonify(_json_safe({
            "success": True,
            "error": None,
            "metrics": metrics,
            "rows": result_df.to_dict(orient="records"),
        }))
    except (ValueError, TypeError, KeyError) as exc:
        return _error(str(exc))
    except Exception:
        logger.exception("Ошибка при выполнении uplift-анализа")
        return _error("Внутренняя ошибка при выполнении uplift-анализа", 500)
