using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class JointSystem : BasicSystem,
        ISubscriber<IEvent<CleanJointing>>
    {
        public void Process(in IEvent<CleanJointing> message)
        {
            var transformation = message.Data.Link.LocalTransformation;

            foreach(var joint in message.Data.Link.Joint.Linkable.Joints)
            {
                joint.LocalTransformation = joint.Configuration.Transformation * transformation;
            }
        }
    }
}
