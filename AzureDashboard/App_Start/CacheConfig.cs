using AzureDashboard.Controllers;

namespace AzureDashboard
{
    public class CacheConfig
    {
        public static void RegisterItems()
        {
            (new HomeController()).FirstStart();
        }
    }
}
