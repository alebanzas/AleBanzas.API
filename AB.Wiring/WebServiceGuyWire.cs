using System.Collections.Generic;
using AB.Common.Wiring;
using AB.Wiring.DomainEvents;
using AB.Wiring.Repositories;

namespace AB.Wiring
{
    public class WebServiceGuyWire : AbstractGuyWire
	{
		protected override IEnumerable<IGuyWire> GuyWires
		{
			get
			{
				yield return new ServiceLocatorGuyWire(Container);
				yield return new NhSessionManagementGuyWire(Container);
				yield return new DaosGuyWire(Container);
				yield return new ApplicationServicesGuyWire(Container);
				yield return new DomainEventsGuyWire(Container);
			}
		}
	}
}