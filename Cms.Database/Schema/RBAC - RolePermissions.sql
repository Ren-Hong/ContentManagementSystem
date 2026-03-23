CREATE TABLE RolePermissions
(
    RoleId RAW(16) NOT NULL,
    PermissionId RAW(16) NOT NULL,
    ScopeId RAW(16) NOT NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    CONSTRAINT PK_RolePermissions
        PRIMARY KEY (RoleId, PermissionId, ScopeId),

    CONSTRAINT FK_RolePermissions_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles (RoleId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissions_Permissions
        FOREIGN KEY (PermissionId)
        REFERENCES Permissions (PermissionId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolePermissions_Scopes
        FOREIGN KEY (ScopeId)
        REFERENCES Scopes (ScopeId)
);

INSERT INTO RolePermissions (RoleId, PermissionId, ScopeId)
SELECT
    r.RoleId,
    p.PermissionId,
    s.ScopeId
FROM Roles r
JOIN Permissions p
    ON p.PermissionCode LIKE 'Content.%'
JOIN Scopes s
    ON s.ScopeCode = 'Department'
WHERE r.RoleCode = 'Admin';
