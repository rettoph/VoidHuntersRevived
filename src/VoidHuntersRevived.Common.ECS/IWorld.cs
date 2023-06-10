using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS.Services;
using VoidHuntersRevived.Common.ECS.Systems;

namespace VoidHuntersRevived.Common.ECS
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
