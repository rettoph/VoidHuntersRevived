using Guppy.Core.Resources.Common.Services;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed partial class BlueprintService : IBlueprintService
    {
        private readonly Dictionary<Id<Blueprint>, Blueprint> _blueprints;

        public BlueprintService(ILogger logger, IEnumerable<Blueprint> blueprints, IResourceService resources)
        {
            _blueprints = resources.GetValues<Blueprint>().Select(x => x.Value).Concat(blueprints).ToDictionary(x => x.Id, x => x);
        }

        public Blueprint GetById(Id<Blueprint> id)
        {
            return _blueprints[id];
        }

        public IEnumerable<Blueprint> GetAll()
        {
            return _blueprints.Values;
        }
    }
}
