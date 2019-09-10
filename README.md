# Galactic Fighters
A game inspired by the Void Hunters from FunOrb

## Custom Events
These are events triggered by Guppy but specific to this game. Events can be bound to via the `Creatable.Events` attribute.

| Project | Class | Event | Arg | Description |
| ------- | ----- | ----- | --- | ----------- |
| `GalacicFighters.Library` | `ConnectionNode` | `attached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is created. |
| `GalacicFighters.Library` | `ConnectionNode` | `detached` | `ConnectionNode` | Invoked after an attachment with another ConnectionNode is destroyed. |