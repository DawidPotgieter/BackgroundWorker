/****** Object:  Table [dbo].[BackgroundWorkerJobs]    Script Date: 04/02/2012 12:11:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BackgroundWorkerJobs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[UniqueId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](max) NULL,
	[Group] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[MetaData] [nvarchar](max) NULL,
	[AbsoluteTimeout] [time](7) NULL,
	[QueueId] [int] NOT NULL,
	[LastExecutionStartDateTime] [datetime] NULL,
	[LastExecutionEndDateTime] [datetime] NULL,
	[NextExecutionStartDateTime] [datetime] NULL,
	[LastErrorMessage] [nvarchar](max) NULL,
	[Instance] [nvarchar](max) NULL,
	[ScheduleType] [nvarchar](max) NULL,
	[Schedule] [nvarchar](max) NULL,
	[SuppressHistory] [bit] NOT NULL,
	[DeleteWhenDone] [bit] NOT NULL,
 CONSTRAINT [PK_BackgroundWorkerJobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BackgroundWorkerJobExecutionHistory]    Script Date: 04/02/2012 12:11:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BackgroundWorkerJobExecutionHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[StatusId] [int] NOT NULL,
	[JobUniqueId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](max) NULL,
	[Group] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[MetaData] [nvarchar](max) NULL,
	[AbsoluteTimeout] [time](7) NULL,
	[QueueId] [int] NOT NULL,
	[Instance] [nvarchar](max) NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NULL,
	[Success] [bit] NULL,
	[ErrorMessage] [nvarchar](max) NULL,
 CONSTRAINT [PK_BackgroundWorkerJobExecutionHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[BackgroundWorkerAlerts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[JobHistoryId] [bigint] NULL,
	[Message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_BackgroundWorkerAlerts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Default [DF_BackgroundWorkerJobs_StatusId]    Script Date: 04/02/2012 12:11:21 ******/
ALTER TABLE [dbo].[BackgroundWorkerJobs] ADD  CONSTRAINT [DF_BackgroundWorkerJobs_StatusId]  DEFAULT ((0)) FOR [StatusId]
GO
/****** Object:  Default [DF_BackgroundWorkerJobs_UniqueId]    Script Date: 04/02/2012 12:11:21 ******/
ALTER TABLE [dbo].[BackgroundWorkerJobs] ADD  CONSTRAINT [DF_BackgroundWorkerJobs_UniqueId]  DEFAULT (newid()) FOR [UniqueId]
GO
/****** Object:  Default [DF_BackgroundWorkerJobs_CreatedDate]    Script Date: 04/02/2012 12:11:21 ******/
ALTER TABLE [dbo].[BackgroundWorkerJobs] ADD  CONSTRAINT [DF_BackgroundWorkerJobs_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF_BackgroundWorkerJobs_SuppressHistory]    Script Date: 04/02/2012 12:11:21 ******/
ALTER TABLE [dbo].[BackgroundWorkerJobs] ADD  CONSTRAINT [DF_BackgroundWorkerJobs_SuppressHistory]  DEFAULT ((0)) FOR [SuppressHistory]
GO
/****** Object:  Default [DF_BackgroundWorkerJobs_DeleteWhenDone]    Script Date: 04/02/2012 12:11:21 ******/
ALTER TABLE [dbo].[BackgroundWorkerJobs] ADD  CONSTRAINT [DF_BackgroundWorkerJobs_DeleteWhenDone]  DEFAULT ((0)) FOR [DeleteWhenDone]
GO
