IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [PasswordHash] varbinary(max) NULL,
    [PasswordSalt] varbinary(max) NULL,
    [Gender] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [KnownAs] nvarchar(max) NULL,
    [Created] datetime2 NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [Introduction] nvarchar(max) NULL,
    [LookingFor] nvarchar(max) NULL,
    [Interests] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [Country] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Values] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_Values] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Likes] (
    [LikerId] int NOT NULL,
    [LikeeId] int NOT NULL,
    CONSTRAINT [PK_Likes] PRIMARY KEY ([LikerId], [LikeeId]),
    CONSTRAINT [FK_Likes_Users_LikeeId] FOREIGN KEY ([LikeeId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Likes_Users_LikerId] FOREIGN KEY ([LikerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] int NOT NULL,
    [RecipientId] int NOT NULL,
    [Content] nvarchar(max) NULL,
    [IsRead] bit NOT NULL,
    [DateRead] datetime2 NULL,
    [MessageSent] datetime2 NOT NULL,
    [SenderDeleted] bit NOT NULL,
    [RecipientDeleted] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_Users_RecipientId] FOREIGN KEY ([RecipientId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Messages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Photos] (
    [Id] int NOT NULL IDENTITY,
    [Url] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [DateAdded] datetime2 NOT NULL,
    [IsMain] bit NOT NULL,
    [PublicId] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Photos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Photos_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Likes_LikeeId] ON [Likes] ([LikeeId]);

GO

CREATE INDEX [IX_Messages_RecipientId] ON [Messages] ([RecipientId]);

GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);

GO

CREATE INDEX [IX_Photos_UserId] ON [Photos] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190131160427_SqlServerFirstMigration', N'2.2.1-servicing-10028');

GO

