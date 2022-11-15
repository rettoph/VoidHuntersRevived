using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
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

        public WorldDebugger(IFiltered<IStepProvider> steps, IFiltered<ITickProvider> ticks)
        {
            _steps = steps.Instance ?? throw new ArgumentNullException();
            _ticks = ticks.Instance ?? throw new ArgumentNullException();
            _textColor = Color.White.ToNumericsVector4();
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
            _fontHeader = imGuiBatch.Fonts[ImGuiFontConstants.DiagnosticsFontHeader].Ptr;
            _font = imGuiBatch.Fonts[ImGuiFontConstants.DiagnosticsFont].Ptr;
        }

        public void Draw(GameTime gameTime)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, _textColor);
            ImGui.PushFont(_font);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Num.Vector2(400, 0));
            if (ImGui.Begin($"World Data"))
            {

            }
            ImGui.End();
            ImGui.PopStyleVar();

            ImGui.PopFont();
            ImGui.PopStyleColor();
        }

        public void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
