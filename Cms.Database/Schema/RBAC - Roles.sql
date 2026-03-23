CREATE TABLE Roles
(
    RoleId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    RoleCode VARCHAR2(50 CHAR) NOT NULL,    -- Admin / Editor / Reviewer
    RoleName VARCHAR2(100 CHAR) NOT NULL,

    Status NUMBER(5) DEFAULT 1 NOT NULL,  -- 1 = Enabled, 2 = Disabled

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    UpdatedAt TIMESTAMP(0) NULL,

    CONSTRAINT PK_Roles
        PRIMARY KEY (RoleId),

    CONSTRAINT UQ_Roles_RoleCode
        UNIQUE (RoleCode)
);

INSERT INTO Roles (RoleCode, RoleName)
VALUES
('Admin', UNISTR('\7CFB\7D71\7BA1\7406\54E1'));
