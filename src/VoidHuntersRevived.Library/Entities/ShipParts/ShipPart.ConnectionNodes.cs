﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using Guppy.Lists;
using Guppy.Extensions.log4net;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial implementation of ShipPart that
    /// specializes on all things related to connection
    /// nodes.
    /// </summary>
    public partial class ShipPart
    {
        #region Public Attributes
        /// <summary>
        /// The parts main mail connection node. All ShipParts must contain a male node.
        /// </summary>
        public ConnectionNode MaleConnectionNode { get; private set; }

        /// <summary>
        /// A list of the ShipPart's female nodes. This may be empty if there are no female nodes.
        /// </summary>
        public ConnectionNode[] FemaleConnectionNodes { get; private set; }

        /// <summary>
        /// A list of all children downstream connected directly to
        /// the current ShipPart instance.
        /// </summary>
        public IEnumerable<ShipPart> Children
        {
            get
            {
                foreach (ConnectionNode female in this.FemaleConnectionNodes)
                    if (female.Attached)
                        yield return female.Target.Parent;
            }
        }
        #endregion

        #region Lifecycle Methods
        private void ConnectionNode_Initialize(ServiceProvider provider)
        {
            // Create new connection nodes for the internal element
            this.MaleConnectionNode = ConnectionNode.Build(provider, this.Context.MaleConnectionNode, this, -1);
            this.FemaleConnectionNodes = this.Context.FemaleConnectionNodes.Select((f, i) =>
                ConnectionNode.Build(provider, f, this, i)).ToArray();
        }

        private void ConnectionNode_PreRelease()
        {
            this.MaleConnectionNode.TryRelease();
            this.FemaleConnectionNodes.ForEach(f =>
            {
                if (f.Attached)
                    f.Target.TryDetach();

                f.TryRelease();
            });
            this.FemaleConnectionNodes = new ConnectionNode[0];
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Populate a recieved list with all open female nodes
        /// within the current part and its children.
        /// </summary>
        /// <param name="list"></param>
        public void GetOpenFemaleConnectionNodes(ref IList<ConnectionNode> list)
        {
            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetOpenFemaleConnectionNodes(ref list);
                else
                    list.Add(female);
        }

        /// <summary>
        /// Populate the recieved list with all items
        /// down the current <see cref="ShipPart"/>'s 
        /// chain.
        /// </summary>
        /// <param name="output"></param>
        public void Items(ref IList<ShipPart> output, Func<ShipPart, Boolean> filter = null)
        {
            if(filter?.Invoke(this) ?? true)
                output.Add(this);

            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.Items(ref output, filter);
        }

        public IEnumerable<ShipPart> Items(Func<ShipPart, Boolean> filter = null)
        {
            IList<ShipPart> temp = new List<ShipPart>();
            this.Items(ref temp, filter);

            return temp;
        }
        #endregion
    }
}
