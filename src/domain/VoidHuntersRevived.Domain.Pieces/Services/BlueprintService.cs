using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed partial class BlueprintService(IEnumerable<Blueprint> blueprints, IResourceService resources) : IBlueprintService
    {
        private readonly Dictionary<Id<Blueprint>, Blueprint> _blueprints = resources.GetAll<Blueprint>().Select(x => x.Value).Concat(blueprints).ToDictionary(x => x.Id, x => x);

        public Blueprint GetById(Id<Blueprint> id) => this._blueprints[id];

        public IEnumerable<Blueprint> GetAll() => this._blueprints.Values;
    }
}