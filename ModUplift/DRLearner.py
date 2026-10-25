class DRLearner(UpliftModel):

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

        control = (
            treatment == 0
        )

        treated = (
            treatment == 1
        )

        # μ0(x)

        self.mu0 = (
            self.create_regressor()
        )

        self.mu0.fit(
            X[control],
            y[control]
        )

        # μ1(x)

        self.mu1 = (
            self.create_regressor()
        )

        self.mu1.fit(
            X[treated],
            y[treated]
        )

        mu0 = self.mu0.predict(X)
        mu1 = self.mu1.predict(X)

        # Propensity e(x)

        self.propensity = (
            LogisticRegression(
                max_iter=self.config.logistic_max_iter
            )
        )

        self.propensity.fit(
            X,
            treatment
        )

        e = (
            self.propensity
            .predict_proba(X)[:, 1]
        )

        e = np.clip(
            e,
            self.config.min_propensity,
            self.config.max_propensity
        )

        # Doubly Robust target

        y_values = y.to_numpy(
            dtype=float
        )

        t_values = treatment.to_numpy(
            dtype=float
        )

        pseudo_effect = (
            (mu1 - mu0)
            +
            t_values
            * (y_values - mu1)
            / e
            -
            (1 - t_values)
            * (y_values - mu0)
            / (1 - e)
        )

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