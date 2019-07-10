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
using Microsoft.Extensions.Logging;

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
                this.config.MaleConnectionNode.Z,
                new Vector2(this.config.MaleConnectionNode.X, this.config.MaleConnectionNode.Y));

            Int32 i = 0;
            this.FemaleConnectionNodes = this.config.FemaleConnectionNodes
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
            this.logger.LogInformation($"Attempting to attatch ShipPart<{this.GetType().Name}>({this.Id}) to ShipPart<{target.Parent.GetType().Name}>({target.Parent.Id}) FemaleConnectionNode({target.Id})");

            target.Attatch(this);
        }

        /// <summary>
        /// Used to refresh a specific part's connection node mapping.
        /// When not deep, no changes will happen but the map event will be
        /// triggered. When shallow is false, all parts will be re-attatched
        /// to their current chain allowing for the growth and updating of specific
        /// chain objects.
        /// </summary>
        /// <param name="deep"></param>
        protected internal void RemapConnectioNodes(Boolean deep = true)
        {
            // Update the local translation matrix
            this.UpdateLocalTranslation();

            // Update the current shipparts chain placement
            this.UpdateChainPlacement();


            // Recursively remap all nodes in the chain, if requested.
            if (deep)
                foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                    if (female.Target != null)
                        female.Target.Parent.RemapConnectioNodes(deep);

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
