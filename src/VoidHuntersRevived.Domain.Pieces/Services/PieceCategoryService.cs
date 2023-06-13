using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Loaders;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed class PieceCategoryService : IPieceCategoryService
    {
        private readonly Dictionary<PieceCategory, PieceCategoryConfiguration> _configurations;

        public PieceCategoryService(ISorted<IPieceCategoryLoader> loaders)
        {
            _configurations = new Dictionary<PieceCategory, PieceCategoryConfiguration>();

            foreach(IPieceCategoryLoader loader in loaders)
            {
                loader.Configure(this);
            }
        }

        public void Configure(PieceCategory category, Action<IPieceCategoryConfiguration> configuration)
        {
            configuration(this.Get(category));
        }

        public PieceCategoryConfiguration Get(PieceCategory category)
        {
            if (!_configurations.TryGetValue(category, out PieceCategoryConfiguration? categoryConfiguration))
            {
                _configurations[category] = categoryConfiguration = new PieceCategoryConfiguration(category);
            }

            return categoryConfiguration;
        }

        public void Initialize(IEntityTypeService entityTypes)
        {
            foreach(PieceCategoryConfiguration configuration in _configurations.Values)
            {
                configuration.Initialize(entityTypes);
            }
        }
    }
}
