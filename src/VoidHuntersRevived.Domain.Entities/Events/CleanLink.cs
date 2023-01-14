using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class CleanLink : IData
    {
        public readonly Linked Link;

        public CleanLink(Linked link)
        {
            this.Link = link;
        }
    }
}
