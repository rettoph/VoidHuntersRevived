using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface ISerializationEngine<T>
        where T : struct, IEntityComponent
    {
        void Serialize(in T component, EntityWriter writer);
        void Deserialize(in VhId seed, EntityReader reader, ref T component);
    }
}
