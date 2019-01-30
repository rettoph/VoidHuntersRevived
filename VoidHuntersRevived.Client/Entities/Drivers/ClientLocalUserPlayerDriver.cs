using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    public class ClientLocalUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;
        private Cursor _cursor;

        public ClientLocalUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var scene = this.Scene as ClientMainScene;

            _cursor = scene.Cursor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
