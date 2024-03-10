using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class TreeComponentSerializer : NotImplementedComponentSerializer<Tree>
    {
        //private readonly IEntityService _entities;
        //
        //public TreeComponentSerializer(IEntityService entities)
        //{
        //    _entities = entities;
        //}
        //
        //protected override Tree Read(EntityReader reader, EntityId id)
        //{
        //    return new Tree(_entities.Deserialize(reader, null));
        //}
        //
        //protected override void Write(EntityWriter writer, Tree instance)
        //{
        //    _entities.Serialize(instance.HeadId, writer);
        //}
    }
}
