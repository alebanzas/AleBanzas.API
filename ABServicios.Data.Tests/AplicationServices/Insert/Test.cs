using System;
using System.Linq;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Insert
{
    public class Tests
    {
        [Test]
        public void AddRepo()
        {
            /*var ventaSUBE = new VentaSUBE
            {
                ID = Guid.NewGuid(),
                Ubicacion = new Point(DateTime.Now.Minute, DateTime.Now.Second),
                Nombre = DateTime.UtcNow.ToShortTimeString(),
            };*/
            var ventaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<VentaSUBE>>();
            var sessionFactory = ServiceLocator.Current.GetInstance<ISessionFactory>();
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var ventaSube = ventaSUBERepo.ToList();



                tx.Commit();
            }
        }
    }
}