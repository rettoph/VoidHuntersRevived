using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Scenes
{
    public class NetworkScene : Scene
    {
        protected Dictionary<Int64, INetworkEntity> NetworkEntityTable;

        public NetworkScene(IServiceProvider provider, IGame game) : base(provider, game)
        {
            this.NetworkEntityTable = new Dictionary<Int64, INetworkEntity>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Entities.OnAdd += this.HandleEntityAdd;
            this.Entities.OnRemove += this.HandleEntityRemove;
        }

        private void HandleEntityAdd(object sender, IEntity e)
        {
            if (e is INetworkEntity)
            {
                var ne = e as INetworkEntity;
                this.NetworkEntityTable.Add(ne.Id, ne);
            }
        }

        private void HandleEntityRemove(object sender, IEntity e)
        {
            if (e is INetworkEntity)
            {
                var ne = e as INetworkEntity;
                this.NetworkEntityTable.Remove(ne.Id);
            }
                    
        }
    }
}
