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
        #region Private Fields
        
        #endregion

        #region Public Attributes
        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }
        public MaleConnectionNode MaleConnectionNode { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ShipPart> OnConnectionNodesRemapped;
        #endregion

        #region Initialization Methods
        private void ConnectionNodes_Boot()
        {
            MaleConnectionNode = ActivatorUtilities.CreateInstance<MaleConnectionNode>(
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

        #region Utility Methods
        /// <summary>
        /// Attempt to attatch the current ship-part
        /// to a target female connection node. This will
        /// automatically detatch from the ship-part's old
        /// connection if any exists.
        /// </summary>
        /// <param name="target"></param>
        public void AttatchTo(FemaleConnectionNode target)
        {
            MaleConnectionNode.AttatchTo(target);
        }

        /// <summary>
        /// Used to alert the current part and its entire chain
        /// to remap the self contained parts.
        /// </summary>
        protected internal void RemapConnectioNodes()
        {
            // Mark the current part as dirty...
            this.Dirty = true;

            // Trigger the event
            this.OnConnectionNodesRemapped?.Invoke(this, this);
        }


        /// <summary>
        /// Recursively update the input list of all 
        /// available female connection nodes
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void GetOpenFemaleNodes(ref List<FemaleConnectionNode> list)
        {
            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Target == null)
                    list.Add(female);
                else
                    female.Target.Parent.GetOpenFemaleNodes(ref list);
        }

        /// <summary>
        /// Return a list of all open female nodes
        /// </summary>
        public List<FemaleConnectionNode> GetOpenFemaleConnectionNodes()
        {
            var list = new List<FemaleConnectionNode>();

            this.GetOpenFemaleNodes(ref list);

            return list;
        }
        #endregion

        #region Network Methods
        private void ConnectionNodes_Read(NetIncomingMessage im)
        {
            //
        }

        private void ConnectionNodes_Write(NetOutgoingMessage om)
        {
            //   
        }
        #endregion
    }
}
