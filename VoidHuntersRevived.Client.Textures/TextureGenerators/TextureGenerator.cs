using Guppy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Client.Textures.TextureGenerators
{
    public abstract class TextureGenerator
    {
        protected abstract Image Generate(ShipPart entity);

        public virtual void TryGenerate(String name, ShipPart entity)
        {
            Console.WriteLine($"Building {name}...");

            var image = this.Generate(entity);
            image.Save($"Sprites/{name.ToFileName()}.png", ImageFormat.Png);
        }
    }
}
