using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Builder.Contexts;
using VoidHuntersRevived.Builder.UI.Inputs;

namespace VoidHuntersRevived.Builder.UI
{
    public class ShapeEditorMenu : SecretContainer<IElement>
    {
        #region Private Fields
        private StackContainer _stack;
        private List<SideContextInput> _sideInputs;
        private ShapeTransformationsInput _transformations;
        #endregion

        #region Internal Properties
        internal ShapeContext shape { get; set; }
        #endregion

        #region Public Properties
        public IEnumerable<SideContext> SideContexts => _sideInputs.Select(si => si.SideContext);

        public ShapeTransformationsInput Transformations => _transformations;
        public TextElement DeleteButton { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _stack = this.inner.Children.Create<StackContainer>((stack, p, c) =>
            {
                stack.Alignment = StackAlignment.Vertical;
            });

            this.DeleteButton = _stack.Children.Create<TextElement>("ui:button:0", (delete, p, c) =>
            {
                delete.Value = "Delete Shape";
                delete.Bounds.Width = 1f;
            });
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _transformations = _stack.Children.Create<ShapeTransformationsInput>((transformations, p, c) =>
            {
                transformations.Translation = this.shape.Translation;
                transformations.Rotation = this.shape.Rotation;
                transformations.Scale = this.shape.Scale;
            });

            _sideInputs = new List<SideContextInput>();

            // Create a new row for every single shape side...
            foreach (SideContext side in this.shape.Sides)
            {
                _sideInputs.Add(_stack.Children.Create<SideContextInput>((input, p, c) =>
                {
                    input.source = side;
                }));
            }
        }

        protected override void Release()
        {
            base.Release();

            _sideInputs.Clear();

            _transformations = null;
            _stack = null;
            this.DeleteButton = null;
        }
        #endregion
    }
}
