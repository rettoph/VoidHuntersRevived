using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Physics;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : BasicSystem, ISimulationUpdateSystem
    {
        public SpaceSystem() : base()
        {
        }

        public void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            // throw new NotImplementedException();
        }

        public void Initialize(ISimulation simulation)
        {
            // throw new NotImplementedException();
        }

        public void Update(ISimulation simulation, GameTime gameTime)
        {
            simulation.Space.Step(gameTime.ElapsedGameTime);
        }
    }
}
