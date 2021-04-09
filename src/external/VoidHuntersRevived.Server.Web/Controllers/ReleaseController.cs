using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Octokit;
using VoidHuntersRevived.Server.Web.Data;
using VoidHuntersRevived.Server.Web.Models;
using VoidHuntersRevived.Server.Web.Services;

namespace VoidHuntersRevived.Server.Web.Controllers
{
    [Route("releases")]
    public class ReleaseController : Controller
    {
        private readonly GitHub _github;

        public ReleaseController(GitHub github)
        {
            _github = github;
        }

        [Route("latest")]
        public async Task<IActionResult> Latest()
        {
            return Json(await _github.GetLatest());
        }
    }
}
