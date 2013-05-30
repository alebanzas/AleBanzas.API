using System;
using ABServicios.BLL.EmbeddedRepositories;
using ABServicios.BLL.Entities;
using NHibernate.SqlTypes;

namespace AB.Data.UserTypes
{
	[Serializable]
	public class RolWellKnownInstanceType : GenericWellKnownInstanceType<Rol, int>
	{
		private static readonly SqlType[] sqlTypes = new[] {SqlTypeFactory.Int32};
		public RolWellKnownInstanceType() : base(Roles.Repository, (p, id) => p.ID == id, p => p.ID) { }

		public override SqlType[] SqlTypes
		{
			get { return sqlTypes; }
		}
	}
}