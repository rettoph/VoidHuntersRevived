using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class NodeSystem : ParallelEntityProcessingSystem
    {
        private static readonly AspectBuilder ShipPartLeafAspect = Aspect.All(new[]
        {
            typeof(Node),
            typeof(Jointable)
        });

        public NodeSystem(ISimulationService simulations) : base(simulations, ShipPartLeafAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            throw new NotImplementedException();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            throw new NotImplementedException();
        }
    }
}
