﻿using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Dtos.Utilities;

namespace VoidHuntersRevived.Library.Contexts.ShipParts
{
    /// <summary>
    /// Defines static global properties relevant
    /// to a ShipPart type. This will be serialized
    /// & saved as JSON within a .vhsp file for easy editing.
    /// </summary>
    public abstract class ShipPartContext
    {
        #region Abstract Properties
        /// <summary>
        /// The service configuration key to be used
        /// when creating a new instance of the defined <see cref="ShipPart"/>.
        /// </summary>
        protected abstract ServiceConfigurationKey shipPartServiceConfigurationKey { get; }
        #endregion

        #region Public Properties
        /// <summary>
        /// The service configuration key to be used
        /// when creating a new instance of the defined <see cref="ShipPart"/>.
        /// </summary>
        [JsonIgnore]
        public ServiceConfigurationKey ShipPartServiceConfigurationKey => this.shipPartServiceConfigurationKey;

        /// <summary>
        /// The cross platform unique key for this context.
        /// </summary>
        [JsonIgnore]
        public UInt32 Id => this.Name.xxHash();

        /// <summary>
        /// A unique name linked to this context instance.
        /// </summary>
        [JsonPropertyOrder(-1)]
        public String Name { get; set; }

        /// <summary>
        /// Defines the shapes of the ShipPart
        /// </summary>
        public Shape[] Shapes { get; set; } = new Shape[0];

        /// <summary>
        /// All connection nodes within the current part.
        /// </summary>
        public ConnectionNodeDto[] ConnectionNodes { get; set; } = new ConnectionNodeDto[0];

        /// <summary>
        /// A custom "center" for the current part. This is where
        /// the tractor beam will select a piece.
        /// </summary>
        public Vector2 Centeroid { get; set; } = Vector2.Zero;
        #endregion
    }
}
