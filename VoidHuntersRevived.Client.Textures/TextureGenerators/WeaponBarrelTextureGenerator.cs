using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.Configurations;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Client.Textures.Extensions.Farseer;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Client.Textures.TextureGenerators
{
    [IsTextureGenerator(typeof(Weapon))]
    public class WeaponBarrelTextureGenerator : TextureGenerator
    {
        protected override Image Generate(EntityConfiguration entity)
        {
            var config = entity.Data as WeaponConfiguration;
            return config.Barrel.ToImageDimensions().DrawShape(ShipPartTextureGenerator.Pen, ShipPartTextureGenerator.Brush);
        }

        public override void TryGenerate(string name, EntityConfiguration entity)
        {
            base.TryGenerate(name + ":barrel", entity);
        }
    }
}
