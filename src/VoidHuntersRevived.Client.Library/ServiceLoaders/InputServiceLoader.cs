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
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterSetup<InputCommandService>((inputs, _, _) =>
            {
                #region Ship Movement Inputs
                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_forward",
                    DefaultInput = new InputButton(Keys.W),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust Forward true"),
                        (state: ButtonState.Released, command: "ship thrust Forward false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_turn_right",
                    DefaultInput = new InputButton(Keys.D),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust TurnRight true"),
                        (state: ButtonState.Released, command: "ship thrust TurnRight false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_backward",
                    DefaultInput = new InputButton(Keys.S),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust Backward true"),
                        (state: ButtonState.Released, command: "ship thrust Backward false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_turn_left",
                    DefaultInput = new InputButton(Keys.A),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust TurnLeft true"),
                        (state: ButtonState.Released, command: "ship thrust TurnLeft false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_right",
                    DefaultInput = new InputButton(Keys.E),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust Right true"),
                        (state: ButtonState.Released, command: "ship thrust Right false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "thrust_left",
                    DefaultInput = new InputButton(Keys.Q),
                    Lockable = false,
                    Commands = new[]
                    {
                        (state: ButtonState.Pressed, command: "ship thrust Left true"),
                        (state: ButtonState.Released, command: "ship thrust Left false")
                    }
                });

                inputs.Add(new InputCommandContext()
                {
                    Handle = "tractorbeam_action",
                    DefaultInput = new InputButton(MouseButton.Right),
                    Lockable = false,
                    Commands = new[]
    {
                        (state: ButtonState.Pressed, command: "ship tractorbeam Select"),
                        (state: ButtonState.Released, command: "ship tractorbeam Attach")
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
