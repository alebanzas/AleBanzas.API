using AB.Common.Wiring;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;

namespace AB.Wiring
{
	public class ServiceLocatorGuyWire: IGuyWire
	{
		private readonly WindsorContainer container;

		public ServiceLocatorGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public void Wire()
		{
			var wServiceLocator = new WindsorServiceLocator(container);
			container.Register(Component.For<IServiceLocator>().Instance(wServiceLocator));
			ServiceLocator.SetLocatorProvider(() => container.Resolve<IServiceLocator>());
		}

		public void Dewire()
		{
			throw new System.NotSupportedException("Child GuyWire does not support dewire.");
		}
	}
}