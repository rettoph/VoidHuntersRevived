using Guppy.ECS;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public class CreateEntity
    {
        public static readonly ParallelKey Noise = ParallelKey.From<CreateEntity>();

        public readonly ParallelKey? Key;
        public readonly EntityType Type;
        public readonly Action<Entity>? Factory;

        public CreateEntity(EntityType type, ParallelKey? key, Action<Entity>? factory)
        {
            this.Type = type;
            this.Key = key;
            this.Factory = factory;
        }
    }
}
