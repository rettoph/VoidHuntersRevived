using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Extensions
{
    public static class EntityExtensions
    {
        public static void PublishEvent(this Entity entity, IData data)
        {
            entity.Get<ISimulation>().PublishEvent(data);
        }

        public static void Input(this Entity entity, ParallelKey user, IData data)
        {
            entity.Get<ISimulation>().Input(user, data);
        }
    }
}
