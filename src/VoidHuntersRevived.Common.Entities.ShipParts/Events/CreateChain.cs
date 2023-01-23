using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public class CreateChain : CreateEntity
    {
        public readonly Entity? Head;
        public readonly Vector2 Position;
        public readonly float Rotation;

        public CreateChain(
            ParallelKey key, 
            Entity? head, 
            Vector2 position, 
            float rotation) : base(key)
        {
            this.Head = head;
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
