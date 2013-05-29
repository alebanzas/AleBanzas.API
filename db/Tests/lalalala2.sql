DECLARE @origen geography;
DECLARE @destino geography;
DECLARE @recorridosOrigen TABLE
(
	recorridoPuntoOrigen varchar(max),
	recorridoPuntoDestino varchar(max),
	caminarTotal int,
	caminarOrigen int,
	caminarDestino int,
	recorridoPuntoOrigenG geography,
	recorridoPuntoDestinoG geography,
	ID uniqueidentifier NOT NULL,
	TipoTransporteID uniqueidentifier NOT NULL,
	Nombre varchar(100) NULL,
	Codigo nvarchar(50) NOT NULL,
	Ramal nvarchar(100) NOT NULL,
	Ubicacion geography NOT NULL,
	DescripcionRecorrido text NULL,
	Regreso bit
)
DECLARE @recorridosDestino TABLE
(
	recorridoPuntoOrigen varchar(max),
	recorridoPuntoDestino varchar(max),
	caminarTotal int,
	caminarOrigen int,
	caminarDestino int,
	recorridoPuntoOrigenG geography,
	recorridoPuntoDestinoG geography,
	ID uniqueidentifier NOT NULL,
	TipoTransporteID uniqueidentifier NOT NULL,
	Nombre varchar(100) NULL,
	Codigo nvarchar(50) NOT NULL,
	Ramal nvarchar(100) NOT NULL,
	Ubicacion geography NOT NULL,
	DescripcionRecorrido text NULL,
	Regreso bit
)
DECLARE @thh int;
SET @origen = geography:: STGeomFromText('POINT(-58.403379 -34.593778)' , 4326); --casa
--SET @destino = geography:: STGeomFromText('POINT(-58.382764 -34.617210)' , 4326); --uade
--SET @destino = geography:: STGeomFromText('POINT(-58.424735 -34.603082)' , 4326); --autocosmos
--SET @destino = geography:: STGeomFromText('POINT(-58.479624 -34.519994)' , 4326); --club banco
--SET @destino = geography:: STGeomFromText('POINT(-58.488765 -34.545944)' , 4326); --DOT
----sin viaje directo
--SET @destino = geography:: STGeomFromText('POINT(-58.483475 -34.608910)' , 4326); --nazca y jonte
SET @destino = geography:: STGeomFromText('POINT(-58.313885 -34.704276)' , 4326); --agus house
SET @thh = 800;

INSERT INTO @recorridosOrigen (recorridoPuntoOrigen,recorridoPuntoDestino,caminarTotal,caminarOrigen,caminarDestino,recorridoPuntoOrigenG,recorridoPuntoDestinoG,ID,TipoTransporteID,Nombre,Codigo,Ramal,Ubicacion,DescripcionRecorrido,Regreso) (
		SELECT 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoOrigen',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoDestino',  
			(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) as 'caminarTotal', 
			@origen.STDistance(Ubicacion) as 'caminarOrigen',
			@destino.STDistance(Ubicacion) as 'caminarDestino', 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoOrigenG',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoDestinoG',
			* 
		FROM [GUIATBA_Transporte]
		WHERE (@origen.STDistance(Ubicacion)) < @thh
	)
INSERT INTO @recorridosDestino (recorridoPuntoOrigen,recorridoPuntoDestino,caminarTotal,caminarOrigen,caminarDestino,recorridoPuntoOrigenG,recorridoPuntoDestinoG,ID,TipoTransporteID,Nombre,Codigo,Ramal,Ubicacion,DescripcionRecorrido,Regreso) (
		SELECT 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoOrigen',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoDestino',  
			(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) as 'caminarTotal', 
			@origen.STDistance(Ubicacion) as 'caminarOrigen',
			@destino.STDistance(Ubicacion) as 'caminarDestino', 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoOrigenG',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoDestinoG',
			* 
		FROM [GUIATBA_Transporte]
		WHERE (@destino.STDistance(Ubicacion)) < @thh
	)

SELECT Nombre, * FROM @recorridosOrigen
SELECT Nombre, * FROM @recorridosDestino

SELECT * FROM @recorridosOrigen
INNER JOIN @recorridosDestino
--WHERE