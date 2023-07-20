using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityType
    {
        VoidHuntersEntityDescriptor Descriptor { get; }
        string Name { get; }
        VhId Id { get; }

        IEntityTypeConfiguration BuildConfiguration();

        void DestroyEntity(IEntityFunctions functions, in EGID egid);
        EntityInitializer CreateEntity(IEntityFactory factory, VhId vhid);
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
