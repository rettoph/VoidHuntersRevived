using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class BooleanInput : SimpleInput<Boolean>
    {
        public override Boolean Value
        {
            get => this.input.Value == "1";
            set => this.input.Value = value ? "1" : "0";
        }

        #region Lifecycle Methods 
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.input.Filter = new Regex("^[1|0]{0,1}$");
        }
        #endregion
    }
}
