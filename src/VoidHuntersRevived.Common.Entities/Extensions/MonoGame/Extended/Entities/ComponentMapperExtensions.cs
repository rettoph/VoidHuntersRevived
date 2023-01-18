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
        public static void Delete<T>(this ComponentMapper<T> mapper, Entity entity)
            where T : class
        {
            mapper.Delete(entity.Id);
        }

        public static bool Has<T>(this ComponentMapper<T> mapper, Entity entity)
            where T : class
        {
            return mapper.Has(entity.Id);
        }

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

        public static bool TryGet<T>(this ComponentMapper<T> mapper, Entity entity, [MaybeNullWhen(false)] out T value)
            where T : class
        {
            if (mapper.Has(entity.Id))
            {
                value = mapper.Get(entity);
                return true;
            }

            value = null;
            return false;
        }
    }
}
