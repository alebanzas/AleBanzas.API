using System;

namespace ABServicios.BLL.Entities
{
	public interface IEntity<TIdentity> : IEquatable<IEntity<TIdentity>>
	{
		TIdentity Id { get; }
	}
}