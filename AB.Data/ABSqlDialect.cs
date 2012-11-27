using NHibernate;
using NHibernate.Dialect.Function;

namespace AB.Data
{
	public class ABSqlDialect : NHibernate.Dialect.MsSql2008Dialect
	{
		public ABSqlDialect()
		{
			RegisterFunction("NewGuid", new NoArgSQLFunction("NEWID", NHibernateUtil.Guid, true));
		}
	}
}