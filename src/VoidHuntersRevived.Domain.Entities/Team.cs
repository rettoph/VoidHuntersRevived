using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Entities
{
    internal class Team : ITeam
    {
        public TeamId Id { get; init; }

        public string Name { get; init; }

        public Color PrimaryColor { get; init; }

        public Color SecondaryColor { get; init; }
    }
}
