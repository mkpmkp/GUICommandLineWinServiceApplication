CREATE TABLE [GIS].[Async_Operations] (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ThreadGUID] [varchar](50) NULL,
	[MessageGuid] [varchar](50) NULL,
	[MessageStatus] [int] NULL,
	[C_Operation_Name] [varchar](50) NULL,
	[C_Operation_Description] [varchar](250) NULL,
	[D_operation_Date] [smalldatetime] NULL,
	[Pack_Error] [varchar](250) NULL,
	CONSTRAINT [idx_GIS_Async_Operations_ID] PRIMARY KEY CLUSTERED (
		[ID] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [Data]
) ON [Data]


-- ALTER TABLE [GIS].[Async_Operations]
-- ADD CONSTRAINT idx_GIS_Async_Operations_ID PRIMARY KEY (ID);


CREATE NONCLUSTERED INDEX [idx_GIS_Async_Operations_ThreadGUID] ON [GIS].[Async_Operations] (
	[ThreadGUID] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);


CREATE NONCLUSTERED INDEX [idx_GIS_Async_Operations_MessageGUID] ON [GIS].[Async_Operations] (
	[MessageGuid] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);


CREATE NONCLUSTERED INDEX [idx_GIS_Async_Operations_OperationName] ON [GIS].[Async_Operations] (
	[C_Operation_Name] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

GO
