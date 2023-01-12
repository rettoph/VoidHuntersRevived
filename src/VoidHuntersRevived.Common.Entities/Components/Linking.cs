using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed partial class Linking : IShipPartComponent, IEnumerable<Linking.Joint>
    {
        private List<Linking.Joint> _joints = new();

        public void Add(Linking.Joint joint)
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
