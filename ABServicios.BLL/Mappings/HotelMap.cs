namespace ABServicios.Nh.Mappings
{
	public class HotelMap
	{
		public HotelMap()
		{
			Id(x => x.Id, map => map.Generator(Generators.Guid));

			Property(x => x.Nombre, map => map.Length(100));
            Property(x => x.Descripcion, map => map.Length(1000));

            Property(x => x.Telefono, map => map.Length(100));
            Property(x => x.Direccion, map => map.Length(100));
			Property(x => x.Barrio, map => map.Length(100));
            Property(x => x.Ciudad, map => map.Length(100));
            Property(x => x.Provincia, map => map.Length(100));

            Property(x => x.Sitio, map => map.Length(200));

            Property(x => x.Id1, map => map.Length(200));
            Property(x => x.Id2, map => map.Length(200));
            Property(x => x.Id3, map => map.Length(200));
		}
	}
}