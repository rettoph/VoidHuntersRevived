using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;
using Guppy.IO.Extensions.log4net;
using Guppy.Lists;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Library.Entities
{
    public partial class Ship : NetworkEntity
    {
        #region Private Fields
        /// <summary>
        /// The primary internal controller to manage all internal
        /// Ship components. This is generally not interacted with
        /// directly.
        /// </summary>
        private ShipController _controller;

        /// <summary>
        /// The ships current bridge ship part (if any).
        /// </summary>
        private ShipPart _bridge;

        /// <summary>
        /// A direct reference to the player, if any, that this ship 
        /// belongs to.
        /// </summary>
        private Player _player;

        /// <summary>
        /// An internal list of entities used
        /// for entity creation and lookup.
        /// </summary>
        private EntityList _entities;

        /// <summary>
        /// Determin whether or not the current ship
        /// is in the  act of firing.
        /// </summary>
        private Boolean _firing;

        /// <summary>
        /// A list of all female nodes current open
        /// within the Ship's Bridge's chain. This is
        /// automatically updated when <see cref="Clean"/>
        /// is invoked.
        /// </summary>
        private IList<ConnectionNode> _openFemaleNodes;

        /// <summary>
        /// A list of all weapons contained within the
        /// Ship's Bridge's chain. This is
        /// automatically updated when <see cref="Clean"/>
        /// is invoked.
        /// </summary>
        private List<Weapon> _weapons;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current player managing the ship (if any).
        /// Updating this value will automatically invoked
        /// the <see cref="OnPlayerChanged"/> event.
        /// </summary>
        public Player Player
        {
            get => _player;
            set => this.OnPlayerChanged.InvokeIfChanged(value != _player, this, ref _player, value);
        }

        /// <summary>
        /// The ships current bridge ship part (if any).
        /// All new Bridge values must first be approved
        /// by the <see cref="OnValidateBridge"/> validator. 
        /// When denied, an exception will be thrown. When
        /// approved the private <see cref="_bridge"/> value
        /// will be updated an the <see cref="OnBridgeChanged"/>
        /// event will be invoked.
        /// </summary>
        public ShipPart Bridge
        {
            get => _bridge;
            set
            {
                if(value != _bridge)
                { // First, only proceed if the value has changed.
                    if (this.OnValidateBridge.Validate(this, value, false)) // Validate the incoming vlaue...
                        this.OnBridgeChanged?.InvokeIfChanged(true, this, ref _bridge, value); // When successful, call the event...
                    else // When not successful, throw an exception...
                        throw new Exception("Invalid bridge value.");
                }
                
            }
        }

        /// <summary>
        /// The Ships personal TractorBeam instance. All world
        /// to piece interaction takes place within.
        /// </summary>
        public TractorBeam TractorBeam { get; private set; }

        /// <summary>
        /// A simple inermerable of all female nodes current open
        /// within the Ship's Bridge's chain. Value based on the
        /// private <see cref="_openFemaleNodes"/> field.
        /// </summary>
        public IEnumerable<ConnectionNode> OpenFemaleNodes => _openFemaleNodes;

        /// <summary>
        /// The current primary color of the Ship
        /// when at full health. This is gnerally
        /// based on the team value.
        /// </summary>
        public Color Color => Color.Cyan;

        /// <summary>
        /// The current Ship's name, if any. This is automatically
        /// imported with <see cref="Import(System.IO.Stream)"/>
        /// invocations & exported with <see cref="Export"/>
        /// invocations.
        /// </summary>
        public String Title { get; set; } = "Unnamed Ship";

        /// <summary>
        /// Determin whether or not the current ship
        /// is in the  act of firing. Updating this value will automatically invoked
        /// the <see cref="OnFiringChanged"/> event.
        /// </summary>
        public Boolean Firing
        {
            get => _firing;
            set => this.OnFiringChanged.InvokeIfChanged(value != _firing, this, ref _firing, value);
        }

        /// <summary>
        /// A readonly collection of all weapons contained within the
        /// Ship's Bridge's chain. Value based on the
        /// private <see cref="_weapons"/> field.
        /// </summary>
        public IReadOnlyCollection<Weapon> Weapons => _weapons;
        #endregion

        #region Events
        /// <summary>
        /// Automatically invoked when the <see cref="Bridge"/> property is updated.
        /// </summary>
        public event OnChangedEventDelegate<Ship, ShipPart> OnBridgeChanged;

        /// <summary>
        /// Automatically invoked when the <see cref="Player"/> property is updated.
        /// </summary>
        public event OnChangedEventDelegate<Ship, Player> OnPlayerChanged;

        /// <summary>
        /// Automatically invoked when the <see cref="Firing"/> property is updated.
        /// </summary>
        public event OnEventDelegate<Ship, Boolean> OnFiringChanged;

        /// <summary>
        /// Invoked whenever the Bridge's chain changes in any
        /// way. This iscludes parts being added & removed.
        /// </summary>
        public event OnEventDelegate<Ship> OnCleaned;

        /// <summary>
        /// Simple validator to determin if an incoming <see cref="Bridge"/>
        /// value is valid before updating with the new value.
        /// 
        /// When false, an exception will be thrown if trying to update 
        /// the <see cref="Bridge"/>.
        /// </summary>
        public event ValidateEventDelegate<Ship, ShipPart> OnValidateBridge;
        #endregion

        #region Lifecycle Methods
        /// <inheritdoc />
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            // Create partial classes...
            this.Network_Create(provider);

            // Add required event handlers...
            this.OnBridgeChanged += Ship.HandleBridgeChanged;
            this.OnValidateBridge += Ship.ValidateBridge;
        }

        /// <inheritdoc />
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Initialize the internal values...
            _openFemaleNodes = new List<ConnectionNode>();
            _weapons = new List<Weapon>();

            // Load required services...
            provider.Service(out _controller);
            provider.Service(out _entities);

            // Create a new tractor beam instance for this ship...
            this.TractorBeam = provider.GetService<EntityList>().Create<TractorBeam>((t, p, c) => t.Ship = this);
        }

        /// <inheritdoc />
        protected override void Dispose()
        {
            base.Dispose();

            // Dispose partial classes...
            this.Network_Dispose();

            // Remove required event handlers...
            this.OnBridgeChanged -= Ship.HandleBridgeChanged;
            this.OnValidateBridge -= Ship.ValidateBridge;
        }
        #endregion

        #region Frame Methods
        /// <inheritdoc />
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal controller...
            _controller.TryUpdate(gameTime);                
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw the internal controller...
            _controller.TryDraw(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// When the Ship's chain is changed in any way we
        /// will refresh the internal known properties here.
        /// </summary>
        private void Clean()
        {
            lock (_openFemaleNodes)
            {
                // Refresh the list of open female nodes...
                _openFemaleNodes.Clear();
                this.Bridge?.GetOpenFemaleConnectionNodes(ref _openFemaleNodes);
            }

            lock (_weapons)
            {
                // Refresh the internal list of weapons...
                _weapons.Clear();

                if (this.Bridge is Weapon w)
                    _weapons.Add(w);

                _weapons.AddRange(this.Bridge.Children.Where(c => c is Weapon).Select(c => c as Weapon));
            }

            // Invoke the cleaned event now that all required instances have been cleaned.
            this.OnCleaned?.Invoke(this);
        }

        /// <summary>
        /// Get the clostest available ConnectionNode to a recieved 
        /// WorldPosition
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ConnectionNode GetClosestOpenFemaleNode(Vector2 position, Single range = 0.75f)
        {
            lock (_openFemaleNodes)
            {
                ConnectionNode closest = default(ConnectionNode);
                Single distance = range;

                foreach (ConnectionNode node in _openFemaleNodes)
                    if (distance != (distance = Math.Min(Vector2.Distance(node.WorldPosition, position), distance)))
                        closest = node;

                return closest;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler that will simply
        /// call the ship's internal <see cref="Clean"/>
        /// method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private static void HandleBridgeChainChanged(Chain sender, ShipPart arg)
            => sender.Ship.Clean();

        /// <summary>
        /// Invoked when a Ship's bridge is changed.
        /// This will map out all required chain events
        /// and ensure that the internal <see cref="Clean"/>
        /// method is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private static void HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if (old != null)
            { // Remove old bridge...
                old.Chain.OnShipPartAdded -= Ship.HandleBridgeChainChanged;
                old.Chain.OnShipPartRemoved -= Ship.HandleBridgeChainChanged;
                old.Chain.Ship = null;
            }

            // Simple log message
            sender.log.Verbose($"Ship({sender.Id}) => Setting bridge to ShipPart<{value.GetType().Name}>({value.Id})");

            if (value != null)
            { // Setup the new bridge...
                sender._controller.TryAdd(value.Chain);
                value.Chain.Ship = sender;
                value.Chain.OnShipPartAdded += Ship.HandleBridgeChainChanged;
                value.Chain.OnShipPartRemoved += Ship.HandleBridgeChainChanged;
            }

            // Clean the ship before releasing...
            sender.Clean();
        }

        /// <summary>
        /// Check whether or not an incoming
        /// bridge value is valid or not.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="bridge"></param>
        /// <returns></returns>
        private static bool ValidateBridge(Ship ship, ShipPart bridge)
        {
            // Instant false
            if (!bridge.IsRoot)
                return false;
            else if (bridge?.Chain.Ship != default)
                return false;

            // Instant true
            if (bridge == default(ShipPart))
                return true;
            else if (bridge.IsRoot)
                return true;

            // Default false
            return false;
        }
        #endregion
    }
}
