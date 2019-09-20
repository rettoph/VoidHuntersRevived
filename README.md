# Galactic Fighters
A game inspired by the Void Hunters from FunOrb

## Custom Events
These are events triggered by Guppy but specific to this game. Events can be bound to via the `Creatable.Events` attribute.

| Project | Class | Event | Arg | Description |
| ------- | ----- | ----- | --- | ----------- |
| `GalacicFighters.Library` | `NetworkEntity` | `read` | `NetIncomingMessage` | Invoked when the TryRead method is called. |
| `GalacicFighters.Library` | `NetworkEntity` | `write` | `NetIncomingMessage` | Invoked when the TryWrite method is called. |
| | | | | |
| `GalacicFighters.Library` | `ConnectionNode` | `attached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is created. |
| `GalacicFighters.Library` | `ConnectionNode` | `detached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is destroyed. |
| | | | | |
| `GalacicFighters.Library` | `FarseerEntity` | `body:created` | `Body` | Invoked when the entities main body is created. |
| `GalacicFighters.Library` | `FarseerEntity` | `body:destroyed` | `Body` | Invoked when the entities main body is destroyed. |
| `GalacicFighters.Library` | `FarseerEntity` | `fixture:created` | `Fixture` | Invoked when a fixture is created on the entity. |
| `GalacicFighters.Library` | `FarseerEntity` | `fixture:destroyed` | `Fixture` | Invoked when a fixture on the entity is destroyed. |
| `GalacicFighters.Library` | `FarseerEntity` | `position:changed` | `Body` | Invoked when the UpdatePosition method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `velocity:changed` | `Body` | Invoked when the UpdateVelocity method is called. |
| | | | | |
| `GalacicFighters.Library` | `Ship` | `bridge:changed` | `ShipPart` | Invoked when the SetBridge method is called. |
| | | | | |
| `GalacicFighters.Library` | `TractorBeam` | `selected` | `ShipPart` | Invoked when the TrySelect method is called. |
| `GalacicFighters.Library` | `TractorBeam` | `released` | `ShipPart` | Invoked when the TryRelease method is called. |
| `GalacicFighters.Library` | `TractorBeam` | `selected:position:changed` | `Vector2` | Invoked when the TractorBeam's Selection's position is updated. |