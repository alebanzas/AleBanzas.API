using System;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
using AzureContest.Web.Services;

namespace AzureContest.Web.Controllers
{
    public class MspController : Controller
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "DashBoardMsp";
        public static string CacheControlKey = "DashBoardControlMsp";

        public VotacionModel DefaultModel = new VotacionModel();

        public readonly AzureChristmasReferalResultQuery query;

        public MspController()
        {
            query = new AzureChristmasReferalResultQuery(AzureAccount.DefaultAccount());
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