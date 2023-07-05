namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        public static readonly EntityWriter Instance = new EntityWriter();

        public EntityWriter() : base(new MemoryStream())
        {
        }

        public unsafe void WriteUnmanaged<T>(T value)
            where T : unmanaged
        {
            byte* pBytes = (byte*)&value;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            this.Write(span);
        }

        public void Write(VhId vhid)
        {
            this.WriteUnmanaged(vhid);
        }

        public void Reset()
        {
            this.BaseStream.Position = 0;
        }

        public EntityData Export()
        {
            byte[] bytes = new byte[this.BaseStream.Position];

            this.BaseStream.Position = 0;
            this.BaseStream.Read(bytes, 0, bytes.Length);

            return new EntityData(bytes);
        }
    }
}
