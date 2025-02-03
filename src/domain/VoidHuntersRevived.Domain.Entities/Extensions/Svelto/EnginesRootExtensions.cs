using System.Reflection;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Extensions.Svelto
{
    public static class EnginesRootExtensions
    {
        private static readonly FieldInfo _entitiesDB = typeof(EnginesRoot).GetField(nameof(_entitiesDB), BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new NotImplementedException();
        public static EntitiesDB GetEntitiesDB(this EnginesRoot enginesRoot)
        {
            EntitiesDB entitiesDB = (EntitiesDB)(_entitiesDB.GetValue(enginesRoot) ?? throw new NotImplementedException());

            return entitiesDB;
        }
    }
}
