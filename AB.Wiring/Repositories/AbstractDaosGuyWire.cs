using AB.Common.Wiring;
using AB.Data;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AB.Wiring.Repositories
{
	public abstract class AbstractDaosGuyWire : IGuyWire
	{
		private readonly WindsorContainer container;

		protected AbstractDaosGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public WindsorContainer Container
		{
			get { return container; }
		}

		public abstract void Wire();

		public void Dewire()
		{
			throw new System.NotSupportedException("Child GuyWire does not support dewire.");
		}

		protected void RegisterEntityDao<T, TEntityDao, TImplementation>()
			where T : AbstractEntity<int>
			where TEntityDao : class, IABDao<T>
			where TImplementation : TEntityDao
		{
			container.Register(Component.For<TEntityDao, IABDao<T>, IEntityDao<T, int>, ICrudDao<T, int>, IDao<T, int>>().ImplementedBy<TImplementation>());
			container.Register(Component.For<IRepository<T>>().ImplementedBy<Repository<T>>());
		}

        protected void RegisterEntityDao<T, U>() where T : AbstractEntity<U>
        {
            container.Register(Component.For<IABDao<T>, IEntityDao<T, U>, ICrudDao<T, U>, IDao<T, U>>().ImplementedBy<ABDao<T>>());
            container.Register(Component.For<IRepository<T>>().ImplementedBy<Repository<T>>());
        }
	}
}