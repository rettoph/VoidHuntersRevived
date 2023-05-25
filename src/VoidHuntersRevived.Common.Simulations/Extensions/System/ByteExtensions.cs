using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Extensions.System
{
    public static class ByteExtensions
    {
        // public static unsafe void Encode(this byte[] array, int offset, Guid value)
        // {
        //     if (array.Length - offset < 16) throw new ArgumentException("buffer too small");
        // 
        //     fixed (byte* pArray = array)
        //     {
        //         var pGuid = (long*)&value;
        //         var pDest = (long*)(pArray + offset);
        //         pDest[0] = pGuid[0];
        //         pDest[1] = pGuid[1];
        //     }
        // }
        // 
        // public static unsafe void Encode(this byte[] array, int offset, UInt128 value)
        // {
        //     if (array.Length - offset < 16) throw new ArgumentException("buffer too small");
        // 
        //     fixed (byte* pArray = array)
        //     {
        //         var pUInt128 = (ulong*)&value;
        //         var pDest = (ulong*)(pArray + offset);
        //         pDest[0] = pUInt128[0];
        //         pDest[1] = pUInt128[1];
        //     }
        // }
    }
}
