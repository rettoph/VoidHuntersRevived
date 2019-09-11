using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Players
{
    /// <summary>
    /// Players represent objects that can preform actions within ships.
    /// A player will always contain a ship
    /// </summary>
    public abstract class Player : NetworkEntity
    {
        #region Protected Attributes
        protected GalacticFightersWorldScene scene { get; private set; }
        #endregion

        #region Public Attributes
        public abstract String Name { get; }
        #endregion

        #region Constructor
        public Player(GalacticFightersWorldScene scene)
        {
            this.scene = scene;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Automatically add the current player instance to the scenes player collection.
            this.scene.Players.Add(this);
        }
        #endregion
    }
}
