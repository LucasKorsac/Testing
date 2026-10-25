from flask import Flask, request, jsonify
import pandas as pd

app = Flask(__name__)


@app.post("/api/uplift/analyze")
def analyze_uplift():
    payload = request.get_json(silent=True)

    if not isinstance(payload, dict):
        return jsonify({
            "success": False,
            "error": "Expected a JSON object"
        }), 400

    rows = payload.get("data")
    if not isinstance(rows, list) or not rows:
        return jsonify({
            "success": False,
            "error": "Data must be a non-empty list"
        }), 400

    df = pd.DataFrame(rows)

    user_col = payload.get("userIdColumn", "userid")
    treatment_col = payload.get("treatmentColumn", "treatment")
    target_col = payload.get("targetColumn", "target")
    features = payload.get("features", [])
    cuped_col = payload.get("cupedColumn")

    required = {treatment_col, target_col, *features}
    if cuped_col:
        required.add(cuped_col)

    missing = required - set(df.columns)
    if missing:
        return jsonify({
            "success": False,
            "error": f"Missing columns: {sorted(missing)}"
        }), 400

    if df[treatment_col].isna().any():
        return jsonify({
            "success": False,
            "error": "Treatment contains missing values"
        }), 400

    if set(df[treatment_col].unique()) != {0, 1}:
        return jsonify({
            "success": False,
            "error": "Treatment must contain both groups encoded as 0 and 1"
        }), 400

    if df[target_col].isna().any():
        return jsonify({
            "success": False,
            "error": "Target contains missing values"
        }), 400

    # Вызов UpliftService с динамической конфигурацией.
    # Названия полей конфигурации нужно согласовать с текущим
    # конструктором UpliftConfig в проекте.

    return jsonify({
        "success": True,
        "message": "Data validated",
        "rows": len(df),
        "userIdColumn": user_col,
        "targetColumn": target_col,
        "treatmentColumn": treatment_col
    })