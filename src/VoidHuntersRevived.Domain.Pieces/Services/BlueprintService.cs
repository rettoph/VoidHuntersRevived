using Guppy.Resources.Providers;
using Serilog;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed partial class BlueprintService : IBlueprintService
    {
        private readonly Dictionary<Id<Blueprint>, Blueprint> _blueprints;
        private readonly IPieceTypeService _pieces;
        private readonly ILogger _logger;

        public BlueprintService(IPieceTypeService pieces, IResourceProvider resources, ILogger logger, IEnumerable<Blueprint> blueprints)
        {
            _pieces = pieces;
            _logger = logger;
            _blueprints = new Dictionary<Id<Blueprint>, Blueprint>();
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
