using Guppy.Resources.Providers;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

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
            _blueprints = resources.GetAll<Blueprint>().Select(x => x.Item2).Concat(blueprints).ToDictionary(x => x.Id, x => x);
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
