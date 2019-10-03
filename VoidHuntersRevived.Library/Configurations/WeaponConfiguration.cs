using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Configurations
{
    public class WeaponConfiguration : ShipPartConfiguration
    {
        public Vertices Barrel { get; private set; }
        public Vector2 BodyAnchor { get; private set; }
        public Vector2 BarrelAnchor { get; private set; }

        public Single Range { get; private set; }

        public WeaponConfiguration()
        {
        }

        public WeaponConfiguration(List<Vertices> vertices, ConnectionNodeConfiguration maleConnectionNode, ConnectionNodeConfiguration[] femaleConnectionNodes = null) : base(vertices, maleConnectionNode, femaleConnectionNodes)
        {
        }

        public WeaponConfiguration(Vertices vertices, ConnectionNodeConfiguration maleConnectionNode, ConnectionNodeConfiguration[] femaleConnectionNodes = null) : base(vertices, maleConnectionNode, femaleConnectionNodes)
        {
        }

        public void AddBarrel(Vertices barrel, Vector2 bodyAnchor, Vector2 barrelAnchor = default(Vector2), Single range = 1.5f)
        {
            this.Barrel = barrel;
            this.BodyAnchor = bodyAnchor;
            this.BarrelAnchor = barrelAnchor;
            this.Range = range;
        }
    }
}
