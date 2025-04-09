using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.Services;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.Graphics.Common;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Guppy.Game.ImGui.Common.Extensions;
using Guppy.Game.ImGui.Common.Services;
using Guppy.Game.ImGui.Common.Styling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    public class AetherDebugSystem :
        ISceneSystem,
        IDrawSystem,
        IImGuiSystem,
        IDebugSystem
    {
        public static string? Group => typeof(World).Name;

        private readonly IStrategy _strategy;
        private readonly IScene _scene;
        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly World _world;
        private readonly DebugView _debug;
        private readonly ICamera2D _camera;
        private bool _debugViewEnabled;
        private bool _aetherExplorerEnabled;
        private string _filter;
        private readonly Asset<ImStyle> _buttonRedStyle;
        private readonly Asset<ImStyle> _buttonGreenStyle;

        public AetherDebugSystem(
            IStrategy strategy,
            IScene scene,
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IAssetService assetService,
            World world,
            GraphicsDevice graphics,
            ICamera2D camera)
        {
            this._strategy = strategy;
            this._scene = scene;
            this._imgui = imgui;
            this._objectExplorer = objectExplorer;
            this._world = world;
            this._debug = new DebugView(world);
            this._camera = camera;
            this._debug.LoadContent(graphics, assetService.Get(Assets.SpriteFonts.Default));
            this._filter = string.Empty;

            this._buttonRedStyle = assetService.Get(Assets.ImGuiStyles.ButtonRed);
            this._buttonGreenStyle = assetService.Get(Assets.ImGuiStyles.ButtonGreen);
        }

        [SequenceGroup<DrawSequenceGroupEnum>(DrawSequenceGroupEnum.Draw)]
        public void Draw(GameTime gameTime)
        {
            if (this._debugViewEnabled == false)
            {
                return;
            }

            this._debug.RenderDebugData(this._camera.Projection, this._camera.View, this._camera.World);
        }

        [SequenceGroup<DebugSequenceGroupEnum>("Aether")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Bodies", this._world.BodyList.Count.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Contacts", this._world.ContactCount.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());


            Asset<ImStyle> buttonStyle = this._debugViewEnabled ? this._buttonGreenStyle : this._buttonRedStyle;

            using (this._imgui.Apply(buttonStyle))
            {
                if (this._imgui.Button($"{(this._debugViewEnabled ? "Disable" : "Enable")} DebugView"))
                {
                    this._debugViewEnabled = !this._debugViewEnabled;
                }
            }

            buttonStyle = this._aetherExplorerEnabled ? this._buttonRedStyle : this._buttonRedStyle;

            using (this._imgui.Apply(buttonStyle))
            {
                if (this._imgui.Button($"{(this._aetherExplorerEnabled ? "Disable" : "Enable")} Aether Explorer"))
                {
                    this._aetherExplorerEnabled = !this._aetherExplorerEnabled;
                }
            }
        }

        [SequenceGroup<ImGuiSequenceGroupEnum>(ImGuiSequenceGroupEnum.PostDraw)]
        public void DrawImGui(GameTime gameTime)
        {
            if (this._aetherExplorerEnabled == false)
            {
                return;
            }

            this._imgui.Begin($"Aether Explorer - {this._strategy.Type}, {this._scene.Name} {this._scene.Id}", ref this._aetherExplorerEnabled);

            this._imgui.InputText("Filter", ref this._filter, 255);

            using (this._imgui.ApplyID(nameof(World.BodyList)))
            {
                this._objectExplorer.DrawObjectExplorer(this._world.BodyList, this._filter, 8, [this._world]);
            }

            using (this._imgui.ApplyID(nameof(World.ContactManager)))
            {
                this._objectExplorer.DrawObjectExplorer(this._world.ContactManager, this._filter, 8);
            }

            this._imgui.End();
        }
    }
}