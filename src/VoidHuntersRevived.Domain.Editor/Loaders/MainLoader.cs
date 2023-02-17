using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.MonoGame.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Editor.Messages;
using VoidHuntersRevived.Domain.Editor.Editors;
using VoidHuntersRevived.Domain.Editor.Services;

namespace VoidHuntersRevived.Domain.Editor.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<EditorGuppy>();

            services.AddTransient<IVerticesBuilder, VerticesBuilder>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<RigidEditor>()
                    .AddInterfaceAliases();
            });

            services.AddInput(Inputs.AddVertex, MouseButtons.Left, new[]
            {
                (ButtonState.Released, new VertexInput{ Action = VertexInput.Actions.Add })
            });

            services.AddInput(Inputs.RemoveVertex, MouseButtons.Right, new[]
            {
                (ButtonState.Released, new VertexInput{ Action = VertexInput.Actions.Remove })
            });
        }
    }
}
