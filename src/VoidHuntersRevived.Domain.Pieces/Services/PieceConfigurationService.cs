using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Loaders;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed class PieceConfigurationService : IPieceConfigurationService
    {
        private readonly PieceCategoryService _categories;
        private readonly PiecePropertyService _properties;
        private readonly Dictionary<PieceType, PieceConfiguration> _configurations;

        public PieceConfigurationService(
            PieceCategoryService categories, 
            PiecePropertyService properties,
            ISorted<IPieceLoader> loaders)
        {
            _categories = categories;
            _properties = properties;
            _configurations = new Dictionary<PieceType, PieceConfiguration>();

            foreach(IPieceLoader loader in loaders)
            {
                loader.Configure(this);
            }
        }

        public void Configure(PieceType type, Action<IPieceConfiguration> configuration)
        {
            if (!_configurations.TryGetValue(type, out PieceConfiguration? conf))
            {
                conf = _configurations[type] = new PieceConfiguration(type, _categories, _properties);
            }

            configuration(conf);

            if(conf.Category is null)
            {
                throw new ArgumentNullException(nameof(conf.Category));
            }
        }

        public Guid Create(PieceType type, Guid id, IEntityService entities)
        {
            PieceConfiguration configuration = _configurations[type];
            return entities.Create(configuration.Category, id, configuration.Initialize);
        }
    }
}
