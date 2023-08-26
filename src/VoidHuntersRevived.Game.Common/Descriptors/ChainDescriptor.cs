using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Common.Descriptors
{
    public class ChainDescriptor : TreeDescriptor
    {
        public ChainDescriptor()
        {
            this.ExtendWith(new[]
            {
                new ComponentManager<Tractorable>(
                    builder: new ComponentBuilder<Tractorable>(),
                    serializer: DefaultComponentSerializer<Tractorable>.Default)
            });
        }
    }
}
