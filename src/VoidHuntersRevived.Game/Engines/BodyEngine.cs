using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Engines
{
    internal sealed class BodyEngine : BasicEngine,
        IReactOnAddEx<Body>
    {
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Body> entities, ExclusiveGroupStruct groupID)
        {
            throw new NotImplementedException();
        }
    }
}
