namespace AB.Common.Events
{
	/// <summary>
	/// Contract for DomainEvent handler.
	/// </summary>
	/// <typeparam name="T">The type of domain event.</typeparam>
	public interface IDomainEventHandler<T> where T : IDomainEvent
	{
		void Handle(T args);
	}
}