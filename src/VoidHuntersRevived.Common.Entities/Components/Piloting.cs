using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public class Piloting
    {
        public Entity Pilotable;

        public Piloting(Entity pilotable)
        {
            this.Pilotable = pilotable;
        }
    }
}
