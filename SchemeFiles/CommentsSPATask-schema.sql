IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Captchas] (
    [Id] uniqueidentifier NOT NULL,
    [CodeHash] nvarchar(max) NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [FailedAttemptsCount] int NOT NULL,
    CONSTRAINT [PK_Captchas] PRIMARY KEY ([Id])
);

CREATE TABLE [Comments] (
    [Id] uniqueidentifier NOT NULL,
    [ParentId] uniqueidentifier NULL,
    [UserName] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [HomePage] nvarchar(250) NULL,
    [Text] nvarchar(2500) NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_Comments_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Comments] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Attachments] (
    [Id] uniqueidentifier NOT NULL,
    [CommentId] uniqueidentifier NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [StoredFileName] nvarchar(max) NOT NULL,
    [ContentType] int NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Attachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attachments_Comments_CommentId] FOREIGN KEY ([CommentId]) REFERENCES [Comments] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Attachments_CommentId] ON [Attachments] ([CommentId]);

CREATE INDEX [IX_Comments_ParentId] ON [Comments] ([ParentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260510121132_CreateDB', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Captchas]') AND [c].[name] = N'FailedAttemptsCount');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Captchas] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Captchas] DROP COLUMN [FailedAttemptsCount];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260511001000_RemoveCaptchaFailedAttempts', N'10.0.0');

COMMIT;
GO

