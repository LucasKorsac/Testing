const schemas = {
    Company: { Name: "text" },
    Role: { Name: "text" },
    Developer: {
        CompanyId: "text",
        RoleId: "text",
        Login: "text",
        Password: "text"
    },
    Application: {
        CompanyId: "text",
        Name: "text",
        Description: "text"
    },
    ABTest: {
        ApplicationId: "text",
        DescriptionId: "text",
        Name: "text"
    },
    Variant: {
        AbTestId: "text",
        Name: "text",
        Description: "text"
    }
};