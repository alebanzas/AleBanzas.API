using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
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
            IEnumerable<IGrouping<string, AzureChristmasVoteLogData>> result = query.GetResults();

            var rr = new VotacionModel();

            foreach (IGrouping<string, AzureChristmasVoteLogData> item in result.OrderByDescending(x => x.Count()))
            {
                if (item.Key.ToLowerInvariant().Contains("google")) continue;

                int count = item.Distinct(new AzureChristmasVoteLogDataComparer()).Count();
                
                rr.Lista.Add(new VotacionItem
                {
                    Nombre = item.Key,
                    Count = count,
                });
            }

            rr.Lista = rr.Lista.OrderByDescending(x => x.Count).ToList();

            return rr;
        }

    }

    public class AzureChristmasVoteLogDataComparer : IEqualityComparer<AzureChristmasVoteLogData>
    {
        // Products are equal if their names and log numbers are equal. 
        public bool Equals(AzureChristmasVoteLogData x, AzureChristmasVoteLogData y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.Ip == y.Ip && x.Date.Date == y.Date.Date;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(AzureChristmasVoteLogData log)
        {
            return log.Date.Date.GetHashCode() + log.Ip.GetHashCode();
        }

    }

    public class VotacionModel
    {
        public VotacionModel()
        {
            Lista = new List<VotacionItem>();
        }
        public List<VotacionItem> Lista { get; set; }
    }

    public class VotacionItem
    {
        public string Nombre { get; set; }
        public int Count { get; set; }
    }
}