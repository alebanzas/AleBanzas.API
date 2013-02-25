BEGIN TRANSACTION
GO
CREATE TABLE [SUBE_Recarga]
	(
	ID uniqueidentifier NOT NULL,
	nombre nvarchar(MAX) NULL,
	lat float(53) NULL,
	lon float(53) NULL,
	ubicacion geometry NULL
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
	ubicacion geometry NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
COMMIT
