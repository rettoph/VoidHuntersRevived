using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Thrusters are ShipParts that can be used to apply thrust to a ship when
    /// controlled by a Player instance.
    /// </summary>
    public class Thruster : ShipPart
    {
        #region Public Attributes
        public ThrusterData ThrusterData { get; private set; }
        #endregion

        #region Constructors
        public Thruster(EntityInfo info, IGame game, string driverHandle = "entity:driver:ship_part") : base(info, game, driverHandle)
        {
            this.ThrusterData = info.Data as ThrusterData;
        }

        public Thruster(long id, EntityInfo info, IGame game, SpriteBatch spriteBatch, string driverHandle = "entity:driver:ship_part") : base(id, info, game, spriteBatch, driverHandle)
        {
            this.ThrusterData = info.Data as ThrusterData;
        }
        #endregion
    }
}
