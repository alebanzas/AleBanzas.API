CREATE TABLE [dbo].[milano_str](
[id] [int] NOT NULL,
[name] [nvarchar](100) NULL,
[type] [nvarchar](50) NULL,
[lat] FLOAT NULL,
[lon] FLOAT NULL,
[g] GEOGRAPHY	
CONSTRAINT [PK_milano_str] PRIMARY KEY CLUSTERED 
([id] ASC)) 
ON [PRIMARY];

CREATE SPATIAL INDEX [geo_sidx] ON [dbo].[milano_str] 
([g]) USING GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]; 

/* 
add SRID constraint to ensure use of "WGS 84" spatial reference system 
*/ 

ALTER TABLE [dbo].[milano_str] WITH CHECK 
ADD CONSTRAINT [enforce_srid_geography_milano_str] 
CHECK ([g].[STSrid]=4326);

ALTER TABLE [dbo].[milano_str] CHECK CONSTRAINT [enforce_srid_geography_milano_str];


CREATE TABLE milan_street_src (
street_id INT, 
node_no INT, 
name Varchar (60),
lat Varchar (60),
lon Varchar (60));


INSERT INTO [milan_street_src] VALUES (8,0,'Via Dante','9.182470116','45.468029409')
INSERT INTO [milan_street_src] VALUES (8,1,'Via Dante','9.183925755','45.466958431')
INSERT INTO [milan_street_src] VALUES (8,2,'Via Dante','9.185194564','45.465925561')
INSERT INTO [milan_street_src] VALUES (8,3,'Via Dante','9.186244976','45.465157677')
INSERT INTO [milan_street_src] VALUES (8,4,'Via Dante','9.187238352','45.464538579')
INSERT INTO [milan_street_src] VALUES (8,5,'Via Dante','9.188393029','45.46380327')




select lat + ' ' + lon + ', ' from dbo.milan_street_src 
where street_id = 8 
order by street_id, node_no; 


INSERT INTO milano_str ([id],[name],[type],[g])
SELECT 1, 'prueba', 'A',
'LINESTRING (
9.182470116 45.468029409, 
9.183925755 45.466958431, 
9.185194564 45.465925561, 
9.186244976 45.465157677, 
9.187238352 45.464538579, 
9.188393029 45.46380327
)'
q
select * from [GUIATBA_Transporte]

select barrio, COUNT(*) as a from TELO_Hotel
group by barrio order by a


select ubicacion.STAsText(), * from TELO_Hotel
where barrio = 'Monserrat' order by direccion





select Ubicacion.STBuffer(40), Ubicacion.STNumPoints(), Ubicacion.STPointN(2).STAsText(),* from [GUIATBA_Transporte]
--where Codigo in ('152', '41')
where tipotransporteid = 'E74E932C-BF15-4ED6-ADA6-F0CBF0688B78'
where tipotransporteid = '440C21D3-71DE-4C94-849D-66139EADCE4C'

DECLARE @g geography;
DECLARE @h geography;
SET @g = (select Ubicacion from [GUIATBA_Transporte] where ID = '5F4F84E9-8291-4C6D-BE6B-EFBF37C32E20')
SET @h = (select Ubicacion from [GUIATBA_Transporte] where ID = '91C580B7-7DC6-4554-9767-BE089942F02E')
SELECT @g.STIntersects(@h);




DECLARE @gps geography;
DECLARE @th int;
SET @gps = geography:: STGeomFromText('POINT(-58.403379 -34.593778)' , 4326);
SET @th = 200;
SELECT @gps.STDistance(Ubicacion) AS 'distancia', * FROM [GUIATBA_Transporte]
WHERE @gps.STDistance(Ubicacion) < @th
ORDER BY distancia




select * from [GUIATBA_TipoTransporte]

--truncate table [GUIATBA_Transporte]




DECLARE @pnt GEOGRAPHY 
DECLARE @line GEOGRAPHY; 

-- select spatial objects
select @pnt = g from dbo.milano_pnt where id=75;
select @line = g from dbo.milano_str where id=1;


-- show map with shortest line and intersection 
select id, name, g.STBuffer(8) AS geog
from dbo.milano_str
where id=1
 	union all
select id, name, g.STBuffer(40) AS geog
from dbo.milano_pnt
where id=75
 	union all
select 0 AS id, '' AS name, 
@pnt.ShortestLineTo(@line).STBuffer(3) AS geog
 	union all
select 0 AS id, '' AS name, 
@pnt.ShortestLineTo(@line).STIntersection(@line).STBuffer(15) AS geog





