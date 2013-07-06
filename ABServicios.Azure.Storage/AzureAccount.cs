using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ABServicios.Azure.Storage
{
	public static class AzureAccount
	{
		private const string DefaultDataconnectionstring = "ABSConnectionString";

		static AzureAccount()
		{
			CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
			{
				string value = ConfigurationManager.AppSettings[configName];
				if (RoleEnvironment.IsAvailable)
				{
					value = RoleEnvironment.GetConfigurationSettingValue(configName);
				}
				
				configSetter(value);
			});
		}

		public static CloudStorageAccount DefaultAccount()
		{
			return CloudStorageAccount.FromConfigurationSetting(DefaultDataconnectionstring);
		}
	}
}