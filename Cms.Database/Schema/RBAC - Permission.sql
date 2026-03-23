CREATE TABLE Permissions
(
    PermissionId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    PermissionCode VARCHAR2(120 CHAR) NOT NULL,   -- e.g. Content.Publish

    PermissionName VARCHAR2(200 CHAR) NULL,

    Status NUMBER(5) DEFAULT 1 NOT NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    UpdatedAt TIMESTAMP(0) NULL,

    CONSTRAINT PK_Permissions
        PRIMARY KEY (PermissionId),

    CONSTRAINT UQ_Permissions_PermissionCode
        UNIQUE (PermissionCode)
);

INSERT ALL
    INTO Permissions (PermissionCode, PermissionName)
    VALUES ('Content.View',   UNISTR('\5167\5BB9\67E5\95B1'))
    INTO Permissions (PermissionCode, PermissionName)
    VALUES ('Content.Create', UNISTR('\5167\5BB9\65B0\589E'))
    INTO Permissions (PermissionCode, PermissionName)
    VALUES ('Content.Edit',   UNISTR('\5167\5BB9\7DE8\8F2F'))
    INTO Permissions (PermissionCode, PermissionName)
    VALUES ('Content.Delete', UNISTR('\5167\5BB9\522A\9664'))
SELECT 1 FROM DUAL;
