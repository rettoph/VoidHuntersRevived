using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Entities
{
    public struct TeamId : IEquatable<TeamId>, IEntityComponent
    {
        public static readonly TeamId TeamZero = new TeamId(nameof(TeamZero));
        public static readonly TeamId TeamOne = new TeamId(nameof(TeamOne));

        public readonly VhId Value;

        public TeamId(string key)
        {
            this.Value = NameSpace<TeamId>.Instance.Create(key);
        }

        public override bool Equals(object? obj)
        {
            return obj is TeamId id && Equals(id);
        }

        public bool Equals(TeamId other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(TeamId left, TeamId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TeamId left, TeamId right)
        {
            return !(left == right);
        }
    }
}
