using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class BodyTreeEngine : BasicEngine, 
        IStepEngine<Step>
    {
        private readonly IEntityService _entities;

        public BodyTreeEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public string name { get; } = nameof(BodyTreeEngine);

        public void Step(in Step _param)
        {
            foreach (var ((bodies, trees, count), _) in _entities.QueryEntities<Location, Tree>())
            {
                for (int i = 0; i < count; i++)
                {
                    trees[i].Transformation = FixMatrix.CreateRotationZ(bodies[i].Rotation) * FixMatrix.CreateTranslation(bodies[i].Position.X, bodies[i].Position.Y, Fix64.Zero);
                }
            }
        }
    }
}
