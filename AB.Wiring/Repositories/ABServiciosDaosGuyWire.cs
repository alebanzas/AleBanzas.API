using System;
using AB.Data;
using AB.Data.Queries;
using ABServicios.BLL.DataInterfaces;
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
            RegisterEntityDao<DreamSparkItem, Guid>();
            RegisterEntityDao<CandyApp, Guid>();
            RegisterEntityDao<InetForm, Guid>();
            RegisterEntityDao<TeamMember, Guid>();

            RegisterInMemoryRepositories();
			RegisterQueries();
		}

		private void RegisterQueries()
        {
            Container.Register(Component.For<IGetTransporteCercanoQuery>().ImplementedBy<GetTransporteCercanoQuery>());
            Container.Register(Component.For<IGetSUBECercanoQuery>().ImplementedBy<GetSUBECercanoQuery>());
		}

        protected virtual void RegisterInMemoryRepositories()
        {
            Container.Register(Component.For<IRepository<Application>>().ImplementedBy<InMemoryApplicationsRepository>());
        }
	}
}