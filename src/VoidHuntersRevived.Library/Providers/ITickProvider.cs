using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Providers
{
    public interface ITickProvider
    {
        void Update(GameTime gameTime);

        bool Next([MaybeNullWhen(false)] out Tick next);
    }
}
