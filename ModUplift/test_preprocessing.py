"""Unit tests for ModUplift.preprocessing.validate_and_prepare.

Run from this directory with:
    python -m pytest test_preprocessing.py
"""
import pandas as pd
import pytest

from config import UpliftConfig
from preprocessing import validate_and_prepare


@pytest.fixture
def config():
    return UpliftConfig(
        target="conversion",
        treatment="treatment",
        user_id="user_id",
        features=["age"],
    )


@pytest.fixture
def valid_data():
    return pd.DataFrame(
        {
            "user_id": [101, 102, 103, 104],
            "treatment": [0, 0, 1, 1],
            "conversion": [0, 1, 1, 1],
            "age": [20, 30, 40, 50],
        }
    )


def test_valid_data_normalizes_treatment_and_preserves_input(config, valid_data):
    original = valid_data.copy()

    result = validate_and_prepare(valid_data, config)

    assert result["treatment"].tolist() == [0, 0, 1, 1]
    pd.testing.assert_frame_equal(valid_data, original)
    assert result is not valid_data


def test_empty_data_is_rejected(config):
    with pytest.raises(ValueError):
        validate_and_prepare(pd.DataFrame(), config)


def test_missing_required_column_is_rejected(config, valid_data):
    data = valid_data.drop(columns=["age"])

    with pytest.raises(ValueError, match="age"):
        validate_and_prepare(data, config)


def test_duplicate_user_ids_are_rejected(config, valid_data):
    valid_data.loc[1, "user_id"] = valid_data.loc[0, "user_id"]

    with pytest.raises(ValueError):
        validate_and_prepare(valid_data, config)


def test_missing_target_value_is_rejected(config, valid_data):
    valid_data.loc[0, "conversion"] = float("nan")

    with pytest.raises(ValueError):
        validate_and_prepare(valid_data, config)


def test_infinite_feature_value_is_rejected(config, valid_data):
    valid_data.loc[0, "age"] = float("inf")

    with pytest.raises(ValueError):
        validate_and_prepare(valid_data, config)


def test_data_with_only_one_treatment_group_is_rejected(config, valid_data):
    valid_data["treatment"] = 0

    with pytest.raises(ValueError):
        validate_and_prepare(valid_data, config)


def test_each_group_must_have_at_least_two_rows(config, valid_data):
    valid_data.loc[3, "treatment"] = 0

    with pytest.raises(ValueError):
        validate_and_prepare(valid_data, config)
