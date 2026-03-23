CREATE TABLE Accounts
(
    AccountId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    Username VARCHAR2(50 CHAR) NOT NULL,

    PasswordHash VARCHAR2(255 CHAR) NOT NULL,

    Status NUMBER(5) NOT NULL,

    CreatedAt TIMESTAMP(0) DEFAULT CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS TIMESTAMP(0)) NOT NULL,

    UpdatedAt TIMESTAMP(0) NULL,

    LastLoginAt TIMESTAMP(0) NULL,

    CONSTRAINT PK_Accounts
        PRIMARY KEY (AccountId),

    CONSTRAINT UQ_Accounts_Username
        UNIQUE (Username)
);

INSERT INTO Accounts
(
    Username,
    PasswordHash,
    Status
)
VALUES
(
    'admin',
    '$2a$11$fp9EDc/tghYChbRxOh9rEO9mcy/akdOTAyK2lm2NzmCzI9RaLnS3q',
    1
);

COMMIT;
