using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts;

namespace VoidHuntersRevived.Common.Editor
{
    [GuppyFilter<IEditorGuppy>]
    public interface IShipPartEditor
    {
        Type ComponentConfigurationType { get; }

        void Initialize(ShipPartResource shipPart);

        void Draw(GameTime gameTime);

        void Update(GameTime gameTime);
    }

    public interface IShipPartEditor<TComponentConfiguration> : IShipPartEditor
    {

    }
}
