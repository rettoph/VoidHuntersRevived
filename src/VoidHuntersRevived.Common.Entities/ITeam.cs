using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeam
    {
        TeamId Id { get; }
        string Name { get; }
        Color Color { get; }
    }
}
