using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct EntityStatus : IEntityComponent
    {
        public EntityStatusEnum Value;

        public EntityStatus(EntityStatusEnum value)
        {
            this.Value = value;
        }

        public bool IsSpawned => this.Value == EntityStatusEnum.Spawned;
        public bool IsDespawned => this.Value > EntityStatusEnum.Spawned;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(EntityStatus lhs, EntityStatus rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(EntityStatus lhs, EntityStatus rhs)
        {
            return lhs.Value != rhs.Value;
        }
    }
}
