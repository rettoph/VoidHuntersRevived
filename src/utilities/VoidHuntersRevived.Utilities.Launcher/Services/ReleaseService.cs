using RestSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Utilities.Launcher.Models;

namespace VoidHuntersRevived.Utilities.Launcher.Services
{
    public class ReleaseService
    {
        private List<Release> Releases { get; set; }

        public readonly String RID;
        public readonly String Type;

        private static RestClient _releaseServer;

        public ReleaseService(String rid, String type)
        {
            this.RID = rid;
            this.Type = type;
            this.Releases = Settings.Default.Releases
                .Where(r => r.Type == this.Type && r.RID == this.RID)
                .OrderBy(r => r.Version)
                .ToList();
        }

        static ReleaseService()
        {
            _releaseServer = new RestClient(Settings.Default.ReleaseServer);
        }

        public Release TryGetVersion(String version)
        {
            return this.Releases.FirstOrDefault(r => r.Version == version) ?? _releaseServer.Execute<Release>(new RestRequest($"{Settings.Default.GetReleaseEndpoint}/{version}?type={this.Type}&rid={this.RID}", Method.GET))?.Data;
        }

        public static Release TryGetLatest(String rid, String type, Boolean checkRemote = true)
        {
            if(checkRemote)
            {
                var remote = _releaseServer.Execute<Release>(new RestRequest($"{Settings.Default.GetLatestEndpoint}?type={type}&rid={rid}", Method.GET))?.Data;
                return Settings.Default.Releases.FirstOrDefault(r => r == remote) 
                    ?? remote 
                    ?? Settings.Default.Releases.Where(r => r.RID == rid && r.Type == type)
                            .OrderByDescending(r => r.Version)
                            .Last();
            }
            
            return Settings.Default.Releases.Where(r => r.RID == rid && r.Type == type)
                .OrderByDescending(r => r.Version)
                .Last();
        }
    }
}
