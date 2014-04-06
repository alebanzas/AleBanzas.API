using System;
using AB.Common.Wiring;
using Castle.Windsor;

namespace AB.Wiring
{
	public class ApplicationServicesGuyWire : IGuyWire
	{
		private readonly WindsorContainer container;

		public ApplicationServicesGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		#region IGuyWire Members

		public void Wire()
		{
			//container.Register(Component.For<IStaticContentDao>().ImplementedBy<OnAzureStaticContentDao>());
		}

		public void Dewire()
		{
			throw new NotSupportedException("Child GuyWire does not support dewire.");
		}

		#endregion
	}
}