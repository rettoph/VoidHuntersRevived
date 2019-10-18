using Guppy.Configurations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Client.Textures.Builders
{
    abstract class Builder
    {
        protected abstract Image Build(EntityConfiguration entity);

        public void TryBuild(String name, EntityConfiguration entity)
        {
            Console.WriteLine($"Building {name}...");

            var image = this.Build(entity);
            image.Save($"Sprites/{name.ToFileName()}.png", ImageFormat.Png);
        }
    }
}
