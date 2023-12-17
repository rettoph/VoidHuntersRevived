using Guppy.Attributes;
using Guppy.Common.Services;
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
        public ImGuiFix64Viewer(IObjectTextFilterService filter) : base(filter)
        {

        }
        protected override bool RenderObjectViewer(int? index, string? name, Type type, Fix64 instance, IImGui imgui, string filter, int maxDepth, int currentDepth)
        {
            string title = this.GetTitle(index, name, type, instance);
            bool filtered = filter.IsNotNullOrEmpty() && title.Contains(filter);

            imgui.Text($"{title}");

            return filtered;
        }
    }
}
