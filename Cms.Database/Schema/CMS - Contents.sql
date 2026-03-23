CREATE TABLE Contents
(
    ContentId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    TypeId RAW(16) NOT NULL,

    OwnerId RAW(16) NOT NULL,

    Status VARCHAR2(30 CHAR) NOT NULL,

    CreatedAt TIMESTAMP(6) DEFAULT CAST(SYSTIMESTAMP AS TIMESTAMP(6)) NOT NULL,

    CONSTRAINT PK_Contents
        PRIMARY KEY (ContentId),

    CONSTRAINT FK_Contents_ContentTypes
        FOREIGN KEY (TypeId)
        REFERENCES ContentTypes(TypeId),

    CONSTRAINT FK_Contents_Owner
        FOREIGN KEY (OwnerId)
        REFERENCES Accounts(AccountId)
);

