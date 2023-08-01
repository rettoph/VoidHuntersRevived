using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IVisibleRenderingService
    {
        void BeginFill();
        void BeginTrace();
        void End();

        void Fill(in Visible visible, ref Matrix transformation);
        void Fill(in Visible visible, ref Matrix transformation, in Color color);

        void Trace(in Visible visible, ref Matrix transformation);
        void Trace(in Visible visible, ref Matrix transformation, in Color color);
    }
}
