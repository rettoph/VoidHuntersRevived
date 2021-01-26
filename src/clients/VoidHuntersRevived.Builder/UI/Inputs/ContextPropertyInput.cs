using Guppy.UI.Elements;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Extensions.System.Reflection;
using Guppy.DependencyInjection;
using Guppy.UI.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    /// <summary>
    /// The primary input source for all context properties.
    /// This will dynamically create a pretty UI based
    /// on the input type.
    /// </summary>
    public class ContextPropertyInput : SecretContainer<IElement>, ILabeledInput
    {
        #region Private Fields
        private ILabeledInput _input;
        private ShipPartContext _context;
        private PropertyInfo _property;
        #endregion

        #region Public Properties
        public String Label { get => _input.Label; set => _input.Label = value; }
        public Object Value { get => _input.Value; set => _input.Value = value; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Vertical;
            this.Padding.Bottom = 15;
            this.Bounds.Height = 0;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_input != default)
                _property.SetValue(_context, _input.Value);
        }
        #endregion

        #region API Methods
        public void LoadProperty(ShipPartContext context, PropertyInfo property)
        {
            var attribute = property.GetAttribute<ShipPartContextPropertyAttribute>();

            switch (attribute.Type)
            {
                case ShipPartContextPropertyType.String:
                    _input = this.inner.Children.Create<StringInput>();
                    break;
                case ShipPartContextPropertyType.Single:
                    _input = this.inner.Children.Create<SingleInput>();
                    break;
                case ShipPartContextPropertyType.Radian:
                    _input = this.inner.Children.Create<RadianInput>();
                    break;
                case ShipPartContextPropertyType.Color:
                    _input = this.inner.Children.Create<ColorInput>();
                    break;
                case ShipPartContextPropertyType.Boolean:
                    _input = this.inner.Children.Create<BooleanInput>();
                    break;
                case ShipPartContextPropertyType.Vector2:
                    _input = this.inner.Children.Create<Vector2Input>();
                    break;
                default:
                    _input = default;
                    break;
            }

            if (_input == default)
                return;

            _input.Label = attribute.Name;
            _input.Value = property.GetValue(context);

            _property = property;
            _context = context;
        }
        #endregion
    }
}
