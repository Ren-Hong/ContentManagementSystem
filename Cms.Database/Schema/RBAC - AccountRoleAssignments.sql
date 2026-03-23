CREATE TABLE AccountRoleAssignments
(
    AccountId RAW(16) NOT NULL,

    RoleId RAW(16) NOT NULL,

    DepartmentId RAW(16) NOT NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    CONSTRAINT PK_AccountRolesAssignments
        PRIMARY KEY (AccountId, RoleId, DepartmentId),

    CONSTRAINT FK_AccountRolesAssignments_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountRolesAssignments_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountRolesAssignments_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
        ON DELETE CASCADE
);

INSERT INTO AccountRoleAssignments (AccountId, RoleId, DepartmentId)
SELECT
    a.AccountId,
    r.RoleId,
    d.DepartmentId
FROM Accounts a
JOIN Roles r ON r.RoleCode = 'Admin'
JOIN Departments d ON d.DepartmentCode = 'IT'
WHERE a.Username = 'admin';
