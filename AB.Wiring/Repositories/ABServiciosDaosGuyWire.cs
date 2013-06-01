using System;
using AB.Data.Queries;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using Castle.MicroKernel.Registration;
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
            RegisterEntityDao<Transporte, Guid>();

			RegisterQueries();
		}

		private void RegisterQueries()
		{
            Container.Register(Component.For<IGetTransporteCercanoQuery>().ImplementedBy<GetTransporteCercanoQuery>());
		}
	}
}