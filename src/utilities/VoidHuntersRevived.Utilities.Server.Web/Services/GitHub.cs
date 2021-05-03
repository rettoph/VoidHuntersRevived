using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Release = VoidHuntersRevived.Utilities.Server.Web.Models.Release;
using VoidHuntersRevived.Utilities.Server.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace VoidHuntersRevived.Utilities.Server.Web.Services
{
    public class GitHub
    {
        #region Private Fields
        private readonly VoidHuntersRevivedServerWebContext _context;
        private GitHubClient _client;
        #endregion

        #region Constructors
        public GitHub(VoidHuntersRevivedServerWebContext context)
        {
            _context = context;
            _client = new GitHubClient(new ProductHeaderValue("VHRUpdater"));
        }
        #endregion

        #region API Methods
        public async Task UpdateLatest()
        {
            var releases = await _client.Repository.Release.GetAll(WebConstants.ProjectOwner, WebConstants.ProjectName);
            var truth = releases.Aggregate((r1, r2) => r1.CreatedAt > r2.CreatedAt ? r1 : r2);

            var version = truth.TagName;
            var updates = false;

            foreach(var asset in truth.Assets)
            {
                var match = Regex.Match(asset.Name, "vhr_(.+)_(.+)_(.+)\\....");
                var rid = match.Groups[3].Value;
                var type = match.Groups[2].Value;
                var downloadURL = asset.BrowserDownloadUrl;

                if (this.GetRelease(type, rid, version) == default)
                { // Create a new instance...
                    _context.Release.Add(new Release()
                    { 
                        Version = version,
                        RID = rid,
                        Type = type,
                        DownloadUrl = downloadURL,
                        CreatedAt = truth.CreatedAt.DateTime
                    });

                    updates = true;
                }
            }

            if (updates)
                _context.SaveChanges();
        }
        public Release GetRelease(String type, String rid, String version)
        {
            if (version == "latest")
                return _context.Release.Where(r => r.RID == rid && r.Type == type)
                    .OrderByDescending(r => r.Version)
                    .FirstOrDefault();
            else
                return _context.Release.FirstOrDefault(r => r.Version == version && r.RID == rid && r.Type == type);
        }
        #endregion
    }
}
