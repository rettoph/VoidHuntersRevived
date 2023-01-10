﻿using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public class Piloting
    {
        public Pilotable Pilotable;

        public Piloting(Entity pilotable)
        {
            this.Pilotable = pilotable.Get<Pilotable>();
        }
    }
}
