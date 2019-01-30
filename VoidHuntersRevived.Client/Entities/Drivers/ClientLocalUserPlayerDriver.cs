using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;

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

            _parent.TractorBeam.Body.Position = _cursor.Body.Position;

            switch (Mouse.GetState().RightButton)
            {
                case ButtonState.Released:
                    if (_parent.TractorBeam.SelectedEntity != null)
                    {
                        _parent.TractorBeam.TryRelease();
                        _parent.Dirty = true;
                    }
                    break;
                case ButtonState.Pressed:
                    if(_parent.TractorBeam.SelectedEntity == null && _cursor.Over is ITractorableEntity)
                    {
                        _parent.TractorBeam.TrySelect(_cursor.Over as ITractorableEntity);
                    }

                    _parent.Dirty = true;
                    break;
            }
        }

        public void Read(NetIncomingMessage im)
        {
            _parent.TractorBeam.Read(im);
        }

        public void Write(NetOutgoingMessage om)
        {
            _parent.TractorBeam.Write(om);
        }
    }
}
