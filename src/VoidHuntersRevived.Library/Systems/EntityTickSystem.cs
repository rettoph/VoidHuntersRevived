using Guppy.Attributes;
using Guppy.Common;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    public abstract class EntityTickSystem : EntitySystem, ISubscriber<Tick>
    {
        private ISetting<int> _tickSpeed;

        protected EntityTickSystem(ISettingProvider settings, AspectBuilder aspectBuilder) : base(aspectBuilder)
        {
            _tickSpeed = settings.Get<int>(SettingConstants.TickSpeed);
        }

        public void Process(in Tick message)
        {
            var elapsedSeconds = (float)_tickSpeed.Value / 1000f;

            this.Begin();

            foreach (var entityId in ActiveEntities)
            {
                this.Process(elapsedSeconds, entityId);
            }

            this.End();
        }

        public virtual void Begin() { }
        public abstract void Process(float elapsedSeconds, int entityId);
        public virtual void End() { }
    }
}
