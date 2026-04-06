┌──────────────┐
│   Scopes     │
│──────────────│
│ ScopeId (PK) │◄──────────────┐
│ ScopeCode    │               │
│ Description  │               │
└──────────────┘               │
                               │
                               │
┌──────────────────────────┐   │
│ RolePermissionScope      │   ←「策略層」
│──────────────────────────│   │
│ RoleId        (PK, FK) ──┼───┼──────► Roles.RoleId
│ PermissionId  (PK, FK) ──┼───┼──────► Permissions.PermissionId
│ ScopeId       (FK) ──────┼───┘
│ CreatedAt                │
└──────────────────────────┘
