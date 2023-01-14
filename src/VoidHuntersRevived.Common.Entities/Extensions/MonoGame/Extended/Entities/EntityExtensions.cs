using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Entities
{
    public static class EntityExtensions
    {
        public static bool TryGet<T>(this Entity entity, [MaybeNullWhen(false)] out T value)
            where T : class
        {
            if(entity.Has<T>())
            {
                value = entity.Get<T>();
                return true;
            }

            value = null;
            return false;
        }    
    }
}
