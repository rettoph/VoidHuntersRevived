using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.Providers;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
using Guppy.MonoGame.UI.Providers;
using Guppy.Network;
using Guppy.Network.Peers;
using ImGuiNET;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Domain.Client
{
    public class MainMenuGuppy : ImGuiGuppy
    {
        private readonly ImGuiBatch _imGui;
        private readonly Menu _menu;
        private ImFontPtr _font;

        public MainMenuGuppy(
            ClientPeer client,
            NetScope scope,
            ImGuiBatch imGui,
            IMenuProvider menus)
        {
            _imGui = imGui;
            _menu = menus.Get(Menus.Main);

            client.Bind(scope, NetScopeIds.MainMenu);
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _font = _imGui.Fonts[ResourceConstants.DiagnosticsImGuiFont].Ptr;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ImGui.PushStyleColor(ImGuiCol.Text, Color.White.ToNumericsVector4());
            ImGui.PushFont(_font);


            if (ImGui.Begin("vhr-main-menu", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground))
            {
                foreach (MenuItem item in _menu.Items)
                {
                    if (ImGui.Button(item.Label, new Num.Vector2(150, 50)))
                    {
                        this.Bus.Enqueue(item.OnClick);
                    }
                }
            }
            ImGui.End();

            ImGui.PopFont();
            ImGui.PopStyleColor();
        }
    }
}
