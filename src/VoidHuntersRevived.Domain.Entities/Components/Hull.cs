using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public sealed partial class Hull : IShipPartComponent
    {
        public Hull.Shape[] Shapes { get; set; } = Array.Empty<Hull.Shape>();
    }
}
