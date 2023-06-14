using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities
{
    internal abstract class PreCacheProperty
    {
        internal abstract PropertyCache Iniitalize(EntityPropertyService properties);
    }

    internal sealed class PreCacheProperty<T> : PreCacheProperty
        where T : class, IEntityProperty
    {
        private readonly T _instance;

        public PreCacheProperty(T instance)
        {
            _instance = instance;
        }

        internal override PropertyCache Iniitalize(EntityPropertyService properties)
        {
            return properties.Cache<T>(_instance);
        }
    }
}
