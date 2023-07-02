using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class CreateEntity : IEventData
    {
        public static readonly VhId NameSpace = new VhId("6a7a5aed-2cfe-4c85-8c24-421a82f1d738");

        public required VhId VhId { get; init; }
        public required EntityType Type { get; init; }
        public required EntityInitializerDelegate? Initializer { get; init; }

        public static EventDto CreateEvent(EntityType type, VhId vhid, EntityInitializerDelegate? initializer = null)
        {
            return new EventDto()
            {
                Id = NameSpace.Create(vhid),
                Data = new CreateEntity()
                {
                    Type = type,
                    VhId = vhid,
                    Initializer = initializer
                }
            };
        }
    }
}
