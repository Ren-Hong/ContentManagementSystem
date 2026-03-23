CREATE TABLE DepartmentPermissionAssignments
(
    AccountId RAW(16) NOT NULL,
    PermissionId RAW(16) NOT NULL,
    DepartmentId RAW(16) NOT NULL,

    ValidFrom TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    ValidTo TIMESTAMP(0) NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    CONSTRAINT PK_DepartmentPermissionAssignments
        PRIMARY KEY (AccountId, DepartmentId, PermissionId),

    CONSTRAINT FK_DPA_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
        ON DELETE CASCADE,

    CONSTRAINT FK_DPA_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
        ON DELETE CASCADE,

    CONSTRAINT FK_DPA_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES Permissions(PermissionId)
        ON DELETE CASCADE
);

CREATE INDEX IX_DPA_Department_Permission
ON DepartmentPermissionAssignments (DepartmentId, PermissionId);
