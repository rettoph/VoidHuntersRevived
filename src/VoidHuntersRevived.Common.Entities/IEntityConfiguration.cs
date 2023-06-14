using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityConfiguration
    {
        EntityName Name { get; }
        EntityType Type { get; set; }

        IEntityConfiguration SetType(EntityType type);

        IEntityConfiguration AddProperty<T>(T property)
            where T : class, IEntityProperty;
    }
}
