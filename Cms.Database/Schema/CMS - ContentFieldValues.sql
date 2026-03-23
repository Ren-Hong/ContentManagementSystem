CREATE TABLE ContentFieldValues (
    RevisionId RAW(16) NOT NULL,

    FieldId RAW(16) NOT NULL,

    FieldValue VARCHAR2(4000 CHAR) NULL,

    CONSTRAINT PK_ContentFieldValues
        PRIMARY KEY (RevisionId, FieldId),

    CONSTRAINT FK_ContentFieldValues_ContentRevisions
        FOREIGN KEY (RevisionId)
        REFERENCES ContentRevisions(RevisionId),

    CONSTRAINT FK_ContentFieldValues_ContentFields
        FOREIGN KEY (FieldId)
        REFERENCES ContentFields(FieldId)
);
