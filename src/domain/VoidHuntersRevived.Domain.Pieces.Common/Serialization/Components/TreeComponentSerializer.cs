using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
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
