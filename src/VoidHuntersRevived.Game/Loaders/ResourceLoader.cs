using Guppy.Attributes;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal class ResourceLoader : IResourceLoader
    {
        public void Load(IResourceProvider resources)
        {
            resources.Register(
                Resources.Strings.TeamZeroName,
                Resources.Strings.TeamOneName,

                Resources.Fonts.Default,

                Resources.Colors.None,
                Resources.Colors.HullPrimaryColor,
                Resources.Colors.HullSecondaryColor,
                Resources.Colors.ThrusterPrimaryColor,
                Resources.Colors.ThrusterSecondaryColor,
                Resources.Colors.TeamOnePrimaryColor,
                Resources.Colors.TeamOneSecondaryColor,
                Resources.Colors.TractorBeamHighlight
            );
        }
    }
}
