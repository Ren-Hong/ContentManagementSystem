CREATE TABLE ContentTypes
(
    TypeId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    DepartmentId RAW(16) NOT NULL,

    TypeCode VARCHAR2(50 CHAR) NOT NULL,

    TypeName VARCHAR2(100 CHAR) NOT NULL,

    Description VARCHAR2(255 CHAR),

    IsEnabled NUMBER(1) DEFAULT 1 NOT NULL
        CONSTRAINT CK_ContentTypes_IsEnabled CHECK (IsEnabled IN (0, 1)),

    CreatedAt TIMESTAMP(6) DEFAULT CAST(SYSTIMESTAMP AS TIMESTAMP(6)) NOT NULL,

    CONSTRAINT PK_ContentTypes
        PRIMARY KEY (TypeId),

    CONSTRAINT UQ_ContentTypes_TypeCode
        UNIQUE (TypeCode),

    CONSTRAINT FK_ContentTypes_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
);

