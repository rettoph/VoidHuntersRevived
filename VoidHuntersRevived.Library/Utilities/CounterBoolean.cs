using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Experimental class used to "reserve" a state.
    /// 
    /// The value of 0 is false, any other value is true.
    /// </summary>
    public sealed class CounterBoolean
    {
        #region Private Fields
        private HashSet<Guid> _reservations;
        private Action<Boolean> _changed;
        private Boolean _oldValue;
        #endregion

        #region Delegaters
        public delegate void CounterBooleanChanged(Boolean value);
        #endregion

        #region Public Attributes
        public Boolean Value { get { lock (_reservations) return _reservations.Any(); } }
        #endregion

        #region Constructor
        public CounterBoolean(Action<Boolean> changed = null)
        {
            _changed = changed;
            _reservations = new HashSet<Guid>();
            _oldValue = false;
        }
        #endregion

        #region Add & Remove Methods
        public Guid Add()
        {
            lock (_reservations)
            {
                var reservation = Guid.NewGuid();
                _reservations.Add(reservation);
                this.CleanChanged();

                return reservation;
            }
        }

        public void Remove(Guid reservationId)
        {
            lock (_reservations)
            {
                _reservations.Remove(reservationId);
                this.CleanChanged();
            }
        }
        #endregion

        private void CleanChanged()
        {
            if(_oldValue != this.Value)
            { // If the value has changed...

                // Update the old value & invoke the changed handler
                _oldValue = !_oldValue;
                _changed?.Invoke(_oldValue);
            }
        }
    }
}
