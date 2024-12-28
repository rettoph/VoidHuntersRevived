namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public readonly ref struct EntityWriter(List<byte> data, Stack<EntityLocalId> nested)
    {
        private readonly List<byte> _data = data;
        private readonly Stack<EntityLocalId> _nested = nested;

        public unsafe void Write<T>(T value)
            where T : unmanaged
        {
            byte* pBytes = (byte*)&value;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            foreach (byte b in span)
            {
                _data.Add(b);
            }
        }

        public unsafe void Write(byte* data, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _data.Add(data[i]);
            }
        }

        public void Push(EntityLocalId localId)
        {
            _nested.Push(localId);
        }

        public bool WriteIf(bool condition)
        {
            this.Write(condition);

            return condition;
        }
    }
}
