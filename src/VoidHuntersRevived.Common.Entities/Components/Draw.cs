using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Configurations;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public class Draw
    {
        public readonly DrawConfiguration Configuration;

        public Draw(DrawConfiguration configuration)
        {
            this.Configuration = configuration;
        }
    }
}
