using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class StringInput : SimpleInput<String>
    {
        public override string Value
        {
            get => this.input.Value;
            set => this.input.Value = value;
        }
    }
}
