using Guppy.Attributes;
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
using VoidHuntersRevived.Common.Pieces.Resources;
using Colors = VoidHuntersRevived.Common.Resources.Colors;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceTypeLoader : IEntityTypeLoader
    {
        private readonly IResourceProvider _resources;

        public PieceTypeLoader(IResourceProvider resources)
        {
            _resources = resources;

            _resources
                .Set(PieceResources.HullSquare.Rigid, Rigid.Polygon(Fix64.One, 4))
                .Set(PieceResources.HullSquare.Visible, Visible.Polygon(Colors.Orange, 4));

            _resources
                .Set(PieceResources.HullTriangle.Rigid, Rigid.Polygon(Fix64.One, 3))
                .Set(PieceResources.HullTriangle.Visible, Visible.Polygon(Colors.Orange, 3));
        }

        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(PieceTypes.HullTriangle, configuration =>
            {
                configuration.HasInitializer((ref EntityInitializer initializer) =>
                {
                    initializer.Init<ResourceId<Rigid>>(PieceResources.HullTriangle.Rigid);
                    initializer.Init<ResourceId<Visible>>(PieceResources.HullTriangle.Visible);
                    initializer.Init<Joints>(Joints.Polygon(3));
                });
            });

            entityTypes.Configure(PieceTypes.HullSquare, configuration =>
            {
                configuration.HasInitializer((ref EntityInitializer initializer) =>
                {
                    initializer.Init<ResourceId<Rigid>>(PieceResources.HullSquare.Rigid);
                    initializer.Init<ResourceId<Visible>>(PieceResources.HullSquare.Visible);
                    initializer.Init<Joints>(Joints.Polygon(4));
                });
            });
        }
    }
}
