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
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.Client.GuppyComponents
{
    [AutoLoad]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [GuppyFilter<GameGuppy>]
    internal class DebugSpaceComponent : GuppyComponent, IDebugComponent
    {
        private readonly ISimulationService _simulations;
        private (SimulationType, ISpace)[] _spaces;

        public DebugSpaceComponent(ISimulationService simulations)
        {
            _simulations = simulations;
            _spaces = Array.Empty<(SimulationType, ISpace)>();
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _spaces = _simulations.Instances.Select(x => (x.Type, x.Scope.Resolve<ISpace>())).ToArray();

        }

        public IEnumerable<string> GetDebugInfo(GameTime gameTime)
        {
            foreach((SimulationType type, ISpace space) in _spaces)
            {
                yield return $"{type}";
                yield return $"\tBodies - {space.BodyCount.ToString("#,##0")}";
                yield return $"\tContacts - {space.ContactCount.ToString("#,##0")}";
            }
        }

        public void RenderDebugInfo(IGui gui, GameTime gameTime)
        {
            gui.PushStyleVar(GuiStyleVar.WindowPadding, new Vector2(1, 1));
            gui.PushStyleVar(GuiStyleVar.CellPadding, new Vector2(5, 5));

            if(gui.BeginChild($"#{nameof(DebugSpaceComponent)}_Container", Vector2.Zero, GuiChildFlags.AlwaysAutoResize | GuiChildFlags.AutoResizeY | GuiChildFlags.AutoResizeX | GuiChildFlags.AlwaysUseWindowPadding))
            {
                if(gui.BeginTable($"#{nameof(DebugSpaceComponent)}_Table", 3, GuiTableFlags.RowBg | GuiTableFlags.NoClip | GuiTableFlags.Borders))
                {
                    gui.TableSetupColumn("Simulation", GuiTableColumnFlags.WidthFixed);

                    gui.TableSetupColumn("Bodies", GuiTableColumnFlags.WidthFixed);

                    gui.TableSetupColumn("Contacts", GuiTableColumnFlags.WidthFixed);

                    gui.TableHeadersRow();

                    foreach ((SimulationType type, ISpace space) in _spaces)
                    {
                        gui.TableNextRow();

                        gui.TableNextColumn();
                        gui.Text(type.ToString());

                        gui.TableNextColumn();
                        gui.Text(space.BodyCount.ToString("#,##0"));

                        gui.TableNextColumn();
                        gui.Text(space.ContactCount.ToString("#,##0"));
                    }
                }

                gui.EndTable();
            }

            gui.EndChild();
            gui.PopStyleVar();
            gui.PopStyleVar();
        }
    }
}
