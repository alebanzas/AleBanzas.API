using System.IO;
using AB.Common.Wiring;
using AB.Wiring;
using AB.Wiring.Repositories;
using Castle.Facilities.FactorySupport;
using Castle.Windsor;
using log4net.Config;
using NUnit.Framework;

namespace ABServicios.Data.Tests.AplicationServices
{
	[SetUpFixture]
	public class ReadOnlyTestsContext
	{
		private WindsorContainer container;

		[SetUp]
		public void RunBeforeAnyTests()
		{
			// Inicializa logger
			XmlConfigurator.Configure(new FileInfo("log4net.configuration.xml"));
			container = new WindsorContainer();
			(new ReadonlyTestsGuyWire(container)).Wire();
			//CurrentSessionContext.Wrapper = new uNhAddIns.CastleAdapters.SessionWrapper();
		}

		[TearDown]
		public void RunAfterAnyTests()
		{
			container.Dispose();
			container = null;
		}

		public class ReadonlyTestsGuyWire: IGuyWire
		{
			private readonly WindsorContainer container;

			public ReadonlyTestsGuyWire(WindsorContainer container)
			{
				this.container = container;
			}

			public void Wire()
			{
				container.AddFacility<FactorySupportFacility>();

				(new ServiceLocatorGuyWire(container)).Wire();
				(new NhSessionManagementGuyWire(container)).Wire();
				(new DaosGuyWire(container)).Wire();
			}

			public void Dewire()
			{
			}
		}
	}
}