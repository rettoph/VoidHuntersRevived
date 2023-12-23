using Svelto.DataStructures;
using VoidHuntersRevived.Common.Entities.Options;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        public EntityWriter() : base(new MemoryStream())
        {
        }

        public void Reset()
        {
            this.BaseStream.Position = 0;
        }

        public EntityData Export(VhId id)
        {
            byte[] bytes = new byte[this.BaseStream.Position];

            this.BaseStream.Position = 0;
            this.BaseStream.Read(bytes, 0, bytes.Length);

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

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native, Action<EntityWriter, T, SerializationOptions> writer, in SerializationOptions options)
            where T : unmanaged
        {
            this.Write(native.count);

            for (int i = 0; i < native.count; i++)
            {
                writer(this, native[i], options);
            }
        }

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native)
            where T : unmanaged
        {
            this.WriteNativeDynamicArray<T>(native, DefaultNativeDynamicArrayItemWriter<T>, SerializationOptions.Default);
        }

        private static void DefaultNativeDynamicArrayItemWriter<T>(EntityWriter writer, T item, SerializationOptions options)
            where T : unmanaged
        {
            writer.WriteStruct<T>(item);
        }
    }
}
