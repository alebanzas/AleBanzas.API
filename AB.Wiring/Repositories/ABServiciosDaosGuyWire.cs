using System;
using ABServicios.BLL.Entities;
using Castle.Windsor;

namespace AB.Wiring.Repositories
{
	public class ABServiciosDaosGuyWire : AbstractDaosGuyWire
	{
		public ABServiciosDaosGuyWire(WindsorContainer container) : base(container) {}

		public override void Wire()
        {
            RegisterEntityDao<Hotel, Guid>();
            RegisterEntityDao<RecargaSUBE, Guid>();
            RegisterEntityDao<VentaSUBE, Guid>();
            RegisterEntityDao<DolarHistorico, Guid>();

			RegisterQueries();
		}

		private void RegisterQueries()
		{
			//Container.Register(Component.For<IUltimasVersionesQuery>().ImplementedBy<UltimasVersionesQuery>());
		}
	}
}