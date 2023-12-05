﻿using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Components;
using Guppy.GUI;
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
        private readonly IGui _gui;
        private readonly GraphicsDevice _graphics;
        private readonly Camera2D _camera;
        private readonly SpriteFont _font;

        public SimulationDebugComponent(
            IGui gui, 
            ISimulationService simulations,
            IResourceProvider resources,
            GraphicsDevice graphics,
            Camera2D camera)
        {
            _gui = gui;
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

            _gui.TextCentered("Simulation Info");

            if (_gui.BeginTable($"#{nameof(SimulationDebugComponent)}_Table", 8, GuiTableFlags.RowBg | GuiTableFlags.NoClip | GuiTableFlags.Borders))
            {
                _gui.TableSetupColumn("Simulation", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Entities", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Trees", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Nodes", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Bodies", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Contacts", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Elapsed Time", GuiTableColumnFlags.WidthStretch);

                _gui.TableSetupColumn("Aether", GuiTableColumnFlags.WidthStretch);

                _gui.TableHeadersRow();

                foreach ((Simulation simulation, ISpace space, IEntityService entities, _, Ref<bool> aether) in _data)
                {
                    _gui.TableNextRow();

                    _gui.TableNextColumn();
                    _gui.Text(simulation.Type.ToString());

                    _gui.TableNextColumn();
                    _gui.Text(entities.CalculateTotal<EntityId>().ToString("#,###,##0"));

                    _gui.TableNextColumn();
                    _gui.Text(entities.CalculateTotal<Tree>().ToString("#,###,##0"));

                    _gui.TableNextColumn();
                    _gui.Text(entities.CalculateTotal<Node>().ToString("#,###,##0"));

                    _gui.TableNextColumn();
                    _gui.Text(space.BodyCount.ToString("#,##0"));

                    _gui.TableNextColumn();
                    _gui.Text(space.ContactCount.ToString("#,##0"));

                    _gui.TableNextColumn();
                    _gui.Text(TimeSpan.FromSeconds((float)simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'));

                    _gui.TableNextColumn();
                    if(_gui.Button(simulation.Type.ToString(), $"Toggle ({(aether ? "enabled" : "disabled")})"))
                    {
                        aether.Value = !aether;
                    }
                }
            }

            _gui.EndTable();
        }

        private DebugView BuildDebugView(World world)
        {
            DebugView debugView = new DebugView(world);
            debugView.LoadContent(_graphics, _font);

            return debugView;
        }
    }
}
