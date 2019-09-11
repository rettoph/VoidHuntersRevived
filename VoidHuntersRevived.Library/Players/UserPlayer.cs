using GalacticFighters.Library.Scenes;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Players
{
    public sealed class UserPlayer : Player
    {
        #region Private Fields
        private GalacticFightersWorldScene _scene;
        #endregion

        #region Public Attributes
        public User User { get; set; }

        public override String Name { get { return this.User.Name; } }
        #endregion

        #region Constructor
        public UserPlayer(GalacticFightersWorldScene scene)
        {
            _scene = scene;
        }
        #endregion

        #region Network Methods
        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            if (om.WriteExists(this.User))
                om.Write(this.User.Id);
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            if (im.ReadBoolean())
                this.User = _scene.Group.Users.GetById(im.ReadGuid());
        }
        #endregion
    }
}
