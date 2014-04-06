using System;
using ABServicios.BLL.Entities;
using NHibernate.SqlTypes;

namespace AB.Data.UserTypes
{
	[Serializable]
	public class TipoTransporteWellKnownInstanceType : GenericWellKnownInstanceType<TipoTransporte, Guid>
	{
		private static readonly SqlType[] sqlTypes = new[] {SqlTypeFactory.Guid};
		public TipoTransporteWellKnownInstanceType() : base(TiposTransporte.Repository, (p, id) => p.ID == id, p => p.ID) { }

		public override SqlType[] SqlTypes
		{
			get { return sqlTypes; }
		}
	}
}