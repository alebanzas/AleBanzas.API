using System.Collections.Generic;
using AB.Common.Wiring;
using AB.Wiring;
using AB.Wiring.DomainEvents;
using AB.Wiring.PlugIns;
using AB.Wiring.Repositories;
using NUnit.Framework;

namespace ABServicios.Data.Tests
{
	[SetUpFixture]
	public class TestServerContext
	{
		private GuyWire guyWire;

		[SetUp]
		public void RunBeforeAnyTests()
		{
			guyWire = new GuyWire();
			guyWire.Wire();
		}

		[TearDown]
		public void RunAfterAnyTests()
		{
			guyWire.Dewire();
		}

		private class GuyWire : AbstractGuyWire
		{
			protected override IEnumerable<IGuyWire> GuyWires
			{
				get
				{
					yield return new ServiceLocatorGuyWire(Container);
					yield return new NhSessionManagementGuyWire(Container);
					yield return new ApplicationServicesGuyWire(Container);
					yield return new DomainEventsGuyWire(Container);
					yield return new ServiciosPlugInsGuyWire(Container);
				}
			}
		}
	}
}
