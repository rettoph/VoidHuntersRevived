using Guppy.Configurations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Client.Textures.TextureGenerators
{
    public abstract class TextureGenerator
    {
        protected abstract Image Generate(EntityConfiguration entity);

        public virtual void TryGenerate(String name, EntityConfiguration entity)
        {
            Console.WriteLine($"Building {name}...");

            var image = this.Generate(entity);
            image.Save($"Sprites/{name.ToFileName()}.png", ImageFormat.Png);
        }
    }
}
