using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common.Core
{
    public static class VhIdExtensions
    {
        private static int[] _buffer = new int[12];

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
            if (nameSpace.Value == default)
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

        public static unsafe VhId Create(this VhId nameSpace, VhId name1, VhId name2)
        {
            int* pNameSpace = (int*)&nameSpace;
            _buffer[0] = pNameSpace[0];
            _buffer[1] = pNameSpace[1];
            _buffer[2] = pNameSpace[2];
            _buffer[3] = pNameSpace[3];

            int* pName1 = (int*)&name1;
            _buffer[4] = pName1[0];
            _buffer[5] = pName1[1];
            _buffer[6] = pName1[2];
            _buffer[7] = pName1[3];

            int* pName2 = (int*)&name2;
            _buffer[8] = pName2[0];
            _buffer[9] = pName2[1];
            _buffer[10] = pName2[2];
            _buffer[11] = pName2[3];

            fixed (int* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, 48);
                uint128 hash = xxHash128.ComputeHash(dataSpan, 48);
                VhId* newId = (VhId*)&hash;

                return newId[0];
            }
        }

        public static unsafe VhId Create(this VhId nameSpace, string name)
        {
            return nameSpace.Create(VhId.HashString(name));
        }
    }
}
