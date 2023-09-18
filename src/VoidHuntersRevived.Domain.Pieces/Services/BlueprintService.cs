using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class BlueprintService : IBlueprintService
    {
        private Dictionary<Id<Blueprint>, Blueprint> _blueprints;

        public BlueprintService(IResourceProvider resources, IEnumerable<Blueprint> blueprints)
        {
            _blueprints = resources.GetAll<Blueprint>()
                .Select(x => x.Item2)
                .Concat(blueprints)
                .ToDictionary(x => x.Id, x => x);
        }
    }
}
