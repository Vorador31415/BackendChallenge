
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/21/2021 20:10:43
-- Generated from EDMX file: C:\Users\Rico\source\repos\ProgramDataAccess\ProgramDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Database1];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Program]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Program];
GO
IF OBJECT_ID(N'[dbo].[Projects]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Projects];
GO
IF OBJECT_ID(N'[dbo].[Tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tasks];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Program'
CREATE TABLE [dbo].[Program] (
    [Id] int  NOT NULL,
    [Name] nchar(10)  NULL,
    [Description] nchar(10)  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [Timestamp] int  NULL,
    [Duration] int  NULL
);
GO

-- Creating table 'Projects'
CREATE TABLE [dbo].[Projects] (
    [Id] int  NOT NULL,
    [Name] nchar(10)  NULL,
    [Description] nchar(10)  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [ProgramOwner] int  NULL,
    [Timestamp] int  NULL,
    [Duration] int  NULL,
    [ProgramId] int  NOT NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [Id] int  NOT NULL,
    [Name] nchar(10)  NULL,
    [Description] nchar(10)  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [DepentsOn] int  NULL,
    [ProjectOwner] int  NULL,
    [Timestamp] int  NULL,
    [Duration] int  NULL,
    [ProjectsId] int  NOT NULL,
    [Tasks2_Id] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Program'
ALTER TABLE [dbo].[Program]
ADD CONSTRAINT [PK_Program]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [PK_Projects]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ProgramId] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [FK_ProgramProjects]
    FOREIGN KEY ([ProgramId])
    REFERENCES [dbo].[Program]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProgramProjects'
CREATE INDEX [IX_FK_ProgramProjects]
ON [dbo].[Projects]
    ([ProgramId]);
GO

-- Creating foreign key on [ProjectsId] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_ProjectsTasks]
    FOREIGN KEY ([ProjectsId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectsTasks'
CREATE INDEX [IX_FK_ProjectsTasks]
ON [dbo].[Tasks]
    ([ProjectsId]);
GO

-- Creating foreign key on [Tasks2_Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_Dependency]
    FOREIGN KEY ([Tasks2_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Dependency'
CREATE INDEX [IX_FK_Dependency]
ON [dbo].[Tasks]
    ([Tasks2_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------