using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Extensions.System;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        private MaleConnectionNode _maleConnectionNode;

        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }

        #region Initialization Methods
        private void ConnectionNodes_Boot()
        {
            _maleConnectionNode = ActivatorUtilities.CreateInstance<MaleConnectionNode>(
                this.provider,
                this,
                _config.MaleConnectionNode.Z,
                new Vector2(_config.MaleConnectionNode.X, _config.MaleConnectionNode.Y));

            Int32 i = 0;
            this.FemaleConnectionNodes = _config.FemaleConnectionNodes
                .Select(fcn => ActivatorUtilities.CreateInstance<FemaleConnectionNode>(
                    this.provider,
                    this,
                    fcn.Z,
                    new Vector2(fcn.X, fcn.Y),
                    i++
                ))
                .ToArray();
        }
        #endregion

        /// <summary>
        /// Attempt to attatch the current ship-part
        /// to a target female connection node. This will
        /// automatically detatch from the ship-part's old
        /// connection if any exists.
        /// </summary>
        /// <param name="target"></param>
        public void AttatchTo(FemaleConnectionNode target)
        {
            _maleConnectionNode.AttatchTo(target);
        }

        protected internal void RemapConnectioNodes()
        {
            this.Dirty = true;
        }

        #region Network Methods
        private void ConnectionNodes_Read(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
            {

            }
        }

        private void ConnectionNodes_Write(NetOutgoingMessage om)
        {
            if(_maleConnectionNode.Target == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(_maleConnectionNode.Target.Parent.Id);
                om.Write(_maleConnectionNode.Target.Id);
            }
        }
        #endregion
    }
}
