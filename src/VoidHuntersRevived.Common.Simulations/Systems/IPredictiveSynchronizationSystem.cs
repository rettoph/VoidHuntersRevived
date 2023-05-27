using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IPredictiveSynchronizationSystem : ISystem
    {
        void Synchronize(ISimulation predctive, ISimulation lockstep, GameTime gameTime, Fix64 damping);
    }
}
