# Void Hunters Revived
A game inspired by the Void Hunters from FunOrb

## Custom Events
These are events triggered by Guppy but specific to this game. Events can be bound to via the `Driven.Events` attribute.

| Project | Class | Event | Arg | Description |
| ------- | ----- | ----- | --- | ----------- |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:collides-with` | `Category` | Invoked after the FarseerEntity's CollidesWith attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:collision-categories` | `Category` | Invoked after the FarseerEntity's CollisionCategories attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:collision-group` | `Int16` | Invoked after the FarseerEntity's CollisionGroup attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:is-sensor` | `Boolean` | Invoked after the FarseerEntity's IsSensor attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:sleeping-allowed` | `Boolean` | Invoked after the FarseerEntity's SleepingAllowed attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:physics-enabled` | `Boolean` | Invoked after the FarseerEntity's PhysicsEnabled attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:linear-damping` | `Single` | Invoked after the FarseerEntity's LinearDamping attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `changed:angular-damping` | `Single` | Invoked after the FarseerEntity's AngularDamping attribute is changed. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `applied:linear-impulse` | `Vector2` | Invoked after the FarseerEntity's ApplyLinearImpulse method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `applied:angular-impulse` | `Single` | Invoked after the FarseerEntity's ApplyAngularImpulse method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `applied:force` | `ForceEventArgs` | Invoked after the FarseerEntity's ApplyForce method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `set:transform` | `Body` | Invoked after the FarseerEntity's SetTransform method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `created:body` | `Body` | Invoked after the FarseerEntity's CreateBody method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `created:fixture` | `Fixture` | Invoked after the FarseerEntity's CreateFixture method is called. |
| `VoidHuntersRevived.Library` | `FarseerEntity` | `destroyed:fixture` | `Fixture` | Invoked after the FarseerEntity's DestroyFixture method is called. |
| | | | |
| `VoidHuntersRevived.Library` | `Ship` | `changed:bridge` | `ShipPart` | Invoked after the Ship's Bridge is changed. |
| `VoidHuntersRevived.Library` | `Ship` | `changed:player` | `Player` | Invoked after the Ship's Player is changed. |
| `VoidHuntersRevived.Library` | `Ship` | `changed:direction` | `Direction` | Invoked after one of the Ship's directions is changed. |
| | | | |
| `VoidHuntersRevived.Library` | `TractorBeam` | `selected` | `ShipPart` | Invoked after the Ship's Bridge is changed. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `released` | `ShipPart` | Invoked after the Ship's Player is changed. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `attached` | `ShipPart` | Invoked after one of the Ship's directions is changed. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `changed:offset` | `Vector2` | Invoked after one of the Ship's directions is changed. |
| `VoidHuntersRevived.Library` | `TractorBeam` | `changed:rotation` | `Single` | Invoked after one of the Ship's directions is changed. |
| | | | |
| `VoidHuntersRevived.Library` | `Player` | `changed:ship` | `Ship` | Invoked after the Player's Ship is changed. |