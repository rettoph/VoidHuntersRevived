using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces
{
    public sealed class PieceType
    {
        public readonly Id<PieceType> Id;
        public readonly string Key;
        public readonly IEntityType<PieceDescriptor> EntityType;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly IReadOnlyDictionary<string, IPieceComponent> Components;

        public PieceType(
            string key,
            VoidHuntersEntityDescriptor descriptor,
            Dictionary<string, IPieceComponent> components)
        {
            this.Id = HashBuilder<PieceType, VhId, VhId>.Instance.CalculateId(VhId.HashString(key), descriptor.Id.Value);
            this.Key = key;
            this.EntityType = BuildEntityType(descriptor, key);
            this.Descriptor = descriptor;
            this.Components = new ReadOnlyDictionary<string, IPieceComponent>(components);
        }

        private static IEntityType<PieceDescriptor> BuildEntityType(VoidHuntersEntityDescriptor descriptor, string key)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<PieceDescriptor>)Activator.CreateInstance(entityTypeType, key)!;
        }
    }
}
