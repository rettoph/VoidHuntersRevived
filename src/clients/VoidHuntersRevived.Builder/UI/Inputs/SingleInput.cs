using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class SingleInput : SimpleInput<Single>
    {
        #region Public Properties
        public override Single Value
        {
            get => this.GetValue(this.input.Value);
            set => this.input.Value = value.ToString("#,##0.#####");
        }

        public Single DefaultValue { get; set; } = 0f;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.input.Filter = new Regex("^[-]{0,1}[0-9\\.]{0,10}$");
        }
        #endregion

        #region Helper Methods
        protected virtual Single GetValue(String input)
        {
            Single output;
            if (Single.TryParse(input, out output))
                return output;

            return this.DefaultValue;
        }
        #endregion
    }
}
