namespace Svelto.ECS
{
    public static class EntityReferenceExtensions
    {
        public static unsafe int GetFilterId(this EntityReference reference) => ((int*)&reference.uniqueID)[0];
    }
}