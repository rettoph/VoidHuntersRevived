using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class BlueprintService : IBlueprintService
    {
        private readonly Dictionary<Id<Blueprint>, Blueprint> _blueprints;

        public BlueprintService(IResourceProvider resources, IEnumerable<Blueprint> blueprints)
        {
            _blueprints = new Dictionary<Id<Blueprint>, Blueprint>();

            foreach (Blueprint blueprint in resources.GetAll<Blueprint>().Select(x => x.Item2).Concat(blueprints))
            {

            }
        }

        public Blueprint GetById(Id<Blueprint> id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Blueprint> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
