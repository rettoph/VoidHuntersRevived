using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Client.Services
{
    public interface IVisibleRenderingService
    {
        void BeginFill(Color color);
        void BeginTrace(Color color);
        void EndTrace();
        void EndFill();

        void Fill(in Visible visible, ref Matrix transformation);
        
        void Trace(in Visible visible, ref Matrix transformation);
    }
}
