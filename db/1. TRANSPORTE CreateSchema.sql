CREATE TABLE dbo.GUIATBA_TipoTransporte
	(
	ID uniqueidentifier NOT NULL,
	Nombre varchar(50) NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.GUIATBA_Transporte
	(
	ID uniqueidentifier NOT NULL,
	TipoTransporteID uniqueidentifier NOT NULL,
	Nombre varchar(100) NULL,
	Codigo nvarchar(50) NOT NULL,
	Ramal nvarchar(100) NOT NULL,
	Ubicacion geography NOT NULL,
	DescripcionRecorrido text NULL,
	Regreso bit NOT NULL CONSTRAINT DF_GUIATBA_TipoTransporte_Regreso DEFAULT ((0))
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE dbo.GUIATBA_TransporteIntersecciones
	(
	ID uniqueidentifier NOT NULL,
	CodigoO nvarchar(50) NOT NULL,
	CodigoD nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.GUIATBA_TipoTransporte ADD CONSTRAINT
	PK_GUIATBA_TipoTransporte PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO

ALTER TABLE dbo.GUIATBA_Transporte ADD CONSTRAINT
	PK_GUIATBA_Transporte PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO

ALTER TABLE dbo.GUIATBA_TransporteIntersecciones ADD CONSTRAINT
	PK_GUIATBA_TransporteIntersecciones PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO

ALTER TABLE dbo.GUIATBA_TransporteIntersecciones ADD
	ID uniqueidentifier NULL
GO
UPDATE dbo.GUIATBA_TransporteIntersecciones SET ID = NEWID()
ALTER TABLE dbo.GUIATBA_TransporteIntersecciones ALTER COLUMN ID uniqueidentifier NOT NULL
ALTER TABLE dbo.GUIATBA_TransporteIntersecciones ADD CONSTRAINT
	PK_GUIATBA_TransporteIntersecciones PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO


INSERT INTO [GUIATBA_TipoTransporte] ([ID],[Nombre]) VALUES ('E74E932C-BF15-4ED6-ADA6-F0CBF0688B78','Subte')
INSERT INTO [GUIATBA_TipoTransporte] ([ID],[Nombre]) VALUES ('440C21D3-71DE-4C94-849D-66139EADCE4C','Tren')
INSERT INTO [GUIATBA_TipoTransporte] ([ID],[Nombre]) VALUES ('8C9A672B-9103-47BF-A373-0648C0F10C5C','Colectivo')
