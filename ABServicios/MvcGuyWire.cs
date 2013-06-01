using System.Collections.Generic;
using AB.Common.Wiring;
using AB.Wiring;
using AB.Wiring.DomainEvents;
using AB.Wiring.Repositories;

namespace ABServicios
{
    public class MvcGuyWire : AbstractGuyWire
    {
        protected override IEnumerable<IGuyWire> GuyWires
        {
            get
            {
                yield return new ServiceLocatorGuyWire(Container);
                yield return new NhSessionManagementGuyWire(Container);
                yield return new DaosGuyWire(Container);
                yield return new DomainEventsGuyWire(Container);
            }
        }
    }
}