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

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad(200)]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var inputs = provider.GetService<InputCommandService>();

            #region Ship Movement Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_forward",
                DefaultInput = new InputType(Keys.W),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=Forward -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=Forward -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_turn_left",
                DefaultInput = new InputType(Keys.A),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=TurnLeft -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=TurnLeft -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_backward",
                DefaultInput = new InputType(Keys.S),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=Backward -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=Backward -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_turn_right",
                DefaultInput = new InputType(Keys.D),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=TurnRight -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=TurnRight -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_left",
                DefaultInput = new InputType(Keys.Q),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=Left -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=Left -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_right",
                DefaultInput = new InputType(Keys.E),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "set direction -d=Right -v=true"),
                    (state: ButtonState.Released, command: "set direction -d=Right -v=false")
                }
            });
            #endregion

            #region TractorBeam Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "tractorbeam",
                DefaultInput = new InputType(MouseButton.Right),
                Commands = new[]
                {
                    (state: ButtonState.Pressed, command: "tractorbeam -a=select"),
                    (state: ButtonState.Released, command: "tractorbeam -a=attach")
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
                    (state: ButtonState.Pressed, command: "save ship -n=ship"),
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
            #endregion
        }
    }
}
