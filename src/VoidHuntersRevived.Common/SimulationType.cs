using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Components;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Common
{
    public abstract class SimulationType
    {
        private static readonly IDictionary<string, SimulationType> _names = new Dictionary<string, SimulationType>();
        private static readonly IList<SimulationType> _list = new List<SimulationType>();

        public static readonly ReadOnlyCollection<SimulationType> Instances = new ReadOnlyCollection<SimulationType>(_list);
        public static readonly SimulationType Lockstep = new SimulationType<Lockstep>(SimulationTypes.Lockstep, new Lockstep());
        public static readonly SimulationType Predictive = new SimulationType<Predictive>(SimulationTypes.Predictive, new Predictive());

        public readonly byte Flag;
        public readonly string Name;
        public readonly Type EntityComponentType;

        internal SimulationType(string name, Type entityComponent)
        {
            this.Flag = (byte)Math.Pow(2, _list.Count);
            this.Name = name;
            this.EntityComponentType = entityComponent;

            _list.Add(this);
            _names.Add(this.Name, this);
        }

        public abstract Entity AttachComponent(Entity entity);

        public static SimulationType GetByName(string name)
        {
            return _names[name];
        }
    }

    public sealed class SimulationType<TComponent> : SimulationType
        where TComponent : class
    {
        public readonly TComponent EntityComponent;

        public SimulationType(string name, TComponent entityComponent) : base(name, typeof(TComponent))
        {
            this.EntityComponent = entityComponent;
        }

        public override Entity AttachComponent(Entity entity)
        {
            entity.Attach(this.EntityComponent);

            return entity;
        }
    }
}
