using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System.Collections
{
    public static class ListExtensions
    {
        public static Boolean AddIfNotNull<TValue>(this List<TValue> list, TValue value)
            where TValue : class
        {
            if(value != default(TValue))
            {
                list.Add(value);
                return true;
            }

            return false;
        }
    }
}
