using System;
using System.Configuration;
using System.Linq;

namespace AB.Common.Wiring
{
	public static class ApplicationConfiguration
	{
		private const string GuyWireConfKey = "guywire";
		private const string GuyWireConfMessage =
			@"La implementación de la clase que cablea toda la applicación no fue configurada.
Ejemplo
	<appSettings>
		<add key='GuyWire' value='ABServicios.Wiring.GuyWire, ABServicios.Wiring.Castle'/>
	</appSettings>";

		public static IGuyWire GetGuyWire()
		{
			var guyWireClassKey =
				ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => GuyWireConfKey.Equals(k.ToLowerInvariant()));
			if (string.IsNullOrEmpty(guyWireClassKey))
			{
				throw new ApplicationException(GuyWireConfMessage);
			}
			var guyWireClass = ConfigurationManager.AppSettings[guyWireClassKey];
			var type = Type.GetType(guyWireClass);
			try
			{
				return (IGuyWire)Activator.CreateInstance(type);
			}
			catch (MissingMethodException ex)
			{
				throw new ApplicationException("Public constructor was not found for " + type, ex);
			}
			catch (InvalidCastException ex)
			{
				throw new ApplicationException(type + "Type does not implement " + typeof(IGuyWire), ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Unable to instantiate: " + type, ex);
			}
		}
	}
}