using Guppy.Core.Resources;
using Serilog;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed partial class BlueprintService : IBlueprintService
    {
        private readonly Dictionary<Id<Blueprint>, Blueprint> _blueprints;
        private readonly ILogger _logger;

        public BlueprintService(ILogger logger, IEnumerable<Blueprint> blueprints)
        {
            _logger = logger;
            _blueprints = Resource<Blueprint>.GetAll().Select(x => x.Value).Concat(blueprints).ToDictionary(x => x.Id, x => x);
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
