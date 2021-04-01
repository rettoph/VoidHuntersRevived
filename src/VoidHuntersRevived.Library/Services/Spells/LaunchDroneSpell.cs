using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Services.Spells
{
    /// <summary>
    /// Primary class responsible for launching all
    /// drone instances.
    /// </summary>
    public class LaunchDroneSpell : Spell
    {
        #region Public Properties
        public Vector2 Position { get; internal set; }
        public Single Rotation { get; internal set; }
        public Single MaxAge { get; internal set; }
        public String Type { get; internal set; }
        public Team Team { get; internal set; }
        #endregion
    }
}
