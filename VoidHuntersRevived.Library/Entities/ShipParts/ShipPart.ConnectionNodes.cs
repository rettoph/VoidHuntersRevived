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
using VoidHuntersRevived.Library.Extensions.Lidgren;

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
                this.config.MaleConnectionNode.Rotation,
                this.config.MaleConnectionNode.Position);

            Int32 i = 0;
            this.FemaleConnectionNodes = this.config.FemaleConnectionNodes
                .Select(fcn => ActivatorUtilities.CreateInstance<FemaleConnectionNode>(
                    this.provider,
                    this,
                    fcn.Rotation,
                    fcn.Position,
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
        public Boolean TryAttatchTo(FemaleConnectionNode target)
        {
            // If the nodes are siblings...
            if (this.MaleConnectionNode.Parent == target.Parent)
                return false;

            // If the node is already connected to something else...
            if (this.MaleConnectionNode.Connected)
                return false;

            this.logger.LogInformation($"Attempting to attach ShipPart<{this.GetType().Name}>({this.Id}) to ShipPart<{target.Parent.GetType().Name}>({target.Parent.Id}) FemaleConnectionNode({target.Id})");
            this.MaleConnectionNode.AttatchTo(target);
            return true;
        }

        /// <summary>
        /// Detatch the current ship-part from
        /// whatever part it is currently attatched to
        /// if any
        /// </summary>
        public Boolean TryDetatchFrom()
        {
            // If the male node is not connected to anything...
            if (!this.MaleConnectionNode.Connected)
                return false;

            this.logger.LogInformation($"Attempting to detach ShipPart<{this.GetType().Name}>({this.Id}) from ShipPart<{this.MaleConnectionNode.Target?.Parent.GetType().Name}>({this.MaleConnectionNode.Target?.Parent.Id}) FemaleConnectionNode({this.MaleConnectionNode.Target?.Id})");
            this.MaleConnectionNode.DetatchFrom();
            return true;
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
        public void GetOpenFemaleConnectionNodes(ref List<FemaleConnectionNode> list)
        {
            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Target == null)
                    list.Add(female);
                else
                    female.Target.Parent.GetOpenFemaleConnectionNodes(ref list);
        }

        /// <summary>
        /// Return a list of all open female nodes
        /// </summary>
        public List<FemaleConnectionNode> GetOpenFemaleConnectionNodes()
        {
            var list = new List<FemaleConnectionNode>();

            this.GetOpenFemaleConnectionNodes(ref list);

            return list;
        }
        #endregion

        #region Network Methods
        private void ConnectionNodes_Read(NetIncomingMessage im)
        {
            this.ReadAttachmentData(im);
        }

        private void ConnectionNodes_Write(NetOutgoingMessage om)
        {
            this.WriteAttachmentData(om);   
        }

        public void ReadAttachmentData(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                this.TryAttatchTo(
                    im.ReadFemaleConnectionNode(this.entities));
        }

        public void WriteAttachmentData(NetOutgoingMessage om)
        {
            if (om.WriteIf(this.MaleConnectionNode.Connected))
                om.Write(this.MaleConnectionNode.Target as FemaleConnectionNode);
        }
        #endregion
    }
}
