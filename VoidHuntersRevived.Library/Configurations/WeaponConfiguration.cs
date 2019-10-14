using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public class WeaponConfiguration : ShipPartConfiguration
    {
        public Vertices Barrel { get; private set; }
        public Vector2 BodyAnchor { get; private set; }
        public Vector2 BarrelAnchor { get; private set; }
        public Single Range { get; private set; }

        public Single FireRate { get; private set; }

        public WeaponConfiguration(Single fireRate)
        {
            this.FireRate = fireRate;
        }

        public WeaponConfiguration(Single fireRate, List<Vertices> vertices, ConnectionNodeConfiguration maleConnectionNode, ConnectionNodeConfiguration[] femaleConnectionNodes = null) : base(vertices, maleConnectionNode, femaleConnectionNodes)
        {
            this.FireRate = fireRate;
        }

        public WeaponConfiguration(Single fireRate, Vertices vertices, ConnectionNodeConfiguration maleConnectionNode, ConnectionNodeConfiguration[] femaleConnectionNodes = null) : base(vertices, maleConnectionNode, femaleConnectionNodes)
        {
            this.FireRate = fireRate;
        }

        public void AddBarrel(Vertices barrel, Vector2 bodyAnchor, Vector2 barrelAnchor = default(Vector2), Single range = 2f)
        {
            this.Barrel = barrel;
            this.BodyAnchor = bodyAnchor;
            this.BarrelAnchor = barrelAnchor;
            this.Range = range;
        }
    }
}
