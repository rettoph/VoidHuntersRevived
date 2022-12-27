using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Implementations;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    [GuppyFilter<GameGuppy>()]
    public abstract class Simulation : Broker
    {
        public World World { get; }
        public abstract SimulationType Type { get; }
        public abstract AetherWorld Aether { get; }

        protected Simulation(IWorldProvider worldProvider, IFilteredProvider filteredProvider)
        {
            object? configuration = this.Type;

            this.World = worldProvider.Get(configuration);

            var subscribers = filteredProvider.Instances<ISubscriber>(configuration);
            foreach(ISubscriber subscriber in subscribers)
            {
                this.SubscribeAll(subscriber);
            }
        }
    }
}