DECLARE @line1 GEOMETRY = geometry::STGeomFromText('LINESTRING ( 
-58.462521 -34.555469, 
-58.456943 -34.561777, 
-58.454196 -34.56494, 
-58.45147 -34.566549, 
-58.437072 -34.574094, 
-58.43173 -34.576638, 
-58.427481 -34.577891, 
-58.425635 -34.578546, 
-58.423876 -34.579624, 
-58.4101 -34.589109, 
-58.407353 -34.591529, 
-58.405358 -34.59319, 
-58.403748 -34.594003, 
-58.401946 -34.594551, 
-58.401411 -34.594921, 
-58.401217 -34.595452, 
-58.400959 -34.596865, 
-58.400702 -34.598171, 
-58.399886 -34.599089, 
-58.398105 -34.599708, 
-58.393064 -34.599584, 
-58.389243 -34.599372, 
-58.387805 -34.599513, 
-58.38684 -34.599938, 
-58.385939 -34.600645, 
-58.385167 -34.601421, 
-58.383707 -34.60257, 
-58.380983 -34.603965, 
-58.377228 -34.606067, 
-58.373579 -34.60786
 )', 0)
DECLARE @line2 GEOMETRY = geometry::STGeomFromText('POINT(-58.403379 -34.593778)', 0)

SELECT @line1.STBuffer(0.001)

SELECT  @line1.ShortestLineTo(@line2).STIntersection(@line2).ToString()


DECLARE @g geography;
DECLARE @h geography;
--SET @g = (select Ubicacion from [GUIATBA_Transporte] where ID = '6F8C787A-94DB-4230-A875-67A885E48292') --C
SET @h = (select Ubicacion from [GUIATBA_Transporte] where ID = '6FE5569A-9693-4553-A777-A81932D971EF') --D
SET @g = (select Ubicacion from [GUIATBA_Transporte] where ID = '87F36C47-9635-49C0-8203-937A76B5A49C') --A
SELECT @g.STBuffer(40).STIntersects(@h.STBuffer(40)),
@g.STBuffer(40).ShortestLineTo(@h.STBuffer(40)).STAsText()






SELECT @line1.STBuffer(0.001)

------------------------
------STIntersection para combinacion



DECLARE @origen geography;
DECLARE @destino geography;
DECLARE @thh int;
SET @origen = geography:: STGeomFromText('POINT(-58.403379 -34.593778)' , 4326); --casa
SET @destino = geography:: STGeomFromText('POINT(-58.382764 -34.617210)' , 4326); --uade
--SET @destino = geography:: STGeomFromText('POINT(-58.424735 -34.603082)' , 4326); --autocosmos
--SET @destino = geography:: STGeomFromText('POINT(-58.479624 -34.519994)' , 4326); --club banco
--SET @destino = geography:: STGeomFromText('POINT(-58.488765 -34.545944)' , 4326); --DOT
----sin viaje directo
--SET @destino = geography:: STGeomFromText('POINT(-58.483475 -34.608910)' , 4326); --nazca y jonte
--SET @destino = geography:: STGeomFromText('POINT(-58.313885 -34.704276)' , 4326); --agus house
SET @thh = 100;

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
WHERE 
(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) < @thh
ORDER BY 
(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)),
Nombre




----------------------------------------------------------------------------------
----------------------------------------------------------------------------------
----------------------------------------------------------------------------------
----------------------------------------------------------------------------------
DECLARE @origen geography;
DECLARE @destino geography;
DECLARE @subtable TABLE
(
	recorridoPuntoOrigen varchar(max),
	recorridoPuntoDestino varchar(max),
	caminarTotal int,
	caminarOrigen int,
	caminarDestino int,
	recorridoPuntoOrigenG geography,
	recorridoPuntoDestinoG geography,
	destino int,
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

INSERT INTO @subtable (recorridoPuntoOrigen,recorridoPuntoDestino,caminarTotal,caminarOrigen,caminarDestino,recorridoPuntoOrigenG,recorridoPuntoDestinoG,destino,ID,TipoTransporteID,Nombre,Codigo,Ramal,Ubicacion,DescripcionRecorrido,Regreso) (
	(
		SELECT 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoOrigen',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoDestino',  
			(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) as 'caminarTotal', 
			@origen.STDistance(Ubicacion) as 'caminarOrigen',
			@destino.STDistance(Ubicacion) as 'caminarDestino', 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoOrigenG',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoDestinoG',
			0 as 'destino',
			* 
		FROM [GUIATBA_Transporte]
		WHERE (@origen.STDistance(Ubicacion)) < @thh
	)
	UNION ALL
	(
		SELECT 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoOrigen',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1).STAsText() as 'recorridoPuntoDestino',  
			(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) as 'caminarTotal', 
			@origen.STDistance(Ubicacion) as 'caminarOrigen',
			@destino.STDistance(Ubicacion) as 'caminarDestino', 
			@origen.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoOrigenG',
			@destino.ShortestLineTo(Ubicacion).STBuffer(1).STIntersection(Ubicacion).STPointN(1) as 'recorridoPuntoDestinoG',
			1 as 'destino',
			* 
		FROM [GUIATBA_Transporte]
		WHERE (@destino.STDistance(Ubicacion)) < @thh
	)
)

SELECT * FROM @subtable d WHERE destino = '1'

SELECT * FROM @subtable o WHERE destino = '0'

ORDER BY
caminarOrigen,
caminarDestino,
Nombre