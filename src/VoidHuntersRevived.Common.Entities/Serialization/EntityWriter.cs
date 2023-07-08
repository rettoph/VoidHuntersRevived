namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        public bool Busy;

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
            if(this.Busy)
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
    }
}
