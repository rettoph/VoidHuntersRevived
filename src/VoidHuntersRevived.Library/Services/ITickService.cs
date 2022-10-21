using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Models;

namespace VoidHuntersRevived.Library.Services
{
    public interface ITickService
    {
        public IEnumerable<Tick> History { get; }

        public Tick Current { get; }

        void Update(GameTime gameTime);
    }
}
