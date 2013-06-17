BEGIN TRANSACTION
GO
CREATE TABLE [SUBE_Recarga]
	(
	ID uniqueidentifier NOT NULL,
	nombre nvarchar(MAX) NULL,
	lat float(53) NULL,
	lon float(53) NULL,
	ubicacion geography NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
COMMIT


BEGIN TRANSACTION
GO
CREATE TABLE [SUBE_Venta]
	(
	ID uniqueidentifier NOT NULL,
	nombre nvarchar(MAX) NULL,
	lat float(53) NULL,
	lon float(53) NULL,
	ubicacion geography NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
COMMIT

ALTER TABLE dbo.SUBE_Recarga ADD CONSTRAINT
	PK_SUBE_Recarga PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO

ALTER TABLE dbo.SUBE_Venta ADD CONSTRAINT
	PK_SUBE_Venta PRIMARY KEY CLUSTERED 
	(
	ID
	)
GO