﻿using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
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
using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Lists;
using Guppy.Network.Utilities.Messages;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.Players;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Common.PhysicsLogic;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Entities.ShipParts;
using tainicom.Aether.Physics2D.Common;
using Guppy.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    public sealed class WorldEntity : AetherEntity<World>
    {
        #region Static Attributes
        public static Int32 WallWidth { get; } = 10;
        #endregion

        #region Private Fields
        private Vector2 _size;
        private Queue<Body> _walls;
        private CommandService _commands;
        private GameScene _scene;
        private Synchronizer _synchronizer;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The "real" instance of the current Farseer entity.
        /// 
        /// This should be changed to reflect rigid/instant states.
        /// </summary>
        public World Master => this.master;

        /// <summary>
        /// The "visible" instance of the current Farseer entity.
        /// 
        /// This should only be updated internally via the UpdateSlave method or
        /// OnUpdateSlave event. This usually appears as a slow lerp towards the
        /// perfection of the master instance.
        /// </summary>
        public World Slave => this.slave;

        /// <summary>
        /// The "live" instance. This is the master if there is no slave,
        /// otherwise it will be the slave.
        /// 
        /// This is used to grab the current state of the entity for rendering
        /// or informational purposes.
        /// </summary>
        public World Live => this.live;

        public Vector2 Size
        {
            get => _size;
            set => this.OnSizeChanged.InvokeIf(_size != value, this, ref _size, value);
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
            provider.Service(out _scene);
            provider.Service(out _synchronizer);

            _walls = new Queue<Body>();

            this.Size = new Vector2(128, 128);

            this.UpdateOrder = 100;

            _commands["world"]["info"].OnExcecute += this.HandleWorldInfoCommand;

            this.LayerGroup = VHR.LayersContexts.World.Group.GetValue();
        }

        protected override void Release()
        {
            base.Release();

            _commands = null;
            _scene = null;
            _synchronizer = null;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.MessageHandlers[MessageType.Setup].Remove(this.ReadSize, this.WriteSize);

            this.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region AetherEntity Implementation
        protected override World BuildMaster(ServiceProvider provider)
            => new World(Vector2.Zero);

        protected override World BuildSlave(ServiceProvider provider)
            => new World(Vector2.Zero);

        protected override void Destroy()
            => this.Do(w => w.Clear());
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
            ObjectDumper.Dump(
                (text, segment) =>
                {
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
                typeof(ShipPartContext),
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
            this.logger.Info($"World Size Changed => {this.Size}");

            // Destroy any pre-existing walls...
            while (_walls.Any())
            {
                var wall = _walls.Dequeue();
                wall.World.Remove(wall);
            }

            // Create brand new walls...
            this.Do(w => _walls.Enqueue(w.CreateRectangle(WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(-WorldEntity.WallWidth / 2, this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(w.CreateRectangle(WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(this.Size.X + (WorldEntity.WallWidth / 2), this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(w.CreateRectangle(this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, -WorldEntity.WallWidth / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(w.CreateRectangle(this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, this.Size.Y + (WorldEntity.WallWidth / 2)), 0, BodyType.Static)));
            
            _walls.ForEach(b =>
            { // Setup wall collisions
                b.SetCollisionCategories(VHR.Categories.BorderCollisionCategories);
                b.SetCollidesWith(VHR.Categories.BorderCollidesWith);

                // b.Restitution = 1f;
                // b.Friction = 0f;
            });
        }
        #endregion
    }
}
