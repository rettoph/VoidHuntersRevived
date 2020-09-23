using FarseerPhysics.Dynamics;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;
using Guppy.IO.Extensions.log4net;

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
        #region Static Attributes
        private static GameTime EmptyGameTime { get; set; } = new GameTime();
        #endregion

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
        public struct Action {
            public readonly ActionType Type;
            public readonly ShipPart Target;

            public Action(ActionType type = ActionType.None, ShipPart shipPart = default(ShipPart))
            {
                this.Type = type;
                this.Target = shipPart;
            }
        }
        #endregion

        #region Private Fields
        private ChunkManager _chunks;
        private ILog _logger;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The Ship that currently owns the tractor beam.
        /// </summary>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The currently selected target, if any
        /// </summary>
        public ShipPart Selected { get; private set; }

        /// <summary>
        /// The world position of the tractor beam
        /// </summary>
        public Vector2 Position { get; set; }
        #endregion

        #region Events & Delegates
        public event GuppyEventHandler<TractorBeam, TractorBeam.Action> OnSelected;
        public event GuppyEventHandler<TractorBeam, TractorBeam.Action> OnDeselected;
        public event GuppyEventHandler<TractorBeam, TractorBeam.Action> OnAttached;
        public event GuppyEventHandler<TractorBeam, TractorBeam.Action> OnAction;

        public delegate Boolean CanAttachDelegate(ShipPart shipPart, ConnectionNode target);

        /// <summary>
        /// Indicates whether or not a attachment can be made between
        /// a given ShipPart and target ConnectionNode.
        /// </summary>
        public CanAttachDelegate CanAttach;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);
            provider.Service(out _logger);

            this.Authorization = GameAuthorization.Full;

            this.CanAttach += this.DefaultCanAttach;
        }

        protected override void Release()
        {
            base.Release();

            this.CanAttach += this.DefaultCanAttach;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.parts.ForEach(p => p.TryDraw(gameTime));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Position = this.Ship.WorldTarget;
            this.Align(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Auto position & update all selected ShipParts.
        /// </summary>
        private void Align(GameTime gameTime)
        {
            if (this.parts.Any())
            {
                var node = this.Ship.GetClosestOpenFemaleNode(this.Position);

                if(node == default(ConnectionNode))
                { // There is no available connection node.. just update the position directly...
                    this.parts.ForEach(p =>
                    {
                        p.SetTransformIgnoreContacts(
                            position: this.Position - Vector2.Transform(p.Configuration.Centeroid, Matrix.CreateRotationZ(p.Rotation)),
                            angle: p.Rotation);

                        p.TryUpdate(gameTime);
                    });
                }
                else
                { // There is an available connection node. Position the ship part to preview it...
                    // Rather than creating the attachment, we just want to move the selection
                    // so that a user can preview what it would look like when attached.
                    var rotation = node.WorldRotation - this.Selected.MaleConnectionNode.LocalRotation;
                    
                    this.parts.ForEach(p =>
                    {
                        p.SetTransformIgnoreContacts(
                            position: node.WorldPosition - Vector2.Transform(this.Selected.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(rotation)),
                            angle: rotation);

                        p.TryUpdate(gameTime);
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

            _logger.Verbose(() => $"Attempted TractorBeam.ActionType({action.Type}) on ShipPart({action.Target?.Id}) and ended with TractorBeam.ActionType({response.Type})");
            this.OnAction?.Invoke(this, response);

            return response;
        }

        /// <summary>
        /// Simple helper method to determin if the recieved
        /// ShipPart may be selected by the tractor beam.
        /// </summary>
        /// <param name="shipPart"></param>
        /// <returns></returns>
        public Boolean CanSelect(ShipPart shipPart)
            => this.CanAdd(shipPart);

        /// <summary>
        /// Attempt to grab a given ShipPart within the tractor beam.
        /// </summary>
        /// <param name="shipPart"></param>
        public TractorBeam.Action TrySelect(ShipPart shipPart)
            => this.TryAction(new TractorBeam.Action(ActionType.Select, shipPart));
        private TractorBeam.Action TrySelect(TractorBeam.Action action)
        {
            if (action.Type != ActionType.Select)
                throw new ArgumentException($"Unable to create selection, Invalid ActionType({action.Type}) recieved.");

            if (this.CanAdd(action.Target))
            {
                action.Target.MaleConnectionNode.TryDetach();
                this.Add(action.Target);
                this.TryUpdate(TractorBeam.EmptyGameTime);
                this.OnSelected?.Invoke(this, action);

                return action;
            }

            return new TractorBeam.Action(ActionType.None, action.Target);
        }

        protected override bool CanAdd(ShipPart shipPart)
            => shipPart != default(ShipPart) && ((shipPart.IsRoot && shipPart.Controller is ChunkManager) || (!shipPart.IsRoot && shipPart.Root.Ship == this.Ship));

        protected override void Add(ShipPart shipPart)
        {
            base.Add(shipPart);

            shipPart.LinearVelocity = Vector2.Zero;
            shipPart.AngularVelocity = 0f;

            if(this.Selected != default(ShipPart)) // Auto attempt a deselect if any...
                this.TryDeselect();

            this.Selected = shipPart;
        }

        /// <summary>
        /// Attempt to release the current selection if any
        /// </summary>
        /// <param name="attach">Whether or not an attachment should also be attempted...</param>
        /// <returns></returns>
        public TractorBeam.Action TryDeselect(Boolean attach = false)
            => this.TryAction(new Action(attach ? ActionType.Attach : ActionType.Deselect, this.Selected));
        private TractorBeam.Action TryDeselect(TractorBeam.Action action)
        {
            if (!action.Type.HasFlag(TractorBeam.ActionType.Deselect))
                throw new ArgumentException($"Unable to create deselection, Invalid ActionType({action.Type}) recieved.");

            if (this.CanRemove(action.Target))
            {
                this.TryUpdate(TractorBeam.EmptyGameTime);

                var old = this.Selected;
                _chunks.TryAdd(old);
                this.OnDeselected?.Invoke(this, action);

                if (action.Type.HasFlag(TractorBeam.ActionType.Attach))
                    return this.TryAttach(action);
                else
                    return action;
            }

            return new Action(ActionType.None, this.Selected);
        }

        protected override void Remove(ShipPart shipPart)
        {
            base.Remove(shipPart);

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
            var node = this.Ship.GetClosestOpenFemaleNode(this.Position);

            foreach (CanAttachDelegate d in this.CanAttach.GetInvocationList())
                if (!d(action.Target, node))
                    return new Action(ActionType.Deselect, action.Target);

            // If all the delegates allow the current attachment...
            action.Target.MaleConnectionNode.TryAttach(node);

            if(action.Type != ActionType.None) // Invoke the action event as needed...
                this.OnAttached?.Invoke(this, action);

            return action;
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
            else if (target.Parent.Root.Ship != this.Ship)
                return false;
            
            // By default, return true.
            return true;
        }
        #endregion
    }
}
