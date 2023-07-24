using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        public bool Busy;

        public EntityWriter() : base(new MemoryStream())
        {
        }

        public void Reset()
        {
            if (this.Busy)
            {
                throw new Exception();
            }

            this.Busy = true;
            this.BaseStream.Position = 0;
        }

        public EntityData Export(VhId id)
        {
            byte[] bytes = new byte[this.BaseStream.Position];

            this.BaseStream.Position = 0;
            this.BaseStream.Read(bytes, 0, bytes.Length);
            this.Busy = false;

            return new EntityData(id, bytes);
        }

        public unsafe void WriteStruct<T>(T value)
            where T : unmanaged
        {
            byte* pBytes = (byte*)&value;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            this.Write(span);
        }

        public void Write(VhId vhid)
        {
            this.WriteStruct(vhid);
        }

        public bool WriteIf(bool value)
        {
            this.Write(value);

            return value;
        }

        public void WriteNativeDynamicArray<T>(IEntityService entities, NativeDynamicArrayCast<T> native, Action<IEntityService, EntityWriter, T> writer)
            where T : unmanaged
        {
            this.Write(native.count);

            for (int i = 0; i < native.count; i++)
            {
                writer(entities, this, native[i]);
            }
        }

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native, Action<EntityWriter, T> writer)
            where T : unmanaged
        {
            this.Write(native.count);

            for (int i = 0; i < native.count; i++)
            {
                writer(this, native[i]);
            }
        }

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native)
            where T : unmanaged
        {
            this.WriteNativeDynamicArray<T>(native, DefaultNativeDynamicArrayItemWriter<T>);
        }

        private static void DefaultNativeDynamicArrayItemWriter<T>(EntityWriter writer, T item)
            where T : unmanaged
        {
            writer.WriteStruct<T>(item);
        }
    }
}
