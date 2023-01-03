using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Systems
{
    public interface IUpdateSimulationSystem : ISystem
    {
        void Update(ISimulation simulation, GameTime gameTime);
    }
}
