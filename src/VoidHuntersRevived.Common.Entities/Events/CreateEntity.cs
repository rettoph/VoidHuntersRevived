using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class CreateEntity : IEventData
    {
        public static readonly VhId NameSpace = new VhId("6a7a5aed-2cfe-4c85-8c24-421a82f1d738");

        public required VhId VhId { get; init; }
        public required IEntityType Type { get; init; }
        public required EntityInitializerDelegate? Initializer { get; init; }

        public static EventDto CreateEvent(IEntityType type, VhId vhid)
        {
            return InternalCreateEvent(type, vhid, null);
        }

        public static EventDto CreateEvent(IEntityType type, VhId vhid, EntityInitializerDelegate initializer)
        {
            return InternalCreateEvent(type, vhid, initializer);
        }

        private static EventDto InternalCreateEvent(IEntityType type, VhId vhid, EntityInitializerDelegate? initializer)
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
