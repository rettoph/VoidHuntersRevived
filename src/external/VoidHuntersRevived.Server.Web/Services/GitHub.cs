using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Release = VoidHuntersRevived.Server.Web.Models.Release;
using Asset = VoidHuntersRevived.Server.Web.Models.Asset;
using VoidHuntersRevived.Server.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace VoidHuntersRevived.Server.Web.Services
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
        public async Task<Release> GetLatest()
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
                        var match = Regex.Match(a.Name, "vhr_(.+)_(.+)_(.+)\\.zip");

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
        #endregion
    }
}
