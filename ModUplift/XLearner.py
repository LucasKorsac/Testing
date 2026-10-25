class XLearner(UpliftModel):

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

        # μ0

        self.mu0 = (
            self.create_regressor()
        )

        self.mu0.fit(
            X[control],
            y[control]
        )

        # μ1

        self.mu1 = (
            self.create_regressor()
        )

        self.mu1.fit(
            X[treated],
            y[treated]
        )

        # D1

        d1 = (
            y[treated]
            - self.mu0.predict(
                X[treated]
            )
        )

        # D0

        d0 = (
            self.mu1.predict(
                X[control]
            )
            - y[control]
        )

        # τ1

        self.tau1 = (
            self.create_regressor()
        )

        self.tau1.fit(
            X[treated],
            d1
        )

        # τ0

        self.tau0 = (
            self.create_regressor()
        )

        self.tau0.fit(
            X[control],
            d0
        )

        # Propensity

        self.propensity = (
            LogisticRegression(
                max_iter=self.config.logistic_max_iter
            )
        )

        self.propensity.fit(
            X,
            treatment
        )

        return self

    def predict(self, data):

        X = data[
            self.config.features
        ]

        propensity = (
            self.propensity
            .predict_proba(X)[:, 1]
        )

        propensity = np.clip(
            propensity,
            self.config.min_propensity,
            self.config.max_propensity
        )

        tau0 = (
            self.tau0.predict(X)
        )

        tau1 = (
            self.tau1.predict(X)
        )

        return (
            propensity * tau0
            + (1 - propensity) * tau1
        )