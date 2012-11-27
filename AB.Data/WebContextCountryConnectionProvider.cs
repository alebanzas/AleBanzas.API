using System;
using System.Configuration;
using System.Web;
using NHibernate.Connection;
using Environment = NHibernate.Cfg.Environment;

namespace AB.Data
{
	public class WebContextCountryConnectionProvider : DriverConnectionProvider
	{
		private string defaultConnectionStringName;
		private readonly ConnectionStringSettingsCollection contextCs = new ConnectionStringSettingsCollection();
		public override void Configure(System.Collections.Generic.IDictionary<string, string> settings)
		{
			base.Configure(settings);
			/*
			 * Ya se configuró usando el connStringName de default así que si no hay configuración por pais, o no estamos en WEB, 
			 * o no se usa mas conectionstringname usa esa conection string NH funciona igual.
			 */
			if (settings.TryGetValue(Environment.ConnectionStringName, out defaultConnectionStringName))
			{
				/*
				 * Usa el connStringName como PREFIX de los connectionStringName por pais. 
				 * De esta manera el conectionStringName configurado para NH puede tener (o no) un valor de default valido y funciona como si fuese un Environment.
				 * Por ejemplo el conectionStringName configurado para NH es "Production".
				 * La configuración levanta todas las connection string que empiezan con "Production" (case insensitive)
				 */
				foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
				{
					if (css.Name.StartsWith(defaultConnectionStringName, StringComparison.InvariantCultureIgnoreCase))
					{
						contextCs.Add(css);
					}
				}
			}
		}
		protected override string ConnectionString
		{
			get
			{
				// si no estamos en un request web usa el funcionamiento de default
				if (HttpContext.Current == null)
				{
					return base.ConnectionString;
				}
				// busca el coutry
				try
				{
					//var country = Paises.All.FirstOrDefault(x => x.MatchFor(HttpContext.Current.Request.Url));
                    return base.ConnectionString;
					//if (country == null)
					//{
					//	return base.ConnectionString;
					//}
					// crea el nuevo nombre de la connectionstring y ve si hay algo
					//string connStringName = string.Format("{0}{1}", defaultConnectionStringName, "");
					//ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connStringName];
					//if (connectionStringSettings == null)
					//{
					//	return base.ConnectionString;
					//}
					//return connectionStringSettings.ConnectionString;
				}
				catch (Exception)
				{
					// TODO: en el app start el request no está disponible
					return base.ConnectionString;
				}
			}
		}
	}
}