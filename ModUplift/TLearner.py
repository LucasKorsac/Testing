class TLearner(UpliftModel):

    def fit(self, data):

        X = data[
            self.config.features
        ]

        y = data[
            self.get_target(data)
        ]

        treatment = data[
            self.config.treatment
        ]

        control_mask = (
            treatment == 0
        )

        treatment_mask = (
            treatment == 1
        )

        if control_mask.sum() == 0:

            raise ValueError(
                "Нет контрольной группы"
            )

        if treatment_mask.sum() == 0:

            raise ValueError(
                "Нет экспериментальной группы"
            )

        # Модель контроля

        self.control_model = (
            self.create_regressor()
        )

        self.control_model.fit(
            X[control_mask],
            y[control_mask]
        )

        # Модель эксперимента

        self.treatment_model = (
            self.create_regressor()
        )

        self.treatment_model.fit(
            X[treatment_mask],
            y[treatment_mask]
        )

        return self

    def predict(self, data):

        X = data[
            self.config.features
        ]

        control_prediction = (
            self.control_model
            .predict(X)
        )

        treatment_prediction = (
            self.treatment_model
            .predict(X)
        )

        return (
            treatment_prediction
            - control_prediction
        )