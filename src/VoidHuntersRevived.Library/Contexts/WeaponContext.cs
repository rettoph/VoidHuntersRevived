using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts
{
    public abstract class WeaponContext : ShipPartContext
    {
        #region Public Properties
        public Single SwivelRange { get; set; }
        #endregion

        #region Constructors
        protected WeaponContext(string name) : base(name)
        {
        }
        #endregion
    }
}
