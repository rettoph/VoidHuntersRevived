using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Enums;
using Guppy.IO.Input;
using Guppy.IO.Input.Contexts;
using Guppy.IO.Input.Services;
using Guppy.UI.Interfaces;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Windows.Library.ServiceLoaders
{
    [AutoLoad(10)]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var inputs = provider.GetService<InputCommandService>();

            #region Spawn Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "try_spawn_ai",
                DefaultInput = new InputType(Keys.G),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Released, command: "spawn ai")
                }
            });
            #endregion

            #region Ship Movement Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "set_firing",
                DefaultInput = new InputType(MouseButton.Left),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship fire -v=true"),
                    (state: ButtonState.Released, command: "ship fire -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_forward",
                DefaultInput = new InputType(Keys.W),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=Forward -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=Forward -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_turn_left",
                DefaultInput = new InputType(Keys.A),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=TurnLeft -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=TurnLeft -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_backward",
                DefaultInput = new InputType(Keys.S),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=Backward -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=Backward -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_turn_right",
                DefaultInput = new InputType(Keys.D),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=TurnRight -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=TurnRight -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_left",
                DefaultInput = new InputType(Keys.Q),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=Left -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=Left -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_right",
                DefaultInput = new InputType(Keys.E),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship direction -d=Right -v=true"),
                    (state: ButtonState.Released, command: "ship direction -d=Right -v=false")
                }
            });
            #endregion

            #region TractorBeam Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "tractorbeam",
                DefaultInput = new InputType(MouseButton.Right),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship tractorbeam -a=select"),
                    (state: ButtonState.Released, command: "ship tractorbeam -a=attach")
                }
            });
            #endregion

            #region SpellCast Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "launch_fighter_bays",
                DefaultInput = new InputType(Keys.F),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship launch-fighters"),
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "toggle_energy_shields",
                DefaultInput = new InputType(Keys.C),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship toggle-energy-shields"),
                }
            });
            #endregion

            #region Save Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "save_ship",
                DefaultInput = new InputType(Keys.F12),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship save -n=ship"),
                }
            });
            #endregion

            #region Self Destruct Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "self-destruct_ship",
                DefaultInput = new InputType(Keys.Space),
                Lockable = true,
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "ship self-destruct"),
                }
            });
            #endregion

            #region Toggle Debug Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "toggle_debug_slave",
                DefaultInput = new InputType(Keys.F1),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "toggle debug -t=slave"),
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "toggle_debug_master",
                DefaultInput = new InputType(Keys.F2),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "toggle debug -t=master"),
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "toggle_debug_data",
                DefaultInput = new InputType(Keys.F3),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "toggle debug -t=data"),
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "toggle_impulse_data",
                DefaultInput = new InputType(Keys.F4),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "toggle debug -t=impulse"),
                }
            });
            #endregion
        }
    }
}
