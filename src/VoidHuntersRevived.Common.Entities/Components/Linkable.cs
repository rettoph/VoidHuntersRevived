using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed partial class Linkable : IShipPartComponent, IEnumerable<Linkable.Joint>
    {
        private List<Linkable.Joint> _joints = new();

        public Linkable.Joint this[int index] => _joints[index];

        public void Add(Linkable.Joint joint)
        {
            _joints.Add(joint);
        }

        public IEnumerator<Joint> GetEnumerator()
        {
            return _joints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
