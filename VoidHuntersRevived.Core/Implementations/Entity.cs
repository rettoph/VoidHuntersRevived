using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class Entity : LayerObject, IEntity
    {
        public EntityInfo Info { get; protected set; }

        public bool IsDeleted { get; private set; }

        public event EventHandler<IEntity> OnDeleted;

        public Entity(EntityInfo info, IGame game) : base(game)
        {
            this.Info = info;

            this.Visible = true;
            this.Enabled = true;

            this.IsDeleted = false;
        }

        

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        public virtual Boolean Delete()
        {
            if(this.IsDeleted)
            {
                this.Game.Logger.LogWarning("Attempting to delete entity already marked for deletion.");

                return false;
            }
            else
            {
                this.IsDeleted = true;

                this.OnDeleted?.Invoke(this, this);

                return true;
            }
        }
    }
}
