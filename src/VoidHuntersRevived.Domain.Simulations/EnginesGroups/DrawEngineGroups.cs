using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;

namespace VoidHuntersRevived.Domain.Simulations.EnginesGroups
{
    public class DrawEngineGroups : BulkEnginesGroups<IStepEngine<GameTime>, GameTime>
    {
        public DrawEngineGroups(IEnumerable<IStepEngine<GameTime>> engines) : base(engines)
        {
        }
    }
}
