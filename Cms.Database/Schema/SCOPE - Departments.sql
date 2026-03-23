CREATE TABLE Departments
(
    DepartmentId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    DepartmentCode VARCHAR2(50 CHAR) NOT NULL,
    DepartmentName VARCHAR2(100 CHAR) NOT NULL,

    Status NUMBER(5) DEFAULT 1 NOT NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    UpdatedAt TIMESTAMP(0) NULL,

    CONSTRAINT PK_Departments
        PRIMARY KEY (DepartmentId),

    CONSTRAINT UQ_Departments_DepartmentCode
        UNIQUE (DepartmentCode)
);

INSERT INTO Departments
(
    DepartmentCode,
    DepartmentName
)
VALUES
('IT', UNISTR('\8CC7\8A0A\90E8'));
