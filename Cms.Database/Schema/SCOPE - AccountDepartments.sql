CREATE TABLE AccountDepartments
(
    AccountId RAW(16) NOT NULL,
    DepartmentId RAW(16) NOT NULL,

    IsPrimary NUMBER(1) DEFAULT 0 NOT NULL
        CONSTRAINT CK_AccountDepartments_IsPrimary CHECK (IsPrimary IN (0, 1)),

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    CONSTRAINT PK_AccountDepartments
        PRIMARY KEY (AccountId, DepartmentId),

    CONSTRAINT FK_AccountDepartments_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_AccountDepartments_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
        ON DELETE CASCADE
);

INSERT INTO AccountDepartments
(
    AccountId,
    DepartmentId,
    IsPrimary
)
SELECT
    a.AccountId,
    d.DepartmentId,
    1 -- Primary department
FROM Accounts a
JOIN Departments d
    ON d.DepartmentCode = 'IT'
WHERE a.Username = 'admin';
