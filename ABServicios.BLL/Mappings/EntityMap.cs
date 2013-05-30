using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using ABServicios.BLL.Entities;

namespace ABServicios.Nh.Mappings
{
	public class EntityMap<T> : ClassMapping<T> where T : BaseEntity
	{
		public EntityMap()
		{
			Id(x => x.Id, map => map.Generator(Generators.Guid));
		}
	}
}