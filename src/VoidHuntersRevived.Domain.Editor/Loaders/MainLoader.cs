using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Input.Enums;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Editor.Messages;
using VoidHuntersRevived.Domain.Editor.Services;
using VoidHuntersRevived.Domain.Editor.Systems;

namespace VoidHuntersRevived.Domain.Editor.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<EditorGuppy>();
            services.AddScoped<IEditor, Editor>();
            services.AddTransient<IVerticesBuilder, VerticesBuilder>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<EditorPreviewSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<RigidEditorSystem>()
                    .AddInterfaceAliases();
            });

            services.AddInput(Inputs.AddVertex, CursorButtons.Left, new[]
            {
                (false, new VertexInput{ Action = VertexInput.Actions.Add })
            });

            services.AddInput(Inputs.RemoveVertex, CursorButtons.Right, new[]
            {
                (false, new VertexInput{ Action = VertexInput.Actions.Remove })
            });
        }
    }
}
