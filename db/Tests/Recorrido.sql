DECLARE @origen geography;
DECLARE @destino geography;
DECLARE @thh float;
SET @origen = geography:: STGeomFromText('POINT(-58.403379 -34.593778)' , 4326); --casa
--SET @destino = geography:: STGeomFromText('POINT(-58.382764 -34.617210)' , 4326); --uade
--SET @destino = geography:: STGeomFromText('POINT(-58.424735 -34.603082)' , 4326); --autocosmos
--SET @destino = geography:: STGeomFromText('POINT(-58.479624 -34.519994)' , 4326); --club banco
--SET @destino = geography:: STGeomFromText('POINT(-58.488765 -34.545944)' , 4326); --DOT
--SET @destino = geography:: STGeomFromText('POINT(-58.436047 -34.577044)' , 4326); --casa vane
SET @thh = 800;

SELECT
	(@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) as 'caminarTotal', 
	* 
FROM [GUIATBA_Transporte]
WHERE (@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion)) < @thh
ORDER BY (@origen.STDistance(Ubicacion) + @destino.STDistance(Ubicacion))