/*
ASPdb_Users
ASPdb_Connections
ASPdb_Tables
ASPdb_UserGroups
ASPdb_UsersToGroups
ASPdb_Values
ASPdb_Permissions
*/

/* **************************************************************************************************** */
/*
USE [ASPdb_AppData]
GO
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE SCHEMA [ASPdb]
GO



/* **************************************************************************************************** */
/* ASPdb_Users **************************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_Users](
	[UserId] [int] IDENTITY(8000,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[TimeCreated] [datetime] NOT NULL,
	[LastLoginTime] [datetime] NULL,
	[RequirePasswordReset] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_ASPdb_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
)  
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT (N'') FOR [FirstName]
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT (N'') FOR [LastName]
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT (N'') FOR [Email]
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT ((0)) FOR [RequirePasswordReset]
GO
ALTER TABLE [ASPdb].[ASPdb_Users] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO

/* **************************************************************************************************** */
/* ASPdb_Connections ********************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_Connections](
	[ConnectionId] [int] IDENTITY(7000,1) NOT NULL,
	[SiteId] [int] NOT NULL,
	[ConnectionName] [nvarchar](100) NOT NULL,
	[ConnectionType] [nvarchar](50) NOT NULL,
	[ParametersType] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[DateTimeCreated] [datetime] NULL,
	[CreatedByUserId] [int] NULL,
	[Param_ServerAddress] [nvarchar](255) NOT NULL,
	[Param_DatabaseName] [nvarchar](255) NOT NULL,
	[Param_U] [nvarchar](255) NOT NULL,
	[Param_P] [ntext] NOT NULL,
	[Param_ConnectionString] [ntext] NOT NULL,
 CONSTRAINT [PK_ASPdb_Databases] PRIMARY KEY CLUSTERED 
(
	[ConnectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
)   
GO

ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_SiteId]  DEFAULT ((1)) FOR [SiteId]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_DatabaseType]  DEFAULT ('') FOR [ConnectionType]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_ParametersType]  DEFAULT ('') FOR [ParametersType]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_ServerAddress]  DEFAULT ('') FOR [Param_ServerAddress]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_DatabaseName]  DEFAULT ('') FOR [Param_DatabaseName]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_Username]  DEFAULT ('') FOR [Param_U]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_P]  DEFAULT ('') FOR [Param_P]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_ConnectionString_1]  DEFAULT ('') FOR [Param_ConnectionString]
GO
ALTER TABLE [ASPdb].[ASPdb_Connections]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_Databases_ASPdb_Users] FOREIGN KEY([CreatedByUserId])
REFERENCES [ASPdb].[ASPdb_Users] ([UserId])
GO
ALTER TABLE [ASPdb].[ASPdb_Connections] CHECK CONSTRAINT [FK_ASPdb_Databases_ASPdb_Users]
GO

/* **************************************************************************************************** */
/* ASPdb_Tables *************************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_Tables](
	[TableId] [int] IDENTITY(1,1) NOT NULL,
	[ConnectionId] [int] NOT NULL,
	[Schema] [nvarchar](100) NOT NULL,
	[TableName] [nvarchar](250) NOT NULL,
	[Hide] [bit] NOT NULL,
 CONSTRAINT [PK_ASPdb_Tables] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
) ON [PRIMARY]
GO
ALTER TABLE [ASPdb].[ASPdb_Tables] ADD  DEFAULT (N'dbo') FOR [Schema]
GO
ALTER TABLE [ASPdb].[ASPdb_Tables] ADD  DEFAULT ((0)) FOR [Hide]
GO
ALTER TABLE [ASPdb].[ASPdb_Tables]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_Tables_ASPdb_Connections] FOREIGN KEY([ConnectionId])
REFERENCES [ASPdb].[ASPdb_Connections] ([ConnectionId])
GO
ALTER TABLE [ASPdb].[ASPdb_Tables] CHECK CONSTRAINT [FK_ASPdb_Tables_ASPdb_Connections]
GO

/* **************************************************************************************************** */
/* ASPdb_UserGroups *********************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_UserGroups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[TimeCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_ASPdb_UserGroups] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO
ALTER TABLE [ASPdb].[ASPdb_UserGroups] ADD  DEFAULT ((1)) FOR [Active]
GO

/* **************************************************************************************************** */
/* ASPdb_UsersToGroups ******************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_UsersToGroups](
	[UserId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_ASPdb_UserAssignments] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO
ALTER TABLE [ASPdb].[ASPdb_UsersToGroups]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_UserGroups] FOREIGN KEY([GroupId])
REFERENCES [ASPdb].[ASPdb_UserGroups] ([GroupId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [ASPdb].[ASPdb_UsersToGroups] CHECK CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_UserGroups]
GO
ALTER TABLE [ASPdb].[ASPdb_UsersToGroups]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_Users] FOREIGN KEY([UserId])
REFERENCES [ASPdb].[ASPdb_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [ASPdb].[ASPdb_UsersToGroups] CHECK CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_Users]
GO

/* **************************************************************************************************** */
/* ASPdb_Values *************************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_Values](
	[ValueId] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Value] [ntext] NULL,
 CONSTRAINT [PK_ASPdb_ASPdb_Values] PRIMARY KEY CLUSTERED 
(
	[ValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)  
GO

/* **************************************************************************************************** */
/* ASPdb_Permissions ********************************************************************************** */
CREATE TABLE [ASPdb].[ASPdb_Permissions](
	[PermissionId] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[PermissionType] [nvarchar](1) NOT NULL,
	[ConnectionId] [int] NULL,
	[Schema] [nvarchar](100) NULL,
	[TableId] [int] NULL,
	[View] [bit] NOT NULL,
	[Edit] [bit] NOT NULL,
	[Insert] [bit] NOT NULL,
	[Delete] [bit] NOT NULL,
 CONSTRAINT [PK_dbo_ASPdb_UserGroups_ToTables] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] ADD  DEFAULT (N'N') FOR [PermissionType]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [View]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Edit]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Insert]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Delete]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_ASPdb_Permissions] FOREIGN KEY([GroupId])
REFERENCES [ASPdb].[ASPdb_UserGroups] ([GroupId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] CHECK CONSTRAINT [FK_ASPdb_ASPdb_Permissions]
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_ASPdb_Permissions_0001] FOREIGN KEY([TableId])
REFERENCES [ASPdb].[ASPdb_Tables] ([TableId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [ASPdb].[ASPdb_Permissions] CHECK CONSTRAINT [FK_ASPdb_ASPdb_Permissions_0001]
GO








