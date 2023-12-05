using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Components;
using Guppy.Game.ImGui;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Client;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Domain.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<IVoidHuntersGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class SimulationDebugComponent : GuppyComponent, IDebugComponent, IGuppyDrawable
    {
        private readonly ISimulationService _simulations;
        private (Simulation, ISpace, IEntityService, DebugView, Ref<bool>)[] _data;
        private ILockstepSimulation? _lockstep;
        private readonly IImGui _imgui;
        private readonly GraphicsDevice _graphics;
        private readonly Camera2D _camera;
        private readonly SpriteFont _font;

        public SimulationDebugComponent(
            IImGui imgui, 
            ISimulationService simulations,
            IResourceProvider resources,
            GraphicsDevice graphics,
            Camera2D camera)
        {
            _imgui = imgui;
            _simulations = simulations;
            _graphics = graphics;
            _camera = camera;
            _data = Array.Empty<(Simulation, ISpace, IEntityService, DebugView, Ref<bool>)>();
            _font = resources.Get(Common.Client.Resources.SpriteFonts.Default);
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _data = _simulations.Instances.Select(x => (
                (x as Simulation)!, 
                x.Scope.Resolve<ISpace>(), 
                x.Scope.Resolve<IEntityService>(), 
                this.BuildDebugView(x.Scope.Resolve<World>()),
                new Ref<bool>(false)
            )).ToArray();
        }

        public void Draw(GameTime gameTime)
        {
            foreach(var (_, _, _, debugView, enabled) in _data)
            {
                if(enabled)
                {
                    debugView.RenderDebugData(_camera.Projection, _camera.View);
                }
            }
        }

        public void RenderDebugInfo(GameTime gameTime)
        {

            _imgui.TextCentered("Simulation Info");

            if (_imgui.BeginTable($"#{nameof(SimulationDebugComponent)}_Table", 8, ImGuiTableFlags.RowBg | ImGuiTableFlags.NoClip | ImGuiTableFlags.Borders))
            {
                _imgui.TableSetupColumn("Simulation", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Entities", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Trees", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Nodes", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Bodies", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Contacts", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Elapsed Time", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableSetupColumn("Aether", ImGuiTableColumnFlags.WidthStretch);

                _imgui.TableHeadersRow();

                foreach ((Simulation simulation, ISpace space, IEntityService entities, _, Ref<bool> aether) in _data)
                {
                    _imgui.TableNextRow();

                    _imgui.TableNextColumn();
                    _imgui.Text(simulation.Type.ToString());

                    _imgui.TableNextColumn();
                    _imgui.Text(entities.CalculateTotal<EntityId>().ToString("#,###,##0"));

                    _imgui.TableNextColumn();
                    _imgui.Text(entities.CalculateTotal<Tree>().ToString("#,###,##0"));

                    _imgui.TableNextColumn();
                    _imgui.Text(entities.CalculateTotal<Node>().ToString("#,###,##0"));

                    _imgui.TableNextColumn();
                    _imgui.Text(space.BodyCount.ToString("#,##0"));

                    _imgui.TableNextColumn();
                    _imgui.Text(space.ContactCount.ToString("#,##0"));

                    _imgui.TableNextColumn();
                    _imgui.Text(TimeSpan.FromSeconds((float)simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'));

                    _imgui.TableNextColumn();
                    if(_imgui.Button(simulation.Type.ToString(), $"Toggle ({(aether ? "enabled" : "disabled")})"))
                    {
                        aether.Value = !aether;
                    }
                }
            }

            _imgui.EndTable();
        }

        private DebugView BuildDebugView(World world)
        {
            DebugView debugView = new DebugView(world);
            debugView.LoadContent(_graphics, _font);

            return debugView;
        }
    }
}
