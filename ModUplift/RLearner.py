class RLearner(UpliftModel):

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

        # m(x)

        self.outcome_model = (
            self.create_regressor()
        )

        self.outcome_model.fit(
            X,
            y
        )

        m_hat = (
            self.outcome_model.predict(X)
        )

        # e(x)

        self.treatment_probability = (
            float(treatment.mean())
        )

        t = treatment.to_numpy(
            dtype=float
        )

        y_values = y.to_numpy(
            dtype=float
        )

        m_values = np.asarray(
            m_hat
        )

        t_residual = (
            t
            - self.treatment_probability
        )

        # Защита от деления на 0

        epsilon = 1e-6

        t_residual = np.where(
            np.abs(t_residual) < epsilon,
            np.where(
                t_residual >= 0,
                epsilon,
                -epsilon
            ),
            t_residual
        )

        # Pseudo outcome

        pseudo_effect = (
            y_values - m_values
        ) / t_residual

        # τ(x)

        self.effect_model = (
            self.create_regressor()
        )

        self.effect_model.fit(
            X,
            pseudo_effect
        )

        return self

    def predict(self, data):

        X = data[
            self.config.features
        ]

        return (
            self.effect_model
            .predict(X)
        )