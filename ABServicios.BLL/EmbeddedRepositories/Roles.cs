using System.Collections.Generic;
using System.Collections.ObjectModel;
using ABServicios.BLL.Entities;

namespace ABServicios.BLL.EmbeddedRepositories
{
	public class Roles : ReadOnlyCollection<Rol>
	{
		private static readonly HashSet<Rol> repository;

		public static readonly Rol Admin = new Rol { ID = 4, Nombre = "Admin", Descripcion = "Admin" };

		static Roles()
		{
			repository = new HashSet<Rol> { Admin };
		}

		public Roles(IList<Rol> list) : base(list)
		{
		}

		public static IEnumerable<Rol> Repository
		{
			get { return repository; }
		}
 
	}
}