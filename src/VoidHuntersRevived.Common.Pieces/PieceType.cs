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
        public readonly IReadOnlyDictionary<Type, IPieceComponent> InstanceComponents;
        public readonly IReadOnlyDictionary<Type, IPieceComponent> StaticComponents;

        public PieceType(
            string key,
            VoidHuntersEntityDescriptor descriptor,
            Dictionary<Type, IPieceComponent> components,
            Dictionary<Type, IPieceComponent> data)
        {
            this.Id = HashBuilder<PieceType, VhId, VhId>.Instance.CalculateId(VhId.HashString(key), descriptor.Id.Value);
            this.Key = key;
            this.EntityType = BuildEntityType(descriptor, key);
            this.Descriptor = descriptor;
            this.InstanceComponents = new ReadOnlyDictionary<Type, IPieceComponent>(components);
            this.StaticComponents = new ReadOnlyDictionary<Type, IPieceComponent>(data);
        }

        private static IEntityType<PieceDescriptor> BuildEntityType(VoidHuntersEntityDescriptor descriptor, string key)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<PieceDescriptor>)Activator.CreateInstance(entityTypeType, key)!;
        }
    }
}
