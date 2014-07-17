create table dbo.InetForm (
    ID UNIQUEIDENTIFIER not null,
    Team NVARCHAR(255) null,
    FileUrl NVARCHAR(255) null,
    Institucion NVARCHAR(255) null,
    Provincia NVARCHAR(255) null,
	[Date] DATETIME,
    primary key (ID)
)
create table dbo.InetFormMember (
    ID UNIQUEIDENTIFIER not null,
    Nombre NVARCHAR(255) null,
    Apellido NVARCHAR(255) null,
    Email NVARCHAR(255) null,
    primary key (ID)
)
alter table dbo.InetFormMember 
    add constraint FK59086131543E97D5 
    foreign key (ID) 
    references dbo.InetForm