﻿using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Options
{
    public unsafe struct DeserializationOptions
    {
        public VhId Seed { get; init; }
        public Id<ITeam> TeamId { get; init; }
        public VhId Owner { get; init; }
    }
}
