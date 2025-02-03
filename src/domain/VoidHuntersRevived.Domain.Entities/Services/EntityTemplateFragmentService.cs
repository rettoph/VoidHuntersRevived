using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateFragmentService : IEntityTemplateFragmentService
    {
        private readonly EntityTemplateFragment[] _fragments;
        private readonly Dictionary<Key<IEntityTemplate>, EntityTemplateFragment[]> _fragmentsByKey;
        private readonly Type[] _distinctComponentTypes;

        public EntityTemplateFragmentService(IResourceService resources)
        {
            this._fragments = resources.GetAll<EntityTemplateFragment>()
                .Where(x => x.HasValue)
                .Select(x => x.Value!)
                .ToArray();

            this._fragmentsByKey = this._fragments
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.ToArray());

            this._distinctComponentTypes = this._fragments
                .SelectMany(x => x.Components.Select(c => c.GetType()))
                .Distinct()
                .ToArray();
        }

        public virtual IReadOnlyDictionary<Key<IEntityTemplate>, EntityTemplateFragment[]> GetAll()
        {
            return this._fragmentsByKey;
        }

        public EntityTemplateFragment[] GetByKey(Key<IEntityTemplate> key)
        {
            return this._fragmentsByKey[key];
        }

        public Type[] GetAllDistinctComponentTypes()
        {
            return this._distinctComponentTypes;
        }
    }
}