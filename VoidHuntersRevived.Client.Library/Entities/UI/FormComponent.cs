using Guppy.UI.Entities.UI;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    public class FormComponent : StyleElement
    {
        private TextElement _label;
        private TextInput _input;

        public String Label { get => _label.Text; set => _label.Text = value; }
        public String Value { get => _input.Text; set => _input.Text = value; }

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Bounds.Set(0, 0, 1f, 70);
            this.BorderSize = 0;

            _label = this.add<TextElement>(t =>
            {
                t.Bounds.Set(15, 15, new Unit[] { 1f, -30 }, 20);
                t.TextAlignment = BaseElement.Alignment.CenterLeft;
                t.TextColor = new Color(222, 229, 229);
            });

            _input = this.add<TextInput>(i =>
            {
                i.Bounds.Set(15, 40, new Unit[] { 1f, -30 }, 30);
                i.BorderColor = new Color(210, 216, 216);
                i.BackgroundColor = new Color(222, 229, 229);
                i.TextColor = Color.Black;
            });
        }
        #endregion
    }
}
