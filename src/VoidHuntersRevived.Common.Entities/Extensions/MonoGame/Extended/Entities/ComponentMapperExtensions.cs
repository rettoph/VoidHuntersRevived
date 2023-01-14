using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Entities
{
    public static class ComponentMapperExtensions
    {
        public static bool TryGet<T>(this ComponentMapper<T> mapper, int entityId, [MaybeNullWhen(false)] out T value)
            where T : class
        {
            if(mapper.Has(entityId))
            {
                value = mapper.Get(entityId);
                return true;
            }

            value = null;
            return false;
        }
    }
}
