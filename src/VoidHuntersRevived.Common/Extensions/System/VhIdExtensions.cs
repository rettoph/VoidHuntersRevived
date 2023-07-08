using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public static class VhIdExtensions
    {
        private static int[] _buffer = new int[8];

        public static unsafe VhId Create(this VhId nameSpace, int name)
        {
            int* pNameSpace = (int*)&nameSpace;
            _buffer[0] = pNameSpace[0];
            _buffer[1] = pNameSpace[1];
            _buffer[2] = pNameSpace[2];
            _buffer[3] = pNameSpace[3];
            _buffer[4] = 0;
            _buffer[5] = 0;
            _buffer[6] = 0;
            _buffer[7] = name;

            fixed (int* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, 32);
                uint128 hash = xxHash128.ComputeHash(dataSpan, 32);
                VhId* newId = (VhId*)&hash;

                return newId[0];
            }
        }

        public static unsafe VhId Create(this VhId nameSpace, VhId name)
        {
            if(nameSpace.Value == name.Value)
            {
                return name;
            }

            int* pNameSpace = (int*)&nameSpace;
            _buffer[0] = pNameSpace[0];
            _buffer[1] = pNameSpace[1];
            _buffer[2] = pNameSpace[2];
            _buffer[3] = pNameSpace[3];

            int* pName = (int*)&name;
            _buffer[4] = pName[0];
            _buffer[5] = pName[1];
            _buffer[6] = pName[2];
            _buffer[7] = pName[3];

            fixed (int* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, 32);
                uint128 hash = xxHash128.ComputeHash(dataSpan, 32);
                VhId* newId = (VhId*)&hash;

                return newId[0];
            }
        }

        public static unsafe VhId Create(this VhId nameSpace, string name)
        {
            uint128 nameHash = xxHash128.ComputeHash(name);
            VhId* pNameHash = (VhId*)&nameHash;

            return nameSpace.Create(pNameHash[0]);
        }
    }
}
