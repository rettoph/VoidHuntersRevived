using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Contexts;
using Guppy.IO.Enums;
using Guppy.IO.Services;
using Guppy.IO.Structs;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterSetup<InputCommandService>((inputs, _, _) =>
            {
                #region Ship Movement Inputs
                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_forward",
                    DefaultInput = new InputButton(Keys.W),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction Forward true"),
                        (state: ButtonState.Released, command: "ship set direction Forward false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_turn_right",
                    DefaultInput = new InputButton(Keys.D),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction TurnRight true"),
                        (state: ButtonState.Released, command: "ship set direction TurnRight false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_backward",
                    DefaultInput = new InputButton(Keys.S),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction Backward true"),
                        (state: ButtonState.Released, command: "ship set direction Backward false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_turn_left",
                    DefaultInput = new InputButton(Keys.A),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction TurnLeft true"),
                        (state: ButtonState.Released, command: "ship set direction TurnLeft false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_right",
                    DefaultInput = new InputButton(Keys.E),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction Right true"),
                        (state: ButtonState.Released, command: "ship set direction Right false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "set_direction_left",
                    DefaultInput = new InputButton(Keys.Q),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship set direction Left true"),
                        (state: ButtonState.Released, command: "ship set direction Left false")
                    }
                });
                #endregion
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
