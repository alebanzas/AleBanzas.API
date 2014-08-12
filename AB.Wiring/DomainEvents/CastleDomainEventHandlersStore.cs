using System.Collections.Generic;
using AB.Common.Events;
using Castle.Windsor;

namespace AB.Wiring.DomainEvents
{
	public class CastleDomainEventHandlersStore : IDomainEventHandlersStore
	{
		private readonly WindsorContainer container;

		public CastleDomainEventHandlersStore(WindsorContainer container)
		{
			this.container = container;
		}

		public IEnumerable<IDomainEventHandler<T>> GetHandlersOf<T>() where T : IDomainEvent
		{
			return container.ResolveAll<IDomainEventHandler<T>>();
		}
	}
}