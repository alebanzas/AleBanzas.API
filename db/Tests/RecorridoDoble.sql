/*
DEVUELVE LAS COMBINACIONES DE TRANSPORTE PARA IR DE UN PUNTO A OTRO
NOTA!: SOLO USAR PARA CUANDO RECORRIDOS SIMPLES NO DA RESULTADO, CUANDO ES RECORRIDO SIMPLE, TARDA MUCHO
*/

DECLARE @origen geography;
DECLARE @destino geography;
DECLARE @thh float;
SET @origen = geography:: STGeomFromText('POINT(-58.403379 -34.593778)' , 4326); --casa
----sin viaje directo
SET @destino = geography:: STGeomFromText('POINT(-58.483475 -34.608910)' , 4326); --nazca y jonte
--SET @destino = geography:: STGeomFromText('POINT(-58.313885 -34.704276)' , 4326); --agus house
SET @thh = 800;

SELECT DISTINCT
ro.Codigo as 'CodigoO'
,rd.Codigo as 'CodigoD'
,MIN(ro.caminarOrigen + rd.caminarDestino) as 'caminarTotal'
FROM
		(
		SELECT 
			@origen.STDistance(Ubicacion) as 'caminarOrigen',
			Codigo
		FROM [GUIATBA_Transporte]
		WHERE (@origen.STDistance(Ubicacion)) < @thh
		) ro

INNER JOIN GUIATBA_TransporteIntersecciones ti ON ro.Codigo = ti.CodigoO
INNER JOIN 
		(
		SELECT 
			@destino.STDistance(Ubicacion) as 'caminarDestino', 
			Codigo
		FROM [GUIATBA_Transporte]
		WHERE (@destino.STDistance(Ubicacion)) < @thh
		) rd

ON rd.Codigo = ti.CodigoD

WHERE (ro.caminarOrigen + rd.caminarDestino) < @thh

GROUP BY ro.Codigo, rd.Codigo
--HAVING (ro.caminarOrigen + rd.caminarDestino) < @thh
ORDER BY caminarTotal