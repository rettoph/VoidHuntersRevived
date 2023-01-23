using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public sealed class CleanJointed : IData
    {
        public enum Statuses
        {
            Create,
            Destroy
        };

        public readonly Jointed Jointed;
        public readonly Statuses Status;

        public CleanJointed(Jointed jointed, Statuses status)
        {
            this.Jointed = jointed;
            this.Status = status;
        }
    }
}
