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
    [Route("releases")]
    public class ReleaseController : Controller
    {
        private readonly GitHub _github;

        public ReleaseController(GitHub github)
        {
            _github = github;
        }

        [Route("update")]
        public async Task<IActionResult> Update()
        {
            await _github.UpdateLatest();
            return Json(true);
        }

        [Route("latest")]
        public async Task<IActionResult> Latest(String type, String rid)
        {
            return Json(_github.GetRelease(type, rid, "latest"));
        }

        [Route("{version}")]
        public async Task<IActionResult> Version(String version, String type, String rid)
        {
            return Json(_github.GetRelease(type, rid, version));
        }
    }
}
