using System;
using ABServicios.BLL.Entities;
using Castle.Windsor;

namespace AB.Wiring.Repositories
{
	public class RoadtripDaosGuyWire : AbstractDaosGuyWire
	{
		public RoadtripDaosGuyWire(WindsorContainer container) : base(container) {}

		public override void Wire()
		{
			RegisterEntityDao<Hotel, Guid>();

			RegisterQueries();
		}

		private void RegisterQueries()
		{
			//Container.Register(Component.For<IUltimasVersionesQuery>().ImplementedBy<UltimasVersionesQuery>());
		}
	}
}