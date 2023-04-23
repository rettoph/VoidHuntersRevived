using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface ITreeService
    {
        Tree MakeTree(Entity entity, Body body, Node? head);

        void AddNode(Node node, Tree tree);

        void RemoveNode(Node node, Tree tree);
    }
}
