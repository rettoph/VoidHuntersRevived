using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Events;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class DestroyEntity : IEventData
    {
        public static readonly VhId NameSpace = new VhId("5674DF89-EF53-4A54-BDB6-B3D8BCBCF90D");

        public required VhId VhId { get; init; }

        public static EventDto CreateEvent(VhId vhid)
        {
            return new EventDto()
            {
                Id = NameSpace.Create(vhid),
                Data = new DestroyEntity()
                {
                    VhId = vhid
                }
            };
        }
    }
}
