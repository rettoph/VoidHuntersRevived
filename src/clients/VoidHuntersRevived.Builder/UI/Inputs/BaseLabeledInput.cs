using Guppy.UI.Elements;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.System;
using Guppy.DependencyInjection;
using Guppy.UI.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public abstract class BaseLabeledInput<T> : SecretContainer<IElement>, ILabeledInput
    {
        #region ILabeledInput Implementation
        String ILabeledInput.Label { get => this.Label; set => this.Label = value; }
        Object ILabeledInput.Value
        {
            get => this.Value;
            set
            {
                if (value is T casted)
                    this.Value = casted;
                else
                    throw new ArgumentException($"Invalid arumgent type, {value.GetType().GetPrettyName()}, expected {typeof(T).GetPrettyName()}.");
            }
        }
        #endregion

        #region Protected Properties
        protected TextElement label { get; private set; }
        #endregion

        #region Public Properties
        public String Label
        {
            get => this.label.Value;
            set => this.label.Value = value;
        }

        public abstract T Value { get; set; }
        #endregion

        #region Events
        public event OnEventDelegate<ILabeledInput, Object> OnValueChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Vertical;

            this.label = this.inner.Children.Create<TextElement>((label, p, c) =>
            {
                label.Color[ElementState.Default] = p.GetColor("ui:color:1");
                label.Value = "Label";
                label.Inline = InlineType.Both;
            });
        }

        protected override void Release()
        {
            base.Release();

            this.label = null;
        }
        #endregion

        #region Helper Methods
        protected void HandleValueChanged(TextElement sender, string args)
            => this.OnValueChanged?.Invoke(this, this.Value);
        #endregion
    }
}
