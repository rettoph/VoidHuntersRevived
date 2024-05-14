using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Extensions
{
    public static class UnmanagedExtensions
    {
        public static unsafe bool IsDefault<T>(this T instance)
            where T : unmanaged
        {
            T* ptr = &instance;
            byte* bytes = (byte*)ptr;

            for(int i=0; i<sizeof(T); i++)
            {
                if (bytes[i] != (byte)0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
