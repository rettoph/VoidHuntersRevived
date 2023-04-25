using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public class CreateEntity : Input
    {
        public readonly ParallelKey Key;

        public CreateEntity(ParallelKey key)
        {
            this.Key = key;
        }
    }
}
