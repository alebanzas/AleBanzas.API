CREATE TABLE [DOLAR_Historico]
	(
	ID uniqueidentifier NOT NULL,
	date date NOT NULL,
	valorCompra float(53) NULL,
	valorVenta float(53) NULL,
	tipoMoneda int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [DOLAR_Historico] ADD CONSTRAINT
	PK_DOLAR_Historico PRIMARY KEY CLUSTERED 
	(
	ID
	)
