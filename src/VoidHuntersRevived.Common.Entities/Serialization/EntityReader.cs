using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        public static readonly EntityReader Instance = new EntityReader();

        private EntityData _loaded;

        public EntityReader() : base(new MemoryStream())
        {
            _loaded = new EntityData(Array.Empty<byte>());
        }

        public void Load(EntityData data)
        {
            if(_loaded.Id != data.Id)
            {
                this.BaseStream.Position = 0;
                this.BaseStream.Write(data.Bytes, 0, data.Bytes.Length);
                this.BaseStream.Position = 0;

                _loaded = data;
            }
        }
        public void Load(EntityReaderState state)
        {
            if (_loaded.Id != state.Data.Id)
            {
                this.BaseStream.Position = state.Position;
                this.BaseStream.Write(state.Data.Bytes, state.Position, state.Data.Bytes.Length - state.Position);

                _loaded = state.Data;
            }

            this.BaseStream.Position = state.Position;
        }

        public EntityReaderState GetState(in VhId seed)
        {
            return new EntityReaderState(_loaded!, seed, (int)this.BaseStream.Position);
        }


        public VhId ReadVhId()
        {
            return this.ReadUnmanaged<VhId>();
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
