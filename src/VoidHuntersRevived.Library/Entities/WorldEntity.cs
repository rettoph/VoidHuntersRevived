using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using Guppy.IO.Commands.Services;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.Extensions.System;
using Guppy.Utilities.ObjectDumper;
using System.Reflection;
using log4net;
using FarseerPhysics.Dynamics.Joints;
using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Lists;
using Guppy.Network.Utilities.Messages;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Entities
{
    public sealed class WorldEntity : FarseerEntity<World>
    {
        #region Static Attributes
        public static Int32 WallWidth { get; } = 10;
        #endregion

        #region Private Fields
        private Vector2 _size;
        private Queue<Body> _walls;
        private CommandService _commands;
        private ILog _log;
        private GameScene _scene;
        #endregion

        #region Public Attributes
        public World Master => this.master;
        public World Slave => this.slave;
        public World Live => this.live;

        public Vector2 Size
        {
            get => _size;
            set => this.OnSizeChanged.InvokeIfChanged(_size != value, this, ref _size, value);
        }
        #endregion

        #region Events
        public event OnEventDelegate<WorldEntity, Vector2> OnSizeChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.MessageHandlers[MessageType.Setup].Add(this.ReadSize, this.WriteSize);

            this.OnSizeChanged += this.HandleSizeChanged;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commands);
            provider.Service(out _log);
            provider.Service(out _scene);

            _walls = new Queue<Body>();

            this.Size = new Vector2(128, 128);

            this.UpdateOrder = -100;

            _commands["world"]["info"].OnExcecute += this.HandleWorldInfoCommand;
        }

        protected override void Release()
        {
            base.Release();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.MessageHandlers[MessageType.Setup].Remove(this.ReadSize, this.WriteSize);

            this.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region Factory Methods
        protected override World BuildMaster(ServiceProvider provider)
            => new World(Vector2.Zero);

        protected override World BuildSlave(ServiceProvider provider)
            => new World(Vector2.Zero);
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            this.Do(w =>
            {
                w.Step((Single)gameTime.ElapsedGameTime.TotalSeconds);
            });
        }
        #endregion

        #region Command Handlers
        private CommandResponse HandleWorldInfoCommand(ICommand sender, CommandInput input)
        {
            String line = "";

            ObjectDumper.Dump(
                (text, segment) =>
                {
                        // if (text.StartsWith("\n"))
                        // {
                        //     _log.Info(line.TrimStart('\n'));
                        //     line = "";
                        // }
                        // 
                        // line += text;
                        // 
                        // if (text.EndsWith("\n"))
                        // {
                        //     _log.Info(line.TrimEnd('\n'));
                        //     line = "";
                        // }

                        switch (segment)
                    {
                        case ObjectDumper.DumpSegment.Scaffold:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case ObjectDumper.DumpSegment.Name:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case ObjectDumper.DumpSegment.MemberType:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case ObjectDumper.DumpSegment.Type:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case ObjectDumper.DumpSegment.Value:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case ObjectDumper.DumpSegment.Children:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case ObjectDumper.DumpSegment.Exception:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                    }

                    Console.Write(text);
                },
                (_scene.Entities.First(e => e is Ship) as Ship).Bridge.master,
                input.GetIfContains<Int32>("depth"),
                input.GetIfContains<MemberTypes>("members"),
                typeof(Assembly),
                typeof(World),
                typeof(JointEdge),
                typeof(Type),
                typeof(Layer),
                typeof(EntityList),
                typeof(LayerList),
                typeof(Scene),
                typeof(Chain),
                typeof(ILog),
                typeof(Settings),
                typeof(MessageManager),
                typeof(NetworkEntityMessageTypeHandler),
                typeof(ChunkManager),
                typeof(IEnumerable<ConnectionNode>),
                typeof(Matrix),
                typeof(ServiceFactory),
                typeof(ShipPartConfiguration),
                typeof(ShipController),
                typeof(IList<ConnectionNode>),
                typeof(ServiceProvider),
                typeof(Player),
                typeof(MethodInfo));

            return CommandResponse.Empty;
        }
        #endregion

        #region Network Methods
        internal void WriteSize(NetOutgoingMessage om)
            => om.Write(this.Size);

        internal void ReadSize(NetIncomingMessage im)
            => this.Size = im.ReadVector2();
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
        {
            this.log.Info($"World Size Changed => {this.Size}");

            // Destroy any pre-existing walls...
            while (_walls.Any())
                _walls.Dequeue().Dispose();

            // Create brand new walls...
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(-WorldEntity.WallWidth / 2, this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(this.Size.X + (WorldEntity.WallWidth / 2), this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, -WorldEntity.WallWidth / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, this.Size.Y + (WorldEntity.WallWidth / 2)), 0, BodyType.Static)));
            
            _walls.ForEach(b =>
            { // Setup wall collisions
                b.CollisionCategories = Categories.BorderCollisionCategories;
                b.CollidesWith = Categories.BorderCollidesWith;
                b.IgnoreCCDWith = Categories.BorderIgnoreCCDWith;

                b.Restitution = 1f;
                b.Friction = 0f;
            });
        }
        #endregion
    }
}
