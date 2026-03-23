CREATE TABLE ContentRevisions
(
    RevisionId RAW(16) DEFAULT SYS_GUID() NOT NULL,

    ContentId RAW(16) NOT NULL,
    Version NUMBER(10) NOT NULL,

    CreatedAt TIMESTAMP(6) DEFAULT CAST(SYSTIMESTAMP AS TIMESTAMP(6)) NOT NULL,

    CONSTRAINT PK_ContentRevisions
        PRIMARY KEY (RevisionId),

    CONSTRAINT FK_ContentRevisions_Contents
        FOREIGN KEY (ContentId)
        REFERENCES Contents(ContentId),

    CONSTRAINT UQ_ContentRevisions_Content_Version
        UNIQUE (ContentId, Version)
);

