using System;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
using AzureDashboard.Services;

namespace AzureDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "DashBoard";
        public static string CacheControlKey = "DashBoardControl";

        public VotacionModel DefaultModel = new VotacionModel();

        public readonly AzureChristmasResultQuery query;

        public HomeController()
        {
            query = new AzureChristmasResultQuery(AzureAccount.DefaultAccount());
        }


        public ActionResult Index(string version = "1", string type = "ALL")
        {
            return View(cache.Get<VotacionModel>(CacheKey) ?? DefaultModel);
        }

        public EmptyResult FirstStart()
        {
            try
            {
                var result = GetModel();
                cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));
            }
            catch (Exception)
            {

            }
            return Start();
        }

        public EmptyResult Start()
        {
            cache.Put(CacheControlKey, new VotacionModel(), new TimeSpan(0, 2, 0), CacheItemPriority.NotRemovable,
                (key, value, reason) =>
                {
                    try
                    {
                        var result = GetModel();
                        cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        Start();
                    }
                });
            return new EmptyResult();
        }

        private VotacionModel GetModel()
        {
            return query.GetResults();
        }

    }
}