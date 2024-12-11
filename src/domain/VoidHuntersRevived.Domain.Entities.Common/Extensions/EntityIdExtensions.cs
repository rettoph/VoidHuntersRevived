namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class EntityIdExtensions
    {
        public static EntityLocalId ToLocalEntityId(this EntityId id)
        {
            return new EntityLocalId(id.EGID);
        }
    }
}
