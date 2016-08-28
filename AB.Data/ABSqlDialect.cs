using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Spatial.Dialect;

namespace AB.Data
{
    //NHibernate.Spatial.Dialect.MsSql2008GeometryDialect, NHibernate.Spatial.MsSql2008
    public class ABSqlDialect : MsSql2012GeographyDialect
	{
		public ABSqlDialect()
		{
			RegisterFunction("NewGuid", new NoArgSQLFunction("NEWID", NHibernateUtil.Guid, true));
		}
	}
}