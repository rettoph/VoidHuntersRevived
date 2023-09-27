﻿using Guppy.Attributes;
using Guppy.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    [AutoLoad]
    public class VisibleFillEffect : EffectMatricesEffect
    {
        private EffectParameter _color;

        public Color Color
        {
            set => _color.SetValue(value.ToVector4());
        }

        public VisibleFillEffect(GraphicsDevice graphics, IResourceProvider resources) : base(graphics, resources.Get(Resources.EffectCodes.VisibleFill))
        {
            _color = this.Parameters["Color"];
        }
    }
}
