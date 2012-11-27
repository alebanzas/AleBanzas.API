using System;

namespace AB.Common.Events
{
	public class DomainEvents
	{
		private static IDomainEventHandlersStore HandlersStore { get; set; }

		public static void Initialize(IDomainEventHandlersStore handlersStore)
		{
			if (handlersStore == null)
			{
				throw new ArgumentNullException("handlersStore");
			}
			HandlersStore = handlersStore;
		}

		public static void Raise<T>(T args) where T : IDomainEvent
		{
			if (HandlersStore == null)
			{
				return;
			}
			foreach (var handler in HandlersStore.GetHandlersOf<T>())
			{
				handler.Handle(args);
			}
		}
	}
}