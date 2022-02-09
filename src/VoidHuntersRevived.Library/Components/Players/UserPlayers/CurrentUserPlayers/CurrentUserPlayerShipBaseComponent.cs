using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    public abstract class CurrentUserPlayerShipBaseComponent<TShipComponent> : CurrentUserPlayerBaseComponent
        where TShipComponent : Component<Ship>
    {
        public TShipComponent ShipComponent { get; private set; }

        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            this.Entity.OnShipChanged += this.HandleShipChanged;
            this.CleanShip(default, this.Entity.Ship);
        }

        protected override void UninitializeCurrentUser()
        {
            this.Entity.OnShipChanged -= this.HandleShipChanged;
            this.CleanShip(this.Entity.Ship, default);
        }

        protected virtual void AddNewShipComponent(TShipComponent shipComponent)
        {
            //
        }
        protected virtual void RemoveOldShipComponent(TShipComponent shipComponent)
        {
            //
        }

        private void CleanShip(Ship old, Ship value)
        {
            if (this.ShipComponent is not null)
            {
                this.RemoveOldShipComponent(this.ShipComponent);
            }

            if (value is not null)
            {
                this.ShipComponent = value.Components.Get<TShipComponent>();
                this.AddNewShipComponent(this.ShipComponent);
            }
            else
            {
                this.ShipComponent = default;
            }
        }

        private void HandleShipChanged(Player sender, Ship old, Ship value)
        {
            this.CleanShip(old, value);
        }
    }
}
