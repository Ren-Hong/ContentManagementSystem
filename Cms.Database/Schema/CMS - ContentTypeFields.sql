CREATE TABLE ContentTypeFields (
    TypeId RAW(16) NOT NULL,
    FieldId RAW(16) NOT NULL,
    SortOrder NUMBER(10) DEFAULT 0 NOT NULL, -- Display order

    CONSTRAINT PK_ContentTypeFields
        PRIMARY KEY (TypeId, FieldId),

    CONSTRAINT FK_ContentTypeFields_ContentTypes
        FOREIGN KEY (TypeId)
        REFERENCES ContentTypes(TypeId),

    CONSTRAINT FK_ContentTypeFields_ContentFields
        FOREIGN KEY (FieldId)
        REFERENCES ContentFields(FieldId)
);
