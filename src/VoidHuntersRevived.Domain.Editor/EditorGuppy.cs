using Guppy.Common;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.MonoGame;
using Guppy.MonoGame.Extensions.Primitives;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using Guppy.Providers;
using Guppy.Resources.Providers;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Client;
using VoidHuntersRevived.Domain.Server;

namespace VoidHuntersRevived.Domain.Editor
{
    public class EditorGuppy : LocalGameGuppy, IEditorGuppy
    {
        private readonly IShipPartEditor[] _editors;
        private readonly ShipPartResource _shipPart;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly ImGuiBatch _imGuiBatch;
        private readonly Camera2D _camera;
        private ImFontPtr _font;
        private readonly Num.Vector2 _menuSpacing;
        private VertexPositionColor[] _grid;
        private Stage _stage;

        public EditorGuppy(
            Stage stage,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            ImGuiBatch imGuiBatch,
            Camera2D camera,
            ISimulationService simulations,
            IFiltered<IShipPartEditor> editors,
            IGuppyProvider guppies) : base(simulations)
        {
            _editors = editors.Instances.ToArray();
            _shipPart = new ShipPartResource("editing");
            _primitiveBatch = primitiveBatch;
            _imGuiBatch = imGuiBatch;
            _menuSpacing = new Num.Vector2(15, 15);
            _camera = camera;
            _grid = new VertexPositionColor[2];
            _stage = stage;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _font = _imGuiBatch.Fonts[ResourceConstants.DiagnosticsImGuiFont].Ptr;
            _imGuiBatch.IO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            _imGuiBatch.IO.ConfigDockingNoSplit = true;
            _imGuiBatch.IO.ConfigDockingWithShift = false;

            foreach (var editor in _editors)
            {
                editor.Initialize(_shipPart);
            }

            _stage.Initialize(StyleSheets.Main);
            _stage.Add(new TextInput("test"));

            return;
            for(int i=0; i<1000; i++)
            {
                _stage.Add(new Label()
                {
                    Text = Guid.NewGuid().ToString()
                });
            }

        }

        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            ImGui.PushStyleColor(ImGuiCol.Text, Color.White.ToNumericsVector4());
            ImGui.PushFont(_font);

            _primitiveBatch.Begin(_camera);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            // ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);
            // ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0.0f, 0.0f));
            // ImGui.Begin(DockSpaces.Editor + "Window", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);
            // {
            //     ImGui.PopStyleVar(3);
            // 
            //     uint id = ImGui.GetID(DockSpaces.Editor);
            //     ImGui.DockSpace(id, new Num.Vector2(0, 0), ImGuiDockNodeFlags.PassthruCentralNode);
            // }
            // 
            // 
            // 
            // 
            // foreach (var editor in _editors)
            // {
            //     editor.Draw(gameTime);
            // }
            // 
            // ImGui.End();
        }

        private void DrawGrid()
        {
            var corners = _camera.Frustum.GetCorners();
            var top = MathF.Floor(corners[0].Y);
            var right = MathF.Ceiling(corners[1].X);
            var bottom = MathF.Ceiling(corners[2].Y);
            var left = MathF.Floor(corners[3].X);

            _grid[0].Position.Y = top;
            _grid[1].Position.Y = bottom;
            for(var x=left; x<=right; x++)
            {
                _grid[0].Position.X = x;
                _grid[1].Position.X = x;

                _grid[0].Color = _grid[1].Color = x == 0 ? Color.White : Color.Gray;

                _primitiveBatch.DrawLine(_grid[0], _grid[1]);
            }

            _grid[0].Position.X = left;
            _grid[1].Position.X = right;
            for (var y = top; y <= bottom; y++)
            {
                _grid[0].Position.Y = y;
                _grid[1].Position.Y = y;

                _grid[0].Color = _grid[1].Color = y == 0 ? Color.White : Color.Gray;

                _primitiveBatch.DrawLine(_grid[0], _grid[1]);
            }
        }

        protected override void PostDraw(GameTime gameTime)
        {
            _primitiveBatch.End();

            _stage.Draw(gameTime);


            ImGui.PopFont();
            ImGui.PopStyleColor();

            base.PostDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var editor in _editors)
            {
                editor.Update(gameTime);
            }
        }
    }
}