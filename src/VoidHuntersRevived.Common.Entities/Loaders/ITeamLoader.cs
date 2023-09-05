using Guppy.Attributes;
using Guppy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Loaders
{
    [Service<ITeamLoader>(ServiceLifetime.Singleton, true)]
    public interface ITeamLoader
    {
        void Configure(ITeamService teams);
    }
}
