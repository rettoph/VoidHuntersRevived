using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    public interface IStepProvider
    {
        void Update(GameTime gameTime);

        bool ShouldStep();
    }
}
