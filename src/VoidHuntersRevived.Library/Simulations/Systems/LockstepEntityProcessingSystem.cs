using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    public abstract class LockstepEntityProcessingSystem : EntitySystem, ISubscriber<Step>
    {
        protected LockstepEntityProcessingSystem(AspectBuilder aspectBuilder) : base(aspectBuilder)
        {
        }

        public virtual void Process(in Step message)
        {
            Begin();

            foreach (var entityId in ActiveEntities)
            {
                Process(message, entityId);
            }


            End();
        }

        public virtual void Begin() { }
        public abstract void Process(GameTime gameTime, int entityId);
        public virtual void End() { }
    }
}
