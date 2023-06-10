namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public abstract class ShipPartComponent
    {
        public abstract ShipPartComponent Clone();
    }

    public abstract class ShipPartComponent<TComponent> : ShipPartComponent
        where TComponent : ShipPartComponent<TComponent>
    {
    }
}
