using Guppy.Attributes;
using Guppy.Game.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Client.ImGui
{
    [AutoLoad]
    internal class ImGuiFix64Viewer : ImGuiObjectViewer<Fix64>
    {
        protected override bool RenderObjectViewer(string title, Type type, Fix64 instance, IImGui imgui, string? filter, int maxDepth, int currentDepth)
        {
            imgui.Text($"{title}");

            return filter is null || title.Contains(filter);
        }
    }
}
