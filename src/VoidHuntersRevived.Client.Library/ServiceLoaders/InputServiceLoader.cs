using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.IO.Builders;
using Guppy.IO.Enums;
using Guppy.IO.Structs;
using Guppy.ServiceLoaders;
using Microsoft.Xna.Framework.Input;
using System;
using VoidHuntersRevived.Client.Library.Messages.Commands;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Commands;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class InputServiceLoader : IInputCommandLoader
    {
        public void RegisterInputCommands(InputCommandServiceBuilder inputCommands)
        {
            inputCommands.RegisterInputCommand("toggle_chunk_debug_view")
                .SetInputButton(Keys.F1)
                .AddCommand(ButtonState.Released, new ToggleRenderChunkDebugViewCommand());

            inputCommands.RegisterInputCommand("toggle_aether_debug_view")
                .SetInputButton(Keys.F2)
                .AddCommand(ButtonState.Released, new ToggleAetherDebugViewCommand());

            inputCommands.RegisterInputCommand("thrust_forward")
                .SetInputButton(Keys.W)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.Forward,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.Forward,
                    State = false
                });

            inputCommands.RegisterInputCommand("thrust_turn_right")
                .SetInputButton(Keys.D)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.TurnRight,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.TurnRight,
                    State = false
                });

            inputCommands.RegisterInputCommand("thrust_backward")
                .SetInputButton(Keys.S)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.Backward,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.Backward,
                    State = false
                });

            inputCommands.RegisterInputCommand("thrust_turn_left")
                .SetInputButton(Keys.A)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.TurnLeft,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.TurnLeft,
                    State = false
                });

            inputCommands.RegisterInputCommand("thrust_right")
                .SetInputButton(Keys.E)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.Right,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.Right,
                    State = false
                });

            inputCommands.RegisterInputCommand("thrust_left")
                .SetInputButton(Keys.Q)
                .AddCommand(ButtonState.Pressed, new DirectionRequestCommand()
                {
                    Direction = Direction.Left,
                    State = true
                })
                .AddCommand(ButtonState.Released, new DirectionRequestCommand()
                {
                    Direction = Direction.Left,
                    State = false
                });

            inputCommands.RegisterInputCommand("tractorbeam_action")
                .SetInputButton(MouseButton.Right)
                .AddCommand(ButtonState.Pressed, new TractorBeamRequestCommand()
                {
                    Type = TractorBeamStateType.Select
                })
                .AddCommand(ButtonState.Released, new TractorBeamRequestCommand()
                {
                    Type = TractorBeamStateType.Deselect
                });
        }

        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            //services.RegisterSetup<InputCommandService>((inputs, _, _) =>
            //{
            //    #region Ship Movement Inputs
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_forward",
            //        DefaultInput = new InputButton(Keys.W),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust Forward true"),
            //            (state: ButtonState.Released, command: "ship thrust Forward false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_turn_right",
            //        DefaultInput = new InputButton(Keys.D),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust TurnRight true"),
            //            (state: ButtonState.Released, command: "ship thrust TurnRight false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_backward",
            //        DefaultInput = new InputButton(Keys.S),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust Backward true"),
            //            (state: ButtonState.Released, command: "ship thrust Backward false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_turn_left",
            //        DefaultInput = new InputButton(Keys.A),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust TurnLeft true"),
            //            (state: ButtonState.Released, command: "ship thrust TurnLeft false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_right",
            //        DefaultInput = new InputButton(Keys.E),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust Right true"),
            //            (state: ButtonState.Released, command: "ship thrust Right false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "thrust_left",
            //        DefaultInput = new InputButton(Keys.Q),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship thrust Left true"),
            //            (state: ButtonState.Released, command: "ship thrust Left false")
            //        }
            //    });
            //
            //    inputs.Add(new InputCommandContext()
            //    {
            //        Handle = "tractorbeam_action",
            //        DefaultInput = new InputButton(MouseButton.Right),
            //        Lockable = false,
            //        Commands = new[]
            //        {
            //            (state: ButtonState.Pressed, command: "ship tractorbeam Select"),
            //            (state: ButtonState.Released, command: "ship tractorbeam Attach")
            //        }
            //    });
            //    #endregion
            //});
        }
    }
}
