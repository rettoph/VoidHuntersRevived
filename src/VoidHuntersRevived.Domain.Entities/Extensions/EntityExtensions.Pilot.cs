using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static partial class EntityExtensions
    {
		public static Entity MakePilot(this Entity entity, Entity pilotable)
		{
			if(!pilotable.IsPilotable())
			{
				throw new ArgumentException($"{nameof(EntityExtensions)}::{nameof(MakePilot)} - Argument '{nameof(pilotable)}' failed {nameof(IsPilotable)} check.");
			}

			entity.Attach(new Piloting(pilotable));

			return entity;
		}

		public static bool IsPilot(this Entity entity)
		{
			return entity.Has<Piloting>();
		}
	}
}
