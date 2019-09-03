using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy.Utilities.Delegaters;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;

namespace GalacticFighters.Library.Utilities
{
    public class ActionDelegater : CustomDelegater<String, NetIncomingMessage>
    {
        #region Private Fields
        private NetworkEntity _entity;
        private GalacticFightersWorldScene _scene;
        #endregion

        #region Constructor
        public ActionDelegater(NetworkEntity entity, GalacticFightersWorldScene scene)
        {
            _entity = entity;
            _scene = scene;
        }
        #endregion

        #region Creatae Methods
        public NetOutgoingMessage Create(String type)
        {
            var om = _scene.Group.CreateMessage("action");
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }
        #endregion
    }
}
