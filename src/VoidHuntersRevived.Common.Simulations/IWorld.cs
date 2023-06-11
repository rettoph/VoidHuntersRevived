using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IWorld : IDisposable
    {
        IEntityTypeService Types { get; }
        IEntityService Entities { get; }
        IComponentService Components { get; }
        ISystem[] Systems { get; }

        void Initialize();

        void Update(GameTime gameTime);
    }
}
