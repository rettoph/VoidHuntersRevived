using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Common.Simulations.Extensions
{
    public static class EntityExtensions
    {
        public static void Enqueue(this Entity entity, IData data)
        {
            entity.Enqueue(entity.Get<Parallelable>().Key, data);
        }

        public static void Enqueue(this Entity entity, ParallelKey sender, IData data)
        {
            entity.Get<ISimulation>().Enqueue(sender, data);
        }
    }
}
