using System;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ABServicios.Data.Tests.SchemaUtils
{
	public class SchemaUpdateUtil
	{
		[Test]
 		public void SchemaUpdate()
		{
			var cfg = new Configuration().Configure();
			//cfg.CreateIndexesForForeignKeys();

			var sb = new StringBuilder();
			new SchemaUpdate(cfg).Execute(s=> sb.Append(s), false);
			Console.WriteLine(sb.ToString());
		}
	}
}