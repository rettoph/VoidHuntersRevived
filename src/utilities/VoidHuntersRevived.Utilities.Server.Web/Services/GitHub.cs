using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Release = VoidHuntersRevived.Utilities.Server.Web.Models.Release;
using Asset = VoidHuntersRevived.Utilities.Server.Web.Models.Asset;
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
        public async Task<Release> UpdateLatest()
        {
            var releases = await _client.Repository.Release.GetAll(WebConstants.ProjectOwner, WebConstants.ProjectName);
            var truth = releases.Aggregate((r1, r2) => r1.CreatedAt > r2.CreatedAt ? r1 : r2);

            var latest = _context.Release.Include(r => r.Assets).OrderByDescending(p => p.Id).FirstOrDefault();

            if (latest?.Version != truth.TagName)
            {
                latest = new Release()
                {
                    ReleaseDate = truth.CreatedAt.DateTime,
                    Version = truth.TagName,
                    Assets = truth.Assets.Select(a =>
                    {
                        var match = Regex.Match(a.Name, "vhr_(.+)_(.+)_(.+)\\....");

                        return new Asset()
                        {
                            RID = match.Groups[3].Value,
                            Type = match.Groups[2].Value,
                            DownloadURL = a.BrowserDownloadUrl
                        };
                    }).ToList()
                };
                _context.Release.Add(latest);
                _context.SaveChanges();
            }

            return latest;
        }
        public async Task<Release> GetRelease(String type, String rid, String version)
        {
            Release release = null;

            if (version == "latest")
                release = _context.Release.Include(r => r.Assets).Select(r => new Release()
                {
                    Id = r.Id,
                    ReleaseDate = r.ReleaseDate,
                    Assets = r.Assets.Where(a => type == a.Type && rid == a.RID).ToList(),
                    Version = r.Version
                }).OrderByDescending(p => p.Id).FirstOrDefault();
            else
                release = _context.Release.Include(r => r.Assets)
                    .Where(r => r.Version == version)
                    .Select(r => new Release() {
                        Id = r.Id,
                        ReleaseDate = r.ReleaseDate,
                        Assets = r.Assets.Where(a => type == a.Type && rid == a.RID).ToList(),
                        Version = r.Version
                    }).FirstOrDefault();

            return release;
        }

        public async Task<Release> GetLatest()
        {
            return _context.Release.OrderByDescending(r => r.ReleaseDate).FirstOrDefault();
        }
        #endregion
    }
}
