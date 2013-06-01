using ABServicios.BLL.DataInterfaces.Queries;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NetTopologySuite.Geometries;
using SharpTestsEx;

namespace ABServicios.Data.Tests.AplicationServices.Queries
{
    public class GetTransporteCercanoQueryTests
    {
        [Test]
        public void WhenModeloLessThanZeroThenShouldBeEmpty()
        {
            using (new PersistenceRequest())
            {
                var query = ServiceLocator.Current.GetInstance<IGetTransporteCercanoQuery>();

                var result = query.GetMasCercanos(new Point(-58.436047, -34.577044), 800);
                result.Should().Be.Empty();
            }
        }
    }
}
