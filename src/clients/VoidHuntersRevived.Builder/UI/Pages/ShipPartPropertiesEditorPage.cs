using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VoidHuntersRevived.Builder.UI.Inputs;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Builder.UI.Pages
{
    /// <summary>
    /// The primary page to manage context properties
    /// </summary>
    public class ShipPartPropertiesEditorPage : SecretContainer<IElement>, IPage
    {
        #region Private Fields
        private StackContainer _left;
        private StackContainer _right;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _left = this.inner.Children.Create<StackContainer>((stack, p, c) =>
            {
                stack.Inline = InlineType.Vertical;
                stack.Alignment = StackAlignment.Vertical;
                stack.Bounds.Width = 300;
                stack.Bounds.X = new CustomUnit(c => (c / 2) - 300);
                stack.Padding.Left = 15;
                stack.Padding.Right = 15;
            });

            _right = this.inner.Children.Create<StackContainer>((stack, p, c) =>
            {
                stack.Inline = InlineType.Vertical;
                stack.Alignment = StackAlignment.Vertical;
                stack.Bounds.Width = 300;
                stack.Bounds.X = 0.5f;
                stack.Padding.Left = 15;
                stack.Padding.Right = 15;
            });
        }

        protected override void Release()
        {
            this.UnloadProperties();

            base.Release();

            _left = null;
            _right = null;
        }
        #endregion

        #region Helper Methods
        public void LoadProperties(ShipPartContext context, List<PropertyInfo> properties)
        {
            for(Int32 i=0; i<properties.Count; i++)
            {
                var stack = i % 2 == 0 ? _left : _right;

                stack.Children.Create<ContextPropertyInput>((input, p, c) => input.LoadProperty(context, properties[i]));
            }
        }

        public void UnloadProperties()
        {
            while (_left.Children.Any())
                _left.Children.First().TryRelease();

            while (_right.Children.Any())
                _right.Children.First().TryRelease();
        }
        #endregion
    }
}
