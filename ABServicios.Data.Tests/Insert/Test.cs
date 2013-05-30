using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AB.Wiring;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using GisSharpBlog.NetTopologySuite.Geometries;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Insert
{
    public class Tests
    {
        [Test]
        public void AddRepo()
        {
            var ventaSUBE = new VentaSUBE
            {
                ID = Guid.NewGuid(),
                Ubicacion = new Point(DateTime.Now.Minute, DateTime.Now.Second),
                Nombre = DateTime.UtcNow.ToShortTimeString(),
            };
            var ventaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<VentaSUBE>>();
            var sessionFactory = ServiceLocator.Current.GetInstance<ISessionFactory>();
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                ventaSUBERepo.Add(ventaSUBE);

                tx.Commit();
            }
        }
    }
}