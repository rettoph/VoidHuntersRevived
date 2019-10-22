# Galactic Fighters
A game inspired by the Void Hunters from FunOrb

## Custom Events
These are events triggered by Guppy but specific to this game. Events can be bound to via the `Creatable.Events` attribute.

| Project | Class | Event | Arg | Description |
| ------- | ----- | ----- | --- | ----------- |
| `VoidHuntersRevived.Library` | `Chunk` | `cleaned` | `Chunk` | Invoked when the chunk is cleaned. |
| | | | | |
| `VoidHuntersRevived.Library` | `NetworkEntity` | `read` | `NetIncomingMessage` | Invoked when the TryRead method is called. |
| `VoidHuntersRevived.Library` | `NetworkEntity` | `write` | `NetIncomingMessage` | Invoked when the TryWrite method is called. |
| | | | | |
| `VoidHuntersRevived.Library` | `ConnectionNode` | `attached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is created. |
| `VoidHuntersRevived.Library` | `ConnectionNode` | `detached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is destroyed. |
| | | | | |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `body-enabled:changed` | `Boolean` | Invoked when the entities BodyEnabled value changes |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `reserved:changed` | `Boolean` | Invoked when the entities reserved value changes |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `body:created` | `Body` | Invoked when the entities main body is created. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `body:destroyed` | `Body` | Invoked when the entities main body is destroyed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `fixture:created` | `Fixture` | Invoked when a fixture is created on the entity. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `fixture:destroyed` | `Fixture` | Invoked when a fixture on the entity is destroyed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `position:changed` | `Body` | Invoked when the UpdatePosition method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `velocity:changed` | `Body` | Invoked when the UpdateVelocity method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `linear-impulse:applied` | `Vector2` | Invoked when the ApplyLinearVelocity method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `angular-impulse:applied` | `Single` | Invoked when the ApplyAngularVelocity method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `force:applied` | `AppliedForce` | Invoked when the ApplyForce method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `controller:chainged` | `IController` | Invoked when the SetController method is called. |
| | | | | |
| `VoidHuntersRevived.Library` | `ShipPart` | `chain:updated` | `ConnectionNode` | Invoked when any node within the ShipPart's chain attached or detached. |
| | | | | |
| `VoidHuntersRevived.Library` | `Ship` | `bridge:changed` | `ShipPart` | Invoked when the SetBridge method is called. |
| `VoidHuntersRevived.Library` | `Ship` | `bridge:chain:updated` | `ShipPart` | Invoked when the Bridge's chain is updated or when the Bridge is changed. |
| `VoidHuntersRevived.Library` | `Ship` | `direcftion:changed` | `Direction` | Invoked when the Bridge's SetDirection method is called. |
| `VoidHuntersRevived.Library` | `Ship` | `target:offset:changed` | `Vector2` | Invoked when the Ship's target offset is changed. |
| `VoidHuntersRevived.Library` | `Ship` | `firing:changed` | `Boolean` | Invoked when the Ship's SetFiring methid is called. |
| | | | | |
| `VoidHuntersRevived.Library` | `TractorBeam` | `selected` | `ShipPart` | Invoked when the TrySelect method is called. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `released` | `ShipPart` | Invoked when the TryRelease method is called. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `attached` | `FemaleConnectionNode` | Invoked when the TractorBeam's Selection is attached to a FemaleConnectionNode. |
| | | | | |
| `VoidHuntersRevived.Library` | `Gun` | `attached` | `Projectile` | Invoked when the Guns's Fire() method is called. |
| | | | | |
| `VoidHuntersRevived.Library` | `Controller<TControlled>` | `added` | `TControlled` | Invoked when an item is added to the controller. |
| `VoidHuntersRevived.Library` | `Controller<TControlled>` | `removed` | `TControlled` | Invoked when an item is removed from the controller. |
| | | | | |
| `VoidHuntersRevived.Library` | `ShipPartController` | `cleaned` | `IEnumerable<ShipPart>` | Invoked when the ship part controller is cleaned. |

