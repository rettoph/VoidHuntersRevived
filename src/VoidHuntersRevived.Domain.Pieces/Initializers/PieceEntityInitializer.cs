using Guppy.Attributes;
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
    internal class PieceEntityInitializer : IEntityInitializer
    {
        private readonly Dictionary<IEntityType, PieceType> _pieceTypes;
        private readonly IPieceTypeService _pieces;

        public IEntityType[] RegisterTypes { get; }

        public PieceEntityInitializer(IPieceTypeService pieces)
        {
            _pieces = pieces;
            _pieceTypes = _pieces.All().ToDictionary(x => x.EntityType as IEntityType, x => x);

            this.RegisterTypes = _pieceTypes.Keys.ToArray();
        }

        public bool ShouldInitialize(IEntityType entityType)
        {
            return this.RegisterTypes.Contains(entityType);
        }

        public InstanceEntityInitializerDelegate? InstanceInitializer(IEntityType entityType)
        {
            PieceType pieceType = _pieceTypes[entityType];

            InstanceEntityInitializerDelegate result = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(pieceType.Id);
            };

            result += EntityInitializerHelper.BuildInstanceEntityInitializerDelegate(pieceType.InstanceComponents.Values);

            return result;
        }

        public DisposeEntityInitializerDelegate? InstanceDisposer(IEntityType entityType)
        {
            return null;
        }

        public StaticEntityInitializerDelegate? StaticInitializer(IEntityType entityType)
        {
            PieceType pieceType = _pieceTypes[entityType];

            return EntityInitializerHelper.BuildStaticEntityInitializerDelegate(pieceType.StaticComponents.Values);
        }

        public DisposeEntityInitializerDelegate? StaticDisposer(IEntityType entityType)
        {
            return null;
        }
    }
}
