using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public interface ILabeledInput
    {
        String Label { get; set; }
        Object Value { get; set; }
    }
}
