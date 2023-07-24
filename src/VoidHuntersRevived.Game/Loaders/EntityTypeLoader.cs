using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Constants;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class EntityTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(EntityTypes.UserShip, configuration =>
            {
                configuration.HasInitializer((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Collision>(new Collision()
                    {
                        Categories = CollisionGroups.ShipCategories,
                        CollidesWith = CollisionGroups.ShipCollidesWith
                    });
                });
            });

            entityTypes.Configure(EntityTypes.Chain, configuration =>
            {
                configuration.HasInitializer((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Collision>(new Collision()
                    {
                        Categories = CollisionGroups.FreeFloatingCategories,
                        CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                    });
                });
            });
        }
    }
}
