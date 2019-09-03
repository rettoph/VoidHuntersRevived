# Galactic Fighters
A game inspired by the Void Hunters from FunOrb

## Custom Events
These are events triggered by Guppy but specific to this game. Events can be bound to via the `Creatable.Events` attribute.

| Project | Class | Event | Arg | Description |
| ------- | ----- | ----- | --- | ----------- |
| `GalacicFighters.Library` | `FarseerEntity` | `collides-with:changed` | `Category` | Invoked after the FarseerEntity's CollidesWith attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `collision-categories:changed` | `Category` | Invoked after the FarseerEntity's CollisionCategories attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `collision-group:changed` | `Int16` | Invoked after the FarseerEntity's CollisionGroup attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `is-sensor:changed` | `Boolean` | Invoked after the FarseerEntity's IsSensor attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `sleeping-allowed:changed` | `Boolean` | Invoked after the FarseerEntity's SleepingAllowed attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `physics-enabled:changed` | `Boolean` | Invoked after the FarseerEntity's PhysicsEnabled attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `linear-damping:changed` | `Single` | Invoked after the FarseerEntity's LinearDamping attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `angular-damping` | `Single` | Invoked after the FarseerEntity's AngularDamping attribute is changed. |
| `GalacicFighters.Library` | `FarseerEntity` | `linear-impulse:applied` | `Vector2` | Invoked after the FarseerEntity's ApplyLinearImpulse method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `angular-impulse:applied` | `Single` | Invoked after the FarseerEntity's ApplyAngularImpulse method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `force:applied` | `ForceEventArgs` | Invoked after the FarseerEntity's ApplyForce method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `transform:set` | `Body` | Invoked after the FarseerEntity's SetTransform method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `body:created` | `Body` | Invoked after the FarseerEntity's CreateBody method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `fixture:created` | `Fixture` | Invoked after the FarseerEntity's CreateFixture method is called. |
| `GalacicFighters.Library` | `FarseerEntity` | `fixture:destroyed` | `Fixture` | Invoked after the FarseerEntity's DestroyFixture method is called. |
| | | | |
| `GalacicFighters.Library` | `Ship` | `bridge:changed` | `ShipPart` | Invoked after the Ship's Bridge is changed. |
| `GalacicFighters.Library` | `Ship` | `player:changed` | `Player` | Invoked after the Ship's Player is changed. |
| `GalacicFighters.Library` | `Ship` | `direction:changed` | `Direction` | Invoked after one of the Ship's directions is changed. |
| | | | |
| `GalacicFighters.Library` | `TractorBeam` | `selected` | `ShipPart` | Invoked after the Ship's Bridge is changed. |
| `GalacicFighters.Library` | `TractorBeam` | `released` | `ShipPart` | Invoked after the Ship's Player is changed. |
| `GalacicFighters.Library` | `TractorBeam` | `attached` | `ShipPart` | Invoked after one of the Ship's directions is changed. |
| `GalacicFighters.Library` | `TractorBeam` | `offset:changed` | `Vector2` | Invoked after one of the Ship's directions is changed. |
| `GalacicFighters.Library` | `TractorBeam` | `rotation:changed` | `Single` | Invoked after one of the Ship's directions is changed. |
| | | | |
| `GalacicFighters.Library` | `Player` | `ship:changed` | `Ship` | Invoked after the Player's Ship is changed. |