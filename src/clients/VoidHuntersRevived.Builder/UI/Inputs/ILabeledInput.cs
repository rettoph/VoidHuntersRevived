using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public interface ILabeledInput
    {
        event OnEventDelegate<ILabeledInput, Object> OnValueChanged;

        String Label { get; set; }
        Object Value { get; set; }
    }
}
