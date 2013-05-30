using NHibernate;
using NHibernate.Dialect.Function;

namespace AB.Data
{
    //NHibernate.Spatial.Dialect.MsSql2008GeometryDialect, NHibernate.Spatial.MsSql2008
    public class ABSqlDialect : NHibernate.Spatial.Dialect.MsSql2008GeometryDialect
	{
		public ABSqlDialect()
		{
			RegisterFunction("NewGuid", new NoArgSQLFunction("NEWID", NHibernateUtil.Guid, true));
		}
	}
}