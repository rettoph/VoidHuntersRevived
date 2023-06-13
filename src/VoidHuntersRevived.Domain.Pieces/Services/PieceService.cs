using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public class PieceService : IPieceService
    {
        private readonly IEntityService _entities;
        private readonly PieceConfigurationService _configurations; 

        public PieceService(
            PieceConfigurationService configurations, 
            IEntityService entities)
        {
            _configurations = configurations;
            _entities = entities;
        }

        public Guid Create(PieceType type, Guid id)
        {
            return _configurations.Create(type, id, _entities);
        }
    }
}
