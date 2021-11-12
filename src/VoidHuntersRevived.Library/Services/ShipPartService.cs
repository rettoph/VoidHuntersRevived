using Guppy;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using System.Text.Json;
using System.IO;
using Guppy.DependencyInjection;
using VoidHuntersRevived.Library.Json.JsonConverters.Contexts.ShipParts;
using Guppy.Extensions.System;
using Guppy.Events.Delegates;
using Guppy.Lists;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Json.JsonConverters;
using VoidHuntersRevived.Library.Json.JsonConverters.Utilities;
using tainicom.Aether.Physics2D.Collision.Shapes;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Extensions.Aether;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using Path = System.IO.Path;
using VoidHuntersRevived.Library.Dtos.Utilities;
using System.Linq;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// A simple service utility used to create
    /// <see cref="ShipPart"/> instances out of
    /// <see cref="ShipPartContext"/>s. New Dto's
    /// should be registered on startup.
    /// </summary>
    public class ShipPartService : ServiceList<ShipPart>
    {
        #region Private Fields
        private Dictionary<UInt32, ShipPartContext> _contexts;
        private GuppyServiceProvider _provider;
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
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;
            _contexts = new Dictionary<UInt32, ShipPartContext>();

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
        public ShipPart Create(UInt32 contextId, Guid? id = default)
        {
            ShipPartContext context = _contexts[contextId];

            return this.Create<ShipPart>(_provider, context.ShipPartServiceConfigurationKey, (sp, p, c) =>
            {
                sp.SetContext(context);
            }, id);
        }

        /// <summary>
        /// Create a fresh new <see cref="ShipPart"/> instance from the
        /// recieved context name, assuming that a context with that name 
        /// has been registered.
        /// </summary>
        /// <param name="contextName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShipPart Create(String contextName, Guid? id = default)
            => this.Create(contextName.xxHash(), id);
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
            foreach (ShapeDto shape in context.Shapes)
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

        #region
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

        #region Network Methods
        public void TryWriteShipPart(ShipPart shipPart, NetOutgoingMessage om, ShipPartSerializationFlags flags)
        {
            this.log.Info($"{nameof(ShipPartService)}::{nameof(TryWriteShipPart)} => flags: {flags}, shipPartId: {shipPart.Id}, children: {shipPart.GetChildren().Count()}");

            this.WriteShipPart(shipPart, om, flags);
        }

        public ShipPart TryReadShipPart(NetIncomingMessage im, ShipPartSerializationFlags flags)
        {
            ShipPart parent = this.ReadShipPart(im, flags);

            this.log.Info($"{nameof(ShipPartService)}::{nameof(TryReadShipPart)} => flags: {flags}, shipPartId: {parent.Id}, children: {parent.GetChildren().Count()}");

            return parent;
        }

        private void WriteShipPart(ShipPart shipPart, NetOutgoingMessage om, ShipPartSerializationFlags flags)
        {
            om.Write(shipPart.Context.Id);
            om.Write(shipPart.Id);

            if (flags.HasFlag(ShipPartSerializationFlags.Create))
            {
                // Notice we include a position value. This is
                // So the reader can skip the WriteAll data if needed.
                // The Create data is only utilized if no instance is discovered
                // on the recieving peer.
                Int32 startPosition = om.LengthBits;
                om.Write(startPosition);

                shipPart.WriteCreate(om);

                om.WriteAt(startPosition, om.LengthBits);
            }

            if (flags.HasFlag(ShipPartSerializationFlags.Tree))
            {
                // Iterate through each parent connection node & broadcast all internal data.
                foreach (ConnectionNode node in shipPart.ConnectionNodes)
                {
                    if (node.Connection.State == ConnectionNodeState.Parent)
                    {
                        om.Write((Byte)ConnectionNodeState.Parent);

                        this.WriteShipPart(node.Connection.Target.Owner, om, flags);

                        om.Write(node.Index);
                        om.Write(node.Connection.Target.Index);
                    }
                    else if (node.Connection.State == ConnectionNodeState.Estranged)
                    {
                        om.Write((Byte)ConnectionNodeState.Estranged);
                        om.Write(node.Index);
                    }
                }

                // For the purposes of reading node data we use the child marker to signify a complete node message.
                om.Write((Byte)ConnectionNodeState.Child);
            }
        }

        private ShipPart ReadShipPart(NetIncomingMessage im, ShipPartSerializationFlags flags)
        {
            UInt32 contextId = im.ReadUInt32();
            Guid shipPartId = im.ReadGuid();
            ShipPart parent = this.GetById(shipPartId);


            if (flags.HasFlag(ShipPartSerializationFlags.Create))
            {
                Int64 endPosition = im.ReadInt32();

                if (parent == default)
                {
                    parent = this.Create(contextId, shipPartId);
                    parent.ReadCreate(im);
                }
                else
                {
                    // Skip the ReadAll data
                    im.Position = endPosition;
                }
            }

            if(flags.HasFlag(ShipPartSerializationFlags.Tree))
            {
                ConnectionNodeState state;
                while ((state = (ConnectionNodeState)im.ReadByte()) != ConnectionNodeState.Child)
                {
                    switch (state)
                    {
                        case ConnectionNodeState.Parent:
                            ShipPart child = this.ReadShipPart(im, flags);

                            Int32 parentNodeIndex = im.ReadInt32();
                            ConnectionNode parentNode = parent.ConnectionNodes[parentNodeIndex];

                            Int32 childNodeIndex = im.ReadInt32();
                            ConnectionNode childNode = child.ConnectionNodes[childNodeIndex];

                            parentNode.TryAttach(childNode);
                            break;
                        case ConnectionNodeState.Estranged:
                            Int32 estrangedNodeIndex = im.ReadInt32();
                            ConnectionNode estrangedNode = parent.ConnectionNodes[estrangedNodeIndex];
                            estrangedNode.TryDetach();
                            break;
                    }
                }
            }

            return parent;
        }
        #endregion
    }
}
