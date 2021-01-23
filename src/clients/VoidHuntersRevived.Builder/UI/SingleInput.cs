using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VoidHuntersRevived.Builder.UI
{
    public class SingleInput : SimpleInput
    {
        #region Public Properties
        public new virtual Single Value
        {
            get => this.GetValue(base.Value);
            set => base.Value = value.ToString("#,##0.#####");
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
