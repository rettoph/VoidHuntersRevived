using Guppy.Common;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Systems.LockstepSimulation
{
    internal sealed class AetherSystem : ISystem, ISubscriber<Step>
    {
        private readonly AetherWorld _aether;

        public AetherSystem(AetherWorld aether)
        {
            _aether = aether;
        }

        public void Initialize(World world)
        {
            // throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public void Process(in Step message)
        {
            _aether.Step(message.Interval);
        }
    }
}
