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
    public abstract class LockstepUpdateSystem : ISystem, ISubscriber<Step>
    {
        protected LockstepUpdateSystem()
        {
        }

        public virtual void Initialize(World world)
        {
            // throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            // throw new NotImplementedException();
        }

        public virtual void Process(in Step message)
        {
            Update(message);
        }

        protected abstract void Update(GameTime gameTime);
    }
}
