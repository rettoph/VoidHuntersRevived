using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.GUI;
using Guppy.MonoGame.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Domain.Debugging.Components
{
    [AutoLoad]
    [GuppyFilter<IGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    internal class SimulationDebugComponent : GuppyComponent, IDebugComponent
    {
        private readonly ISimulationService _simulations;
        private (Simulation, ISpace, IEntityService)[] _data;
        private ILockstepSimulation? _lockstep;

        public SimulationDebugComponent(ISimulationService simulations)
        {
            _simulations = simulations;
            _data = Array.Empty<(Simulation, ISpace, IEntityService)>();
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _data = _simulations.Instances.Select(x => ((x as Simulation)!, x.Scope.Resolve<ISpace>(), x.Scope.Resolve<IEntityService>())).ToArray();
        }

        public void Initialize(IGui gui)
        {
            //throw new NotImplementedException();
        }

        public void RenderDebugInfo(IGui gui, GameTime gameTime)
        {

            gui.TextCentered("Simulation Info");

            if (gui.BeginTable($"#{nameof(SimulationDebugComponent)}_Table", 7, GuiTableFlags.RowBg | GuiTableFlags.NoClip | GuiTableFlags.Borders))
            {
                gui.TableSetupColumn("Simulation", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Entities", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Trees", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Nodes", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Bodies", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Contacts", GuiTableColumnFlags.WidthStretch);

                gui.TableSetupColumn("Elapsed Time", GuiTableColumnFlags.WidthStretch);

                gui.TableHeadersRow();

                foreach ((Simulation simulation, ISpace space, IEntityService entities) in _data)
                {
                    gui.TableNextRow();

                    gui.TableNextColumn();
                    gui.Text(simulation.Type.ToString());

                    gui.TableNextColumn();
                    gui.Text(entities.CalculateTotal<EntityId>().ToString("#,###,##0"));

                    gui.TableNextColumn();
                    gui.Text(entities.CalculateTotal<Tree>().ToString("#,###,##0"));

                    gui.TableNextColumn();
                    gui.Text(entities.CalculateTotal<Node>().ToString("#,###,##0"));

                    gui.TableNextColumn();
                    gui.Text(space.BodyCount.ToString("#,##0"));

                    gui.TableNextColumn();
                    gui.Text(space.ContactCount.ToString("#,##0"));

                    gui.TableNextColumn();
                    
                    gui.Text(TimeSpan.FromSeconds((float)simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'));
                }
            }

            gui.EndTable();
        }
    }
}
