using System;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Configuration = NHibernate.Cfg.Configuration;

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