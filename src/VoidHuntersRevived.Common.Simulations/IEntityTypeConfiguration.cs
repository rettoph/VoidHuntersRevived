using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        IEntityTypeConfiguration Inherits(EntityType baseType);

        IEntityTypeConfiguration Has<T>()
            where T : unmanaged;
    }
}
