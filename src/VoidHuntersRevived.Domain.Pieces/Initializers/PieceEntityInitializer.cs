﻿using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Initializers
{
    [AutoLoad]
    internal class PieceEntityInitializer : BaseEntityInitializer
    {
        public PieceEntityInitializer(IPieceTypeService pieces)
        {
            foreach(PieceType piece in pieces.All())
            {
                StaticEntityInitializerDelegate? staticInitializer = EntityInitializerHelper.BuildStaticEntityInitializerDelegate(piece.StaticComponents.Values);
                InstanceEntityInitializerDelegate? instanceInitializer = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(piece.Id);
                };

                instanceInitializer += EntityInitializerHelper.BuildInstanceEntityInitializerDelegate(piece.InstanceComponents.Values);

                this.WithStaticInitializer(piece.EntityType, staticInitializer);
                this.WithInstanceInitializer(piece.EntityType, instanceInitializer);
            }
        }
    }
}
