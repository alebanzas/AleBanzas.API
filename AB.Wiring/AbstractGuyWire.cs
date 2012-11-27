using System.Collections.Generic;
using AB.Common.Wiring;
using Castle.Facilities.FactorySupport;
using Castle.Windsor;

namespace AB.Wiring
{
	public abstract class AbstractGuyWire : IGuyWire
	{
		protected WindsorContainer Container { get; private set; }

		protected abstract IEnumerable<IGuyWire> GuyWires { get; }

		#region IGuyWire Members

		public void Dewire()
		{
			if (Container != null)
			{
				Container.Dispose();
			}

			Container = null;
		}

		public void Wire()
		{
			if (Container != null)
			{
				Dewire();
			}

			Container = new WindsorContainer();
			Container.AddFacility<FactorySupportFacility>();

			foreach (IGuyWire guyWire in GuyWires)
			{
				guyWire.Wire();
			}
		}

		#endregion
	}
}