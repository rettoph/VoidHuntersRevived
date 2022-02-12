using Guppy;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using System.Text.Json;
using System.IO;
using Guppy.EntityComponent.DependencyInjection;
using VoidHuntersRevived.Library.Json.JsonConverters.Contexts.ShipParts;
using Guppy.Extensions.System;
using Guppy.Events.Delegates;
using Guppy.EntityComponent.Lists;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Json.JsonConverters;
using VoidHuntersRevived.Library.Json.JsonConverters.Utilities;
using tainicom.Aether.Physics2D.Collision.Shapes;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Extensions.Aether;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Library.Contexts.Utilities;
using System.Linq;
using LiteNetLib.Utils;
using Path = System.IO.Path;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using Guppy.EntityComponent;
using Guppy.Network.Services;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Structs;
using Minnow.System.Helpers;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// A simple service utility used to create
    /// <see cref="ShipPart"/> instances out of
    /// <see cref="ShipPartContext"/>s. New Dto's
    /// should be registered on startup.
    /// </summary>
    public class ShipPartService : Service
    {
        #region Private Fields
        private ServiceProvider _provider;
        private Dictionary<UInt32, ShipPartContext> _contexts;
        private NetworkEntityService _networkEntities;
        #endregion

        #region Public Properties
        /// <summary>
        /// The <see cref="JsonSerializerOptions"/> used internally when
        /// attempting to deserialize incoming JSON dto instances. When creating
        /// a brand new <see cref="ShipPartContext"/> child type, a Json serializer
        /// for that type should also be registered.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        /// <summary>
        /// A collection of all contexts registed.
        /// </summary>
        public IReadOnlyDictionary<UInt32, ShipPartContext> RegisteredContexts => _contexts;
        #endregion

        #region Events
        public OnEventDelegate<ShipPartService, ShipPartContext> OnContextRegistered;
        #endregion

        #region Initialization Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;
            _contexts = new Dictionary<UInt32, ShipPartContext>();

            provider.Service(out _networkEntities);

            this.JsonSerializerOptions = new JsonSerializerOptions()
            {
                Converters = {
                    new ShipPartContextJsonConverter(),
                    new ShapeJsonConverter(),
                    new ConnectionNodeDtoJsonConverter(),
                    new Vector2JsonConverter()
                },
                WriteIndented = true
            };
        }
        #endregion

        #region Creation Methods
        /// <summary>
        /// Create a fresh new <see cref="ShipPart"/> instance from the
        /// recieved context id, assuming that a context with that id has
        /// been registered.
        /// </summary>
        /// <param name="contextId"></param>
        /// <param name="id">The service id, if any</param>
        /// <returns></returns>
        private ShipPart Create(UInt32 contextId, UInt16 networkId)
        {
            ShipPartContext context = _contexts[contextId];

            return _provider.GetService<ShipPart>(context.ShipPartServiceConfigurationType, (sp, p, c) =>
            {
                sp.SetContext(context);
                sp.NetworkId = networkId;
            });
        }

        /// <summary>
        /// Create a fresh new <see cref="ShipPart"/> instance from the
        /// recieved context id, assuming that a context with that id has
        /// been registered.
        /// </summary>
        /// <param name="contextId"></param>
        /// <param name="id">The service id, if any</param>
        /// <returns></returns>
        public ShipPart Create(UInt32 contextId)
        {
            ShipPartContext context = _contexts[contextId];

            return _provider.GetService<ShipPart>(context.ShipPartServiceConfigurationType, (sp, p, c) =>
            {
                sp.SetContext(context);
            });
        }

        /// <summary>
        /// Create a fresh new <see cref="ShipPart"/> instance from the
        /// recieved context name, assuming that a context with that name 
        /// has been registered.
        /// </summary>
        /// <param name="contextName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShipPart Create(String contextName)
            => this.Create(contextName.xxHash());
        #endregion

        #region Registration Methods
        /// <summary>
        /// Register a new context instance.
        /// </summary>
        /// <param name="context"></param>
        public void RegisterContext(ShipPartContext context)
        {
            if(this.ValidateContext(context))
            {
                _contexts[context.Id] = context;
                this.OnContextRegistered?.Invoke(this, context);
            }
        }

        /// <summary>
        /// Parse an input JSON string and register
        /// the defined context.
        /// </summary>
        /// <param name="contextJson"></param>
        public void RegisterJsonContext(String contextJson)
            => this.RegisterContext(this.DeserializeContext(contextJson));

        /// <summary>
        /// Attempt to parse a recieved JSON file & 
        /// register the defined context.
        /// </summary>
        /// <param name="pathToFile"></param>
        public void RegisterFileContext(String pathToFile)
            => this.RegisterJsonContext(File.ReadAllText(pathToFile));

        /// <summary>
        /// Register multiple files at once.
        /// </summary>
        /// <param name="pathsToFiles"></param>
        public void RegisterFileContexts(params String[] pathsToFiles)
        {
            foreach (String pathToFile in pathsToFiles)
                this.RegisterFileContext(pathToFile);
        }

        /// <summary>
        /// Register all files within a recieved directory.
        /// </summary>
        /// <param name="pathToDirectory"></param>
        public void RegisterDirectoryContexts(String pathToDirectory)
            => this.RegisterFileContexts(Directory.GetFiles(pathToDirectory, "*.vhsp"));
        #endregion

        #region Validation Methods
        private Boolean ValidateContext(ShipPartContext context)
        {
            foreach (ShapeContext shape in context.Shapes)
            {
                String err;
                Boolean shapeValid = shape.Data switch
                {
                    PolygonShape polygon => this.ValidatePolygonShape(polygon, out err),
                    _ => throw new ArgumentOutOfRangeException(""),
                };

                if (!shapeValid)
                {
                    throw new Exception($"Error validating ShipPartContext('{context.Name}') => {err}");
                }
            }

            return true;
        }

        private Boolean ValidatePolygonShape(PolygonShape polygon, out String error)
        {
            Vertices vertices = polygon.Vertices.Clone();

            if (vertices.Count <= 3)
                vertices.ForceCounterClockWise();
            else
                vertices = GiftWrap.GetConvexHull(vertices);

            // Ensure the vertices still match
            for (Int32 i = 0; i < vertices.Count; i++)
            {
                if (vertices[i] != polygon.Vertices[i])
                {
                    error = "PolygonShape vertices are not properly gift-wrapped.";
                    return false;
                }
            }

            error = default;
            return true;
        }
        #endregion

        #region I/O Methods
        public ShipPartContext DeserializeContext(String contextJson)
            => JsonSerializer.Deserialize<ShipPartContext>(contextJson, this.JsonSerializerOptions);

        public String SerializeContext(ShipPartContext context)
            => JsonSerializer.Serialize<ShipPartContext>(context, this.JsonSerializerOptions);

        public void ExportAll(String path)
        {
            foreach(ShipPartContext context in this.RegisteredContexts.Values)
            {
                this.Export(context, path);
            }
        }

        public void Export(ShipPartContext context, String path)
        {
            Directory.CreateDirectory(path);

            String json = this.SerializeContext(context);
            String filename = $"{String.Join('.', context.Name.Split(Path.GetInvalidFileNameChars()))}.vhsp";

            File.WriteAllText(Path.Combine(path, filename), json);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get the defined ship part, or create it if possible.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Boolean TryGet(ShipPartPacket packet, out ShipPart shipPart)
        {
            if(packet is null)
            {
                return MethodHelper.FailResponse(out shipPart);
            }

            // Create the target if possible...
            if (!this.TryGetByNetworkId(packet.NetworkId, out shipPart))
            {
                if ((packet.SerializationFlags & ShipPartSerializationFlags.Create) != 0)
                {
                    shipPart = this.Create(packet.ContextId.Value, packet.NetworkId);
                }
                else
                {
                    return MethodHelper.FailResponse(out shipPart);
                }
            }

            if ((packet.SerializationFlags & ShipPartSerializationFlags.Tree) != 0)
            {
                foreach(ConnectionNodeDto node in packet.Children)
                {
                    switch (node.State)
                    {
                        case ConnectionNodeState.Estranged:
                            ConnectionNode estrangedNode = shipPart.ConnectionNodes[node.Index];
                            estrangedNode.TryDetach();
                            break;
                        case ConnectionNodeState.Parent:
                            if (this.TryGet(node.TargetNodeOwner, out ShipPart child))
                            {
                                ConnectionNode parentNode = shipPart.ConnectionNodes[node.Index];
                                ConnectionNode childNode = child.ConnectionNodes[node.TargetNodeIndex.Value];
                                parentNode.TryAttach(childNode);
                            }
                            break;
                        case ConnectionNodeState.Child:
                            throw new NotImplementedException();
                    }
                }
            }

            return true;
        }

        public Boolean TryGetByNetworkId(UInt16? networkId, out ShipPart shipPart)
        {
            if(!networkId.HasValue)
            {
                shipPart = default;
                return false;
            }

            return _networkEntities.TryGetByNetworkId(networkId.Value, out shipPart);
        }

        public Boolean TryGetConnectionNodeByNetworkId(ConnectionNodeNetworkId? networkId, out ConnectionNode node)
        {
            Boolean FailResponse(out ConnectionNode node)
            {
                node = default;
                return false;
            }

            if(!networkId.HasValue)
            {
                return FailResponse(out node);
            }

            if(!this.TryGetByNetworkId(networkId.Value.OwnerNetworkId, out ShipPart owner))
            {
                return FailResponse(out node);
            }

            return owner.ConnectionNodes.TryGetElementAt(networkId.Value.Index, out node);
        }
        #endregion
    }
}
