using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Octokit;
using VoidHuntersRevived.Utilities.Server.Web.Data;
using VoidHuntersRevived.Utilities.Server.Web.Models;
using VoidHuntersRevived.Utilities.Server.Web.Services;

namespace VoidHuntersRevived.Utilities.Server.Web.Controllers
{
    [Route("release")]
    public class ReleaseController : Controller
    {
        private readonly GitHub _github;

        public ReleaseController(GitHub github)
        {
            _github = github;
        }

        public async Task<IActionResult> Index(String type = "", String rid = "", String version = "latest")
        {
            return Json(await _github.GetRelease(type, rid, version));
        }

        [Route("update")]
        public async Task<IActionResult> Update()
        {
            return Json(await _github.UpdateLatest());
        }

        [Route("latest")]
        public async Task<IActionResult> LatestVersion(String type)
        {
            return Json(await _github.GetLatest());
        }
    }
}
