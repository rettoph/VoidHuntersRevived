using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        private readonly bool _seeding;
        private readonly VhId _seed;

        public EntityReader(EntityData data) : this(null, data)
        {
        }

        public EntityReader(VhId? seed, EntityData data) : base(data, Encoding.UTF8, true)
        {
            _seeding = seed is not null;
            _seed = seed ?? default;
        }

        public VhId ReadVhId()
        {
            VhId value = this.ReadUnmanaged<VhId>();

            return _seeding ? this.Seed(value) : value;
        }

        public VhId Seed(VhId vhid)
        {
            return _seed.Create(vhid);
        }

        public unsafe T ReadUnmanaged<T>()
            where T : unmanaged
        {
            Span<byte> bytes = stackalloc byte[sizeof(T)];
            this.Read(bytes);

            fixed(byte* pbytes = &bytes[0])
            {
                T* value = (T*)&pbytes[0];

                return value[0];
            }
        }
    }
}
