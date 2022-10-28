using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Providers
{
    public interface ITickProvider
    {
        void Update(GameTime gameTime);

        bool Ready();

        Tick Next();
    }
}
