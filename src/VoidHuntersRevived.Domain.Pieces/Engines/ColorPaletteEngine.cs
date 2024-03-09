using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class ColorPaletteEngine : BasicEngine, IReactOnAddEx<ColorPalette>
    {
        private readonly IEntityService _entities;

        public ColorPaletteEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<ColorPalette> entities, ExclusiveGroupStruct groupID)
        {
            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {

            }
        }
    }
}
