using System;
using AB.Common.Wiring;
using Castle.Windsor;

namespace AB.Wiring.PlugIns
{
	public class ServiciosPlugInsGuyWire: IGuyWire
	{
		private readonly WindsorContainer container;

		public ServiciosPlugInsGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public void Wire()
		{
			//container.Register(AllTypes.FromAssemblyContaining<IServicioPlugIn>().BasedOn(typeof(IServicioPlugIn)).WithService.FromInterface());
			
			// Hasta que no usemos la ultima versión de Castle la siguientes lineas tienen que ser las ultimas de la registraccion de plug-ins
			//ServiciosPlugInsService plugInsService = new ServiciosPlugInsService(container.ResolveAll<IServicioPlugIn>()); 
			//container.Register(Component.For<ServiciosPlugInsService>().Instance(plugInsService));
		}

		public void Dewire()
		{
			throw new NotSupportedException("Child GuyWire does not support dewire.");
		}
	}
}