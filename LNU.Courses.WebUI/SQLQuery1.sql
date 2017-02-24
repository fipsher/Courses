Create table Variables(
	[Key] nvarchar(20) not null primary key,
	[Value] datetime not null
);


INSERT INTO [dbo].[Variables]
           ([Key]
           ,[Value])
     VALUES
           ('Start', '2017-09-01 12:00:00.000'),
		   ('FirstDeadline', '2017-10-01  12:00:00.000'),
		   ('LastDeadline', '2017-11-01  12:00:00.000')