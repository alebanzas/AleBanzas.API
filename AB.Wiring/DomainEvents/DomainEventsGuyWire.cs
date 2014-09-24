using System;
using System.ComponentModel;
using AB.Common.Events;
using AB.Common.Wiring;
using Castle.Windsor;

namespace AB.Wiring.DomainEvents
{
	public class DomainEventsGuyWire: IGuyWire
	{
		private readonly WindsorContainer container;

		public DomainEventsGuyWire(WindsorContainer container)
		{
			this.container = container;
		}

		public void Wire()
		{
			//var castleDomainEventHandlersStore = new CastleDomainEventHandlersStore(container);
			//container.Register(Component.For<IDomainEventHandlersStore>().Instance(castleDomainEventHandlersStore));
			//ABServicios.Common.Events.DomainEvents.Initialize(castleDomainEventHandlersStore);
			//container.Register(AllTypes.FromAssemblyContaining<UserSignInEvent>().BasedOn(typeof(IDomainEventHandler<>)).WithService.FromInterface());
		}

		public void Dewire()
		{
			throw new NotSupportedException("Child GuyWire does not support dewire.");
		}
	}
}