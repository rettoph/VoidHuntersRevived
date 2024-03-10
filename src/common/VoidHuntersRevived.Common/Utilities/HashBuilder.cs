using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common.Core.Utilities
{
    public class HashBuilder<TNameSpace>
        where TNameSpace : unmanaged
    {
        private byte[] _buffer;
        private int _index;

        public unsafe HashBuilder(TNameSpace nameSpace, int bufferCapacity)
        {
            _buffer = new byte[bufferCapacity + sizeof(TNameSpace)];
            this.Append(nameSpace);
        }

        public unsafe HashBuilder<TNameSpace> Reset()
        {
            _index = sizeof(TNameSpace);

            return this;
        }

        public unsafe HashBuilder<TNameSpace> Append<T>(in T name)
            where T : unmanaged
        {
            fixed (T* pValue = &name)
            {
                byte* pBytes = (byte*)pValue;

                for (int i = 0; i < sizeof(T); i++)
                {
                    _buffer[_index++] = pBytes[i];
                }
            }

            return this;
        }

        public unsafe VhId Calculate()
        {
            uint128 hash = xxHash128.ComputeHash(_buffer, _index);
            VhId* newId = (VhId*)&hash;

            return newId[0];
        }
    }

    public class HashBuilder<TNameSpace, TName1> : HashBuilder<VhId>
        where TName1 : unmanaged
    {
        public static readonly HashBuilder<TNameSpace, TName1> Instance = new HashBuilder<TNameSpace, TName1>();

        public unsafe HashBuilder() : base(NameSpace<TNameSpace>.Instance, sizeof(TName1))
        {
        }

        public VhId Calculate(in TName1 value)
        {
            return this.Reset()
                .Append(in value)
                .Calculate();
        }
    }

    public class HashBuilder<TNameSpace, TName1, TName2> : HashBuilder<VhId>
        where TName1 : unmanaged
        where TName2 : unmanaged
    {
        public static readonly HashBuilder<TNameSpace, TName1, TName2> Instance = new HashBuilder<TNameSpace, TName1, TName2>();

        public unsafe HashBuilder() : base(NameSpace<TNameSpace>.Instance, sizeof(TName1) + sizeof(TName2))
        {
        }

        public VhId Calculate(in TName1 value1, in TName2 value2)
        {
            return this.Reset()
                .Append(in value1)
                .Append(in value2)
                .Calculate();
        }
    }

    public class HashBuilder<TNameSpace, TName1, TName2, TName3> : HashBuilder<VhId>
        where TName1 : unmanaged
        where TName2 : unmanaged
        where TName3 : unmanaged
    {
        public static readonly HashBuilder<TNameSpace, TName1, TName2, TName3> Instance = new HashBuilder<TNameSpace, TName1, TName2, TName3>();

        public unsafe HashBuilder() : base(NameSpace<TNameSpace>.Instance, sizeof(TName1) + sizeof(TName2) + sizeof(TName3))
        {
        }

        public VhId Calculate(in TName1 value1, in TName2 value2, in TName3 value3)
        {
            return this.Reset()
                .Append(in value1)
                .Append(in value2)
                .Append(in value3)
                .Calculate();
        }
    }

    public class HashBuilder<TNameSpace, TName1, TName2, TName3, TName4> : HashBuilder<VhId>
        where TName1 : unmanaged
        where TName2 : unmanaged
        where TName3 : unmanaged
        where TName4 : unmanaged
    {
        public static readonly HashBuilder<TNameSpace, TName1, TName2, TName3, TName4> Instance = new HashBuilder<TNameSpace, TName1, TName2, TName3, TName4>();

        public unsafe HashBuilder() : base(NameSpace<TNameSpace>.Instance, sizeof(TName1) + sizeof(TName2) + sizeof(TName3) + sizeof(TName4))
        {
        }

        public VhId Calculate(in TName1 value1, in TName2 value2, in TName3 value3, in TName4 value4)
        {
            return this.Reset()
                .Append(in value1)
                .Append(in value2)
                .Append(in value3)
                .Append(in value4)
                .Calculate();
        }
    }

    public class HashBuilder<TNameSpace, TName1, TName2, TName3, TName4, TName5> : HashBuilder<VhId>
        where TName1 : unmanaged
        where TName2 : unmanaged
        where TName3 : unmanaged
        where TName4 : unmanaged
        where TName5 : unmanaged
    {
        public static readonly HashBuilder<TNameSpace, TName1, TName2, TName3, TName4, TName5> Instance = new HashBuilder<TNameSpace, TName1, TName2, TName3, TName4, TName5>();

        public unsafe HashBuilder() : base(NameSpace<TNameSpace>.Instance, sizeof(TName1) + sizeof(TName2) + sizeof(TName3) + sizeof(TName4) + sizeof(TName5))
        {
        }

        public VhId Calculate(in TName1 value1, in TName2 value2, in TName3 value3, in TName4 value4, in TName5 value5)
        {
            return this.Reset()
                .Append(in value1)
                .Append(in value2)
                .Append(in value3)
                .Append(in value4)
                .Append(in value5)
                .Calculate();
        }
    }
}
