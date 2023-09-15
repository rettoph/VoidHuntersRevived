using Guppy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces
{
    public record Piece
    {
        public static readonly Resource<Piece> Resource = new Resource<Piece>($"{nameof(Piece)}s");

        private IEntityType<PieceDescriptor>? _entityType;
        public IEntityType<PieceDescriptor> EntityType => _entityType ??= BuildEntityType(this.Descriptor, this.Key);

        public string Key { get; set; } = string.Empty;
        public VoidHuntersEntityDescriptor Descriptor { get; set; } = null!;
        public Dictionary<string, IPieceComponent> Components { get; set; } = new Dictionary<string, IPieceComponent>();

        private static IEntityType<PieceDescriptor> BuildEntityType(VoidHuntersEntityDescriptor descriptor, string key)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<PieceDescriptor>)Activator.CreateInstance(entityTypeType, key)!;
        }
    }
}
