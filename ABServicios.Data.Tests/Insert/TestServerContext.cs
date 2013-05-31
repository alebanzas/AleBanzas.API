using System.Collections.Generic;
using AB.Common.Wiring;
using AB.Wiring;
using AB.Wiring.DomainEvents;
using AB.Wiring.PlugIns;
using AB.Wiring.Repositories;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Insert
{
	[SetUpFixture]
	public class TestServerContext
	{
        private TestGuyWire guyWire;

		[SetUp]
		public void RunBeforeAnyTests()
		{
            guyWire = new TestGuyWire();
			guyWire.Wire();
		}

		[TearDown]
		public void RunAfterAnyTests()
		{
			guyWire.Dewire();
		}

		private class TestGuyWire : AbstractGuyWire
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
	}
}
