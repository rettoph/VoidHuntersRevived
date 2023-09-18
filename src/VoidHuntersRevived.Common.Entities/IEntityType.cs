using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityType : IEntityResource<IEntityType>
    {
        VoidHuntersEntityDescriptor Descriptor { get; }
        string Key { get; }

        IEntityTypeConfiguration BuildConfiguration();
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
