CREATE TABLE Scopes
(
    ScopeId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    ScopeCode VARCHAR2(30 CHAR) NOT NULL,   -- Global / Department / Self / Assigned

    ScopeName VARCHAR2(100 CHAR) NULL,

    CONSTRAINT PK_Scopes
        PRIMARY KEY (ScopeId),

    CONSTRAINT UQ_Scopes_ScopeCode
        UNIQUE (ScopeCode)
);

INSERT ALL
    INTO Scopes (ScopeCode, ScopeName)
    VALUES ('Global',     UNISTR('\5168\7CFB\7D71\7BC4\570D'))
    INTO Scopes (ScopeCode, ScopeName)
    VALUES ('Department', UNISTR('\6240\5C6C\90E8\9580'))
    INTO Scopes (ScopeCode, ScopeName)
    VALUES ('Self',       UNISTR('\50C5\9650\672C\4EBA'))
    INTO Scopes (ScopeCode, ScopeName)
    VALUES ('Assigned',   UNISTR('\660E\78BA\6307\6D3E'))
SELECT 1 FROM DUAL;

COMMIT;

