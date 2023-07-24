﻿using Guppy.Attributes;
using Guppy.Resources.Providers;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;

using Colors = VoidHuntersRevived.Common.Resources.Colors;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(PieceTypes.HullTriangle, configuration =>
            {
                configuration.HasInitializer((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Rigid>(Rigid.Polygon(Fix64.One, 3));
                    initializer.Init<Visible>(Visible.Polygon(Colors.Orange, 3));
                    initializer.Init<Joints>(Joints.Polygon(id, 3));
                });
            });

            entityTypes.Configure(PieceTypes.HullSquare, configuration =>
            {
                configuration.HasInitializer((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Rigid>(Rigid.Polygon(Fix64.One, 4));
                    initializer.Init<Visible>(Visible.Polygon(Colors.Orange, 4));
                    initializer.Init<Joints>(Joints.Polygon(id, 4));
                });
            });
        }
    }
}
