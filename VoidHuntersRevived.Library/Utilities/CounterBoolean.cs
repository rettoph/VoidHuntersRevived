using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple counter that acts mainly as a boolean
    /// </summary>
    public class CounterBoolean
    {
        private List<Guid> _counter;

        public Boolean Value { get; private set; }

        public event EventHandler<Boolean> OnValueUpdated;

        public CounterBoolean()
        {
            _counter = new List<Guid>();
        }

        public Guid Add()
        {
            var guid = Guid.NewGuid();

            _counter.Add(guid);
            this.UpdateValue();

            return guid;
        }

        public void Remove(Guid guid)
        {
            if (!_counter.Contains(guid))
                throw new Exception($"Unable to remove value from counter. Unknown counter id({guid})!");

            _counter.Remove(guid);
            this.UpdateValue();
        }

        private void UpdateValue()
        {
            if(this.Value != _counter.Count > 0)
            {
                this.Value = !this.Value;
                this.OnValueUpdated?.Invoke(this, this.Value);
            }
        }

        public static implicit operator Boolean(CounterBoolean cb)
        {
            return cb.Value;
        }
    }
}
