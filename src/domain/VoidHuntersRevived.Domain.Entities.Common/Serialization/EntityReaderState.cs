namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public struct EntityReaderState(EntityData data, int position)
    {
        public readonly EntityData Data = data;
        public readonly int Position = position;
    }
}
