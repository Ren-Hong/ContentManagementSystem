CREATE TABLE ContentFields (
    FieldId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    FieldCode VARCHAR2(50 CHAR) NOT NULL,

    FieldType VARCHAR2(30 CHAR) NOT NULL,   -- text / richtext / number / enum

    IsRequired NUMBER(1) DEFAULT 0 NOT NULL
        CONSTRAINT CK_ContentFields_IsRequired CHECK (IsRequired IN (0, 1)), -- Required field flag

    CreatedAt TIMESTAMP(6) DEFAULT CAST(SYSTIMESTAMP AS TIMESTAMP(6)) NOT NULL,

    CONSTRAINT PK_ContentFields
        PRIMARY KEY (FieldId),

    CONSTRAINT UQ_ContentFields_FieldCode
        UNIQUE (FieldCode)
);
