using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class EntityTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Register(
                EntityTypes.UserShip, 
                EntityTypes.Chain);
        }
    }
}
