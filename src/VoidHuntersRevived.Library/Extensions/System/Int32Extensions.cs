using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System
{
    public static class Int32Extensions
    {
        public static Guid ToGuid(this Int32 value)
        {
            Byte[] bytes = new Byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}
