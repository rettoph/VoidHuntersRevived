using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Client.Services
{
    public interface IVisibleRenderingService
    {
        void BeginFill();
        void BeginTrace();
        void End();

        void Fill(in Visible visible, ref Matrix transformation, in Color color);
        
        void Trace(in Visible visible, ref Matrix transformation, in Color color);
    }
}
