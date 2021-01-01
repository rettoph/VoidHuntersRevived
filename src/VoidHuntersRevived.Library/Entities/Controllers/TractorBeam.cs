using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;
using Guppy.Extensions.log4net;
using Guppy.Events.Delegates;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using Guppy.Lists;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// A tractor beam is a special controller contained within
    /// a Ship instance.
    /// 
    /// This will allow players to pick up ad interact with free floating
    /// pieces.
    /// </summary>
    public class TractorBeam : Controller
    {
        #region Enums
        [Flags]
        public enum ActionType { 
            None = 1,
            Select = 2,
            Deselect = 4,
            Attach = 8 | ActionType.Deselect
        }
        #endregion

        #region Structs
        /// <summary>
        /// Defines <see cref="TractorBeam"/> specific action data
        /// that contains required data to preform a desired <see cref="ActionType"/>.
        /// </summary>
        public struct Action {
            /// <summary>
            /// The <see cref="ActionType"/> this current <see cref="Action"/>
            /// is defining.
            /// </summary>
            public readonly ActionType Type;
            /// <summary>
            /// The target<see cref="ShipPart"/> in question this <see cref="Type"/>
            /// <see cref="ActionType"/> is to be preformed on.
            /// </summary>
            public readonly ShipPart TargetPart;
            /// <summary>
            /// The <see cref="ConnectionNode"/>, if any, this <see cref="ActionType"/> is to be
            /// preformed on. This is generally used to defined which node
            /// the <see cref="TargetPart"/> wishes to attach to when
            /// the <see cref="Type"/> is <see cref="ActionType.Attach"/>.
            /// </summary>
            public ConnectionNode TargetNode;

            public Action(ActionType type = ActionType.None, ShipPart target = default, ConnectionNode targetNode = default)
            {
                this.Type = type;
                this.TargetPart = target;
                this.TargetNode = targetNode;
            }
        }
        #endregion

        #region Private Fields
        private ChunkManager _chunks;
        private EntityList _entities;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The Ship that currently owns the tractor beam.
        /// </summary>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The currently selected target, if any
        /// </summary>
        public Chain Selected { get; private set; }

        /// <summary>
        /// The world position of the tractor beam
        /// </summary>
        public Vector2 Position => this.Ship.Target;
        #endregion

        #region Events & Delegates
        public event OnEventDelegate<TractorBeam, TractorBeam.Action> OnSelected;
        public event OnEventDelegate<TractorBeam, TractorBeam.Action> OnDeselected;
        public event OnEventDelegate<TractorBeam, TractorBeam.Action> OnAttached;
        public event OnEventDelegate<TractorBeam, TractorBeam.Action> OnAction;

        /// <summary>
        /// Indicates whether or not a attachment can be made between
        /// a given ShipPart and target ConnectionNode.
        /// </summary>
        public event ValidateEventDelegate<ShipPart, ConnectionNode> CanAttach;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);
            provider.Service(out _entities);

            this.UpdateOrder = 120;

            this.CanAttach += this.DefaultCanAttach;
        }

        protected override void Release()
        {
            base.Release();

            _chunks = null;
            _entities = null;

            this.CanAttach += this.DefaultCanAttach;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.chains.ForEach(p => p.TryDraw(gameTime));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.chains.ForEach(p => p.TryUpdate(gameTime));

            this.Align(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Auto position & update all selected ShipParts.
        /// </summary>
        private void Align(GameTime gameTime)
        {
            if (this.chains.Any())
            {
                var node = this.Ship.GetClosestOpenFemaleNode(this.Position);

                if(node == default(ConnectionNode))
                { // There is no available connection node.. just update the position directly...
                    this.chains.ForEach(chain =>
                    {
                        chain.Root.SetTransformIgnoreContacts(
                            position: this.Position - Vector2.Transform(chain.Root.Configuration.Centeroid, Matrix.CreateRotationZ(chain.Root.Rotation)),
                            angle: chain.Root.Rotation);

                        chain.TryUpdate(gameTime);
                    });
                }
                else
                { // There is an available connection node. Position the ship part to preview it...
                    this.chains.ForEach(chain =>
                    {
                        chain.Root.LinearVelocity = Vector2.Zero;
                        chain.Root.AngularVelocity = 0f;

                        node.TryPreview(chain.Root);

                        chain.TryUpdate(gameTime);
                    });
                }
            }
        }
        #endregion

        #region Controller Methods
        /// <summary>
        /// Attempt an incoming action, and return the result.
        /// </summary>
        /// <param name="action"></param>
        public TractorBeam.Action TryAction(TractorBeam.Action action)
        {
            TractorBeam.Action response = action;

            switch (action.Type)
            {
                case ActionType.Select:
                    response = this.TrySelect(action);
                    break;
                case ActionType.Deselect:
                case ActionType.Attach:
                    response = this.TryDeselect(action);
                    break;
            }

            this.logger.Verbose(() => $"Attempted TractorBeam.ActionType({action.Type}) on ShipPart({action.TargetPart?.Id}) and ended with TractorBeam.ActionType({response.Type})");
            this.OnAction?.Invoke(this, response);

            return response;
        }

        /// <summary>
        /// Simple helper method to determin if the recieved
        /// ShipPart may be selected by the tractor beam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean CanSelect(ShipPart target)
        {
            if (target?.Chain.Ship == this.Ship && !target.IsRoot)
                return true;

            return this.CanAdd(target?.Chain);
        }

        private TractorBeam.Action TrySelect(TractorBeam.Action action)
        {
            if (action.Type != ActionType.Select)
                throw new ArgumentException($"Unable to create selection, Invalid ActionType({action.Type}) recieved.");

            if (this.CanSelect(action.TargetPart))
            {
                action.TargetPart.MaleConnectionNode.TryDetach();
                this.TryAdd(action.TargetPart.Chain);
                this.OnSelected?.Invoke(this, action);

                return action;
            }

            return new TractorBeam.Action(ActionType.None, action.TargetPart);
        }

        protected override bool CanAdd(Chain chain)
            => chain?.Controller is ChunkManager;

        protected override void Add(Chain chain)
        {
            base.Add(chain);

            chain.Root.Items().ForEach(sp =>
            {
                sp.LinearVelocity = Vector2.Zero;
                sp.AngularVelocity = 0f;
            });


            if(this.Selected != default(Chain)) // Auto attempt a deselect if any...
                this.TryAction(new Action(ActionType.Deselect));

            this.Selected = chain;
        }

        /// <summary>
        /// Attempt to release the current selection if any
        /// </summary>
        /// <param name="attach">Whether or not an attachment should also be attempted...</param>
        /// <returns></returns>
        private TractorBeam.Action TryDeselect(TractorBeam.Action action)
        {
            if (!action.Type.HasFlag(TractorBeam.ActionType.Deselect))
                throw new ArgumentException($"Unable to create deselection, Invalid ActionType({action.Type}) recieved.");

            if (this.CanRemove(action.TargetPart?.Chain))
            {
                var old = this.Selected;
                this.OnDeselected?.Invoke(this, action);

                if (action.Type.HasFlag(TractorBeam.ActionType.Attach))
                {
                    return this.TryAttach(action);
                }
                else
                {
                    _chunks.TryAdd(old);
                    return action;
                }
            }

            return new Action(ActionType.None, this.Selected?.Root);
        }

        protected override void Remove(Chain chain)
        {
            base.Remove(chain);

            this.Selected = null;
        }

        /// <summary>
        /// Attempt to attach the current selected ship part to the recieved
        /// target node.
        /// </summary>
        /// <param name="shipPart"></param>
        /// <param name="target"></param>
        private TractorBeam.Action TryAttach(TractorBeam.Action action)
        {
            var node = action.TargetNode ?? this.Ship.GetClosestOpenFemaleNode(this.Position);

            if (this.CanAttach.Validate(action.TargetPart, node, false))
            { // If all the delegates allow the current attachment...
                action.TargetPart.MaleConnectionNode.TryAttach(node);
                action.TargetNode = node;
                this.OnAttached?.Invoke(this, action);

                return action;
            }
            else
            {
                _chunks.TryAdd(this.Selected);
                return new Action(ActionType.Deselect, action.TargetPart);
            }
        }
        /// <summary>
        /// Detect the best possible attachment node and attempt a connection.
        /// </summary>
        public TractorBeam.Action TryAttach(ShipPart shipPart)
            => this.TryAction(new TractorBeam.Action(ActionType.Attach, shipPart));

        private Boolean DefaultCanAttach(ShipPart shipPart, ConnectionNode target)
        {
            if (target == default(ConnectionNode))
                return false;
            if (target.Attached)
                return false;
            else if (shipPart.MaleConnectionNode.Attached)
                return false;
            else if (target.Parent == shipPart)
                return false;
            else if (!shipPart.IsRoot)
                return false;
            else if (target.Parent.Chain.Ship != this.Ship)
                return false;
            
            // By default, return true.
            return true;
        }
        #endregion

        #region Network Methods
        public void WriteAction(NetOutgoingMessage om, TractorBeam.Action action)
        {
            this.Ship.WriteTarget(om);

            om.Write(action.Type);
            om.Write(action.TargetPart, (m, e) =>
            {
                m.Write(action.TargetPart.Position);
                m.Write(action.TargetPart.Rotation);
            });
            om.Write(action.TargetNode);
        }

        public TractorBeam.Action ReadAction(NetIncomingMessage im)
        {
            this.Ship.ReadTarget(im);

            var request = new TractorBeam.Action(
                type: (TractorBeam.ActionType)im.ReadByte(),
                target: im.ReadEntity<ShipPart>(_entities, (m, sp) =>
                {
                    sp.SetTransformIgnoreContacts(
                        position: m.ReadVector2(),
                        angle: m.ReadSingle());
                }),
                targetNode: im.ReadConnectionNode(_entities));

            return this.TryAction(request);
        }
        #endregion
    }
}
