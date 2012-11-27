using AB.Common.Wiring;
using AB.Data;
using AB.Data.Cache;
using ABServicios.BLL.Entities;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.Caches.SysCache;
using NHibernate.Cfg;

namespace AB.Wiring.Repositories
{
	public class NhSessionManagementGuyWire : IGuyWire
	{
		public const string SessionFactoryProviderComponentKey = "sessionFactoryProvider";

		private readonly WindsorContainer container;

		public NhSessionManagementGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		#region IGuyWire Members

		public void Wire()
		{
            var conf = new Configuration();
            conf.Configure();
            conf.DataBaseIntegration(x =>
            {
                x.Dialect<ABSqlDialect>();
                x.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                x.Timeout = 60;
                x.BatchSize = 100;
            });
            conf.Cache(x =>
            {
                x.Provider<SysCacheProvider>();
                x.UseQueryCache = true;
                x.DefaultExpiration = 120;
            });
						conf.QueryCache().ResolveRegion("SearchStatistics").Using<TolerantQueryCache>().AlwaysTolerant();
						conf.AddResource("RoadTrip.BLL.Mapping.CustomTypes.xml",
                             typeof(Hotel).Assembly);
                        conf.AddAssembly(typeof(Hotel).Assembly);

						container.Register(Component.For<ISessionFactory>().UsingFactoryMethod(() => conf.BuildSessionFactory()));
		}

		public void Dewire()
		{
			throw new System.NotSupportedException("Child GuyWire does not support dewire.");
		}

		#endregion
	}
}