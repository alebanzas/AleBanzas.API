SELECT TOP 100 [ID]
      ,[nombre]
      ,[lat]
      ,[lon]
      ,[ubicacion] FROM [SUBE_Recarga] order by ubicacion.STDistance(geometry::STGeomFromText('POINT(-34.5938 -58.4036)', 4326))
      
      

