using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Services
{
    [GuppyFilter(typeof(GameGuppy))]
    public interface IStepService : IGameComponent, IUpdateable
    {
    }
}
