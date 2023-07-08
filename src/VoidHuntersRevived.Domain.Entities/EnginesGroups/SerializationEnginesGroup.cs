﻿using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.EnginesGroups
{
    internal abstract class SerializationEnginesGroup
    {
        public abstract void Serialize(EntityWriter writer, EGID egid, EntitiesDB entities, uint index);

        public abstract void Deserialize(in VhId seed, EntityReader reader, ref EntityInitializer initializer, bool confirmed);
    }

    internal sealed class SerializationEnginesGroup<T> : SerializationEnginesGroup
        where T : unmanaged, IEntityComponent
    {
        private FasterList<ISerializationEngine<T>> _engines;

        public SerializationEnginesGroup(IEngineService engines)
        {
            _engines = new FasterList<ISerializationEngine<T>>(engines.OfType<ISerializationEngine<T>>().ToArray());
        }

        public override void Serialize(EntityWriter writer, EGID egid, EntitiesDB entities, uint index)
        {
            var (components, _) = entities.QueryEntities<T>(egid.groupID);
            ref T component = ref components[index];

            foreach (ISerializationEngine<T> engine in _engines)
            {
                engine.Serialize(in component, writer);
            }
        }

        public override void Deserialize(in VhId seed, EntityReader reader, ref EntityInitializer initializer, bool confirmed)
        {
            ref T component = ref initializer.Get<T>();
            foreach (ISerializationEngine<T> engine in _engines)
            {
                engine.Deserialize(in seed, reader, ref component, confirmed);
            }
        }
    }
}
