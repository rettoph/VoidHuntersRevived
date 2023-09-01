using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    internal interface IVoidHuntersEntityDescriptorEngine
    {
        VoidHuntersEntityDescriptor Descriptor { get; }

        EntityInitializer Spawn(VhId vhid, out EntityId id);

        void SoftDespawn(in EntityId id, in GroupIndex groupIndex);
        void RevertSoftDespawn(in EntityId id, in GroupIndex groupIndex);

        void HardDespawn(in EntityId id, in GroupIndex groupIndex);

        void Serialize(EntityWriter writer, in GroupIndex groupIndex);
        void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }
}
