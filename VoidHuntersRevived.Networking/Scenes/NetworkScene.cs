using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Scenes
{
    public class NetworkScene : Scene
    {
        public NetworkEntityCollection NetworkEntities;

        public NetworkScene(IServiceProvider provider, IGame game) : base(provider, game)
        {
            this.NetworkEntities = new NetworkEntityCollection();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Entities.OnAdded += this.HandleEntityAdd;
            this.Entities.OnRemove += this.HandleEntityRemove;
        }

        private void HandleEntityAdd(object sender, IEntity e)
        {
            if (e is INetworkEntity)
            {
                var ne = e as INetworkEntity;
                this.NetworkEntities.Add(ne);
            }
        }

        private void HandleEntityRemove(object sender, IEntity e)
        {
            if (e is INetworkEntity)
            {
                var ne = e as INetworkEntity;
                this.NetworkEntities.Remove(ne);
            }     
        }
    }
}
