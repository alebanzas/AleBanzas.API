using System;
using System.Collections.Generic;
using AB.Common.Wiring;
using AB.Data;
using AB.Data.Cache;
using AB.Wiring;
using AB.Wiring.DomainEvents;
using AB.Wiring.PlugIns;
using AB.Wiring.Repositories;
using ABServicios.BLL.Entities;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.Caches.SysCache;
using NHibernate.Cfg;
using Environment = NHibernate.Cfg.Environment;

namespace ABServicios
{
    public class MvcGuyWire : AbstractGuyWire
    {
        protected override IEnumerable<IGuyWire> GuyWires
        {
            get
            {
                yield return new ServiceLocatorGuyWire(Container);
                yield return new NhibernateGuyWire(Container);
                yield return new DaosGuyWire(Container);
                yield return new DomainEventsGuyWire(Container);
            }
        }
    }

    public class NhibernateGuyWire : IGuyWire
    {
        private readonly WindsorContainer container;

        public NhibernateGuyWire(WindsorContainer container)
        {
            this.container = container;
        }

        #region IGuyWire Members

        public void Wire()
        {
            //#if DEBUG
            //            var confProvider = new SerializedSessionFactoryConfigurationProvider();
            //            confProvider.AfterConfigure += confProvider_AfterConfigure;
            //            Configuration conf = confProvider.Configure().First();
            //#else
            var conf = new Configuration();
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
            //conf.QueryCache().ResolveRegion("SearchStatistic").Using<TolerantQueryCache>().AlwaysTolerant();
            //conf.AddResource("Mapping.CustomTypes.xml", typeof(Hotel).Assembly);
            conf.AddAssembly(typeof(RecargaSUBE).Assembly);
            conf.SetProperty(Environment.SqlExceptionConverter, typeof(MsSqlExceptionConverter).AssemblyQualifiedName);
            conf.Configure();
            //#endif
            container.Register(Component.For<ISessionFactory>().UsingFactoryMethod(() => conf.BuildSessionFactory()));
        }


        public void Dewire()
        {
            throw new NotSupportedException("Child GuyWire does not support dewire.");
        }

        #endregion
    }
}