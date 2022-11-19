using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
using Guppy.MonoGame.UI.Elements;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Providers;
using Num = System.Numerics;

namespace VoidHuntersRevived.Client.Library.Debuggers
{
    [GuppyFilter(typeof(ClientGameGuppy))]
    internal sealed class WorldDebugger : IImGuiDebugger
    {
        private IStepProvider _steps;
        private ITickProvider _ticks;
        private Num.Vector4 _textColor;
        private ImFontPtr _font;
        private ImFontPtr _fontHeader;
        private Window _window;

        public WorldDebugger(IFiltered<IStepProvider> steps, IFiltered<ITickProvider> ticks)
        {
            _steps = steps.Instance ?? throw new ArgumentNullException();
            _ticks = ticks.Instance ?? throw new ArgumentNullException();
            _textColor = Color.White.ToNumericsVector4();
            _window = new Window("World Data");
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
            _fontHeader = imGuiBatch.Fonts[ImGuiFontConstants.DiagnosticsFontHeader].Ptr;
            _font = imGuiBatch.Fonts[ImGuiFontConstants.DiagnosticsFont].Ptr;

            _window.AddStyleVar(ImGuiStyleVar.WindowMinSize, new Num.Vector2(400, 0));

            _window.Font = imGuiBatch.Fonts[ImGuiFontConstants.DiagnosticsFontHeader];
        }

        public void Draw(GameTime gameTime)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, _textColor);

            _window.Draw(gameTime);

            ImGui.PopStyleColor();
        }

        public void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
