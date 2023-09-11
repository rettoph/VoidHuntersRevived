using Guppy.Attributes;
using Guppy.Resources;
using Guppy.Resources.Constants;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Common.Loaders
{
    [AutoLoad]
    internal class ResourcePackLoader : IPackLoader
    {
        public void Load(IResourcePackProvider packs)
        {
            packs.Configure(VoidHuntersPack.Id, pack =>
            {
                pack.Add(Resources.Strings.TeamZeroName, Localization.en_US, "Default");
                pack.Add(Resources.Strings.TeamOneName, Localization.en_US, "Blue Team");

                pack.Add(Resources.Colors.None, default);

                pack.Add(Resources.Colors.HullPrimaryColor, new Color(Color.Orange, 0.85f));
                pack.Add(Resources.Colors.HullSecondaryColor, new Color(Color.Yellow, 0.85f));
                pack.Add(Resources.Colors.ThrusterPrimaryColor, new Color(Color.Green, 0.85f));
                pack.Add(Resources.Colors.ThrusterSecondaryColor, new Color(Color.LightGreen, 0.85f));

                pack.Add(Resources.Colors.TeamOnePrimaryColor, new Color(0, 200, 255, 230));
                pack.Add(Resources.Colors.TeamOneSecondaryColor, new Color(0, 255, 247, 230));

                pack.Add(Resources.Colors.TractorBeamHighlight, new Color(Color.LightGray, 0.25f));
            });
        }
    }
}
