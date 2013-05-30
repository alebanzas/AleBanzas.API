INSERT INTO GUIATBA_TransporteIntersecciones (CodigoO, CodigoD)
SELECT DISTINCT o.Codigo, d.Codigo
FROM [GUIATBA_Transporte] o
INNER JOIN (
SELECT *
FROM [GUIATBA_Transporte]
) d
ON o.Ubicacion.STBuffer(0.001).STIntersects(d.Ubicacion.STBuffer(0.001)) = 1


--sin distinct: >400k |tiempo: >13min

--final con distinct: 12964 |tiempo: 20:27min


SELECT DISTINCT o.Codigo, d.Codigo, o.Ubicacion.STIntersection(d.Ubicacion.STBuffer(0.001)).STPointN(1).STAsText()
FROM [GUIATBA_Transporte] o
INNER JOIN (
SELECT *
FROM [GUIATBA_Transporte]
) d
ON o.Ubicacion.STBuffer(0.001).STIntersects(d.Ubicacion.STBuffer(0.001)) = 1