CREATE TABLE dbo.ErrorLog (
    ErrorLogId int NOT NULL IDENTITY,
    LogTime datetime NOT NULL
        CONSTRAINT DF_ErrorLog_LogTime DEFAULT GETUTCDATE(),

    ProcessId int NULL,
    WorkerId int NULL,
    OperationId int NULL,
    ProcedureName nvarchar(128) NULL,
    LineNum int NOT NULL,
    ErrorNum int NOT NULL,
    ErrorMessage nvarchar(4000) NOT NULL,

    UserLogin nvarchar(128) NOT NULL
        CONSTRAINT DF_ErrorLog_UserLogin DEFAULT SYSTEM_USER,
    UserHost  nvarchar(128) NOT NULL
        CONSTRAINT DF_ErrorLog_UserHost DEFAULT HOST_NAME(),

    CONSTRAINT PK_ErrorLog PRIMARY KEY (ErrorLogId)
    WITH (OPTIMIZE_FOR_SEQUENTIAL_KEY = ON)
);
GO
CREATE INDEX IX_ErrorLog_LogTime ON dbo.ErrorLog (LogTime ASC);
GO
CREATE INDEX IX_ErrorLog_ProcessId ON dbo.ErrorLog (ProcessId ASC);
GO
CREATE INDEX IX_ErrorLog_WorkerId ON dbo.ErrorLog (WorkerId ASC);
GO
CREATE INDEX IX_ErrorLog_OperationId ON dbo.ErrorLog (OperationId ASC);