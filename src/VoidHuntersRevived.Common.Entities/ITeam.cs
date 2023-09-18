﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeam : IEntityResource<ITeam>
    {
        string Name { get; }
        Color PrimaryColor { get; }
        Color SecondaryColor { get; }
    }
}
