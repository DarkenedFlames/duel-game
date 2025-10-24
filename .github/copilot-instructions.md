## Repo overview

- Small turn-based console duel game implemented in C# (top-level statements in `Program.cs`).
- Core pattern: lightweight ECS-like system: `Entity`, `Component`, `World` (singleton), and `TurnManager` drive the game loop.

## Key locations

- `src/core/` — foundational types: `World.cs`, `Entity.cs`, `Component.cs`.
- `src/components/` — concrete components (items, effects, player behaviors).
- `src/systems/` — gameplay systems (e.g. `TurnManager.cs`, `Printer.cs`, `InputHandler.cs`, `ItemFactory.cs`).
- `Program.cs` — simple bootstrap showing how Entities and components are constructed and added to `World`.

## High-level architecture & patterns

- World is a singleton: `World.Instance` is created in `new World()` and enforces a single instance in the constructor. Use `World.Instance` to query or mutate global entity state.
- Entities are lightweight containers for Components. Use `entity.AddComponent` / `AddComponents` and `GetComponent<T>()` to access required behavior/data. `GetComponent<T>()` throws if the component is missing — the codebase often relies on this contract.
- Components manage their subscriptions via `RegisterSubscription` + `Subscribe`/`Unsubscribe`. Prefer adding event handlers inside `RegisterSubscriptions()` so they are automatically wired/unwired when an entity is added or removed from the world.
- Event-driven comms: `World` has events `OnEntityAdded` / `OnEntityRemoved`; `TurnManager` exposes `OnTurnStart` / `OnTurnEnd`. `Printer` is the single place for console output and is often subscribed to world events (see `World()` constructor where `Printer.PrintEntityAdded`/`PrintEntityRemoved` are registered).

## Common developer workflows

- Build: from repository root run `dotnet build MyApp\MyApp.csproj` or `dotnet run --project MyApp` to run the game.
- Run the compiled exe: `MyApp\bin\Debug\net9.0\MyApp.exe` (or the release path). On Windows PowerShell use standard `dotnet` commands.
- Debugging: attach a debugger to the console process or open the `MyApp` project in Visual Studio/VS Code and run with the normal .NET debugger.

## Codebase-specific conventions & examples

- Menu pattern: use `Printer.MultiChoiceList(header, options)` and `InputHandler.GetNumberInput(...)` for consistent console menus (see `TurnManager.HandlePlayerTurn`).
- Inventory menu: `Printer.ShowItemMenu(player, equipment: bool)` returns `(Entity? SelectedItem, string? Action)`; actions are lower-case strings such as `"equip"`, `"unequip"`, `"use"`, `"remove"` — match these exact strings when implementing action handlers.
- Player/items/effects are all `Entity` instances. Use `World.Instance.GetAllForPlayer<T>(player, EntityCategory.Item|Effect, typeId, equipped)` to query items or effects belonging to a player (see `World.GetAllForPlayer`).
- When adding components in code follow the `Program.cs` example: create Entity, then call `AddComponents([...])` with required components (order is not significant, but components expect a valid `Owner` in their constructor).

## Error and defensive patterns

- Many core methods call `GetComponent<T>()` and rely on the presence of required components. If you add features, either ensure components are always present or handle `InvalidOperationException` where appropriate.
- `World.GenerateInstanceId(...)` is used to create stable `EntityId` instance numbers — don't bypass it when constructing Entities.

## Integration points and extension notes

- To add new gameplay features: create a new component under `src/components/` implementing required subscriptions in `RegisterSubscriptions()`. Add that component to entities where needed and raise or listen to the existing events (World/TurnManager/Printer) as appropriate.
- For new UI flow, extend `Printer` with focused helpers and reuse `InputHandler` utilities for input validation.

## Factories

- Item and effect construction is centralized in `src/systems/ItemFactory.cs` and `src/systems/EffectFactory.cs`. Use these factories when you need to spawn items/effects so instance IDs, initial components, and ownership are created consistently. For example, factories will create the `Entity`, add `ItemData`/`EffectData`, and wire initial components (Wearable, Usable, EffectDuration, etc.).
- Avoid manually constructing items/effects in many places; prefer factory methods so the game keeps predictable entity initialization and `World.GenerateInstanceId(...)` is used.

## Component communication and events

- Components communicate using two simple mechanisms used throughout the codebase:
	1. Event subscriptions via `RegisterSubscription` inside a component's `RegisterSubscriptions()` method (then `Subscribe()`/`Unsubscribe()` are called automatically by `Entity.SubscribeAll()`/`UnsubscribeAll()` when entities are added/removed from the `World`). This is the recommended way to listen to global or manager-level events such as `TurnManager.OnTurnStart`/`OnTurnEnd` or `World.OnEntityAdded`/`OnEntityRemoved`.
	2. Direct component access through the owning entity: inside a component, call `Owner.GetComponent<T>()` to access sibling components on the same entity (for example, a `Wearable` may call `Owner.GetComponent<ItemData>()` or a `Usable` may call `Owner.GetComponent<ItemData>().PlayerEntity` to find the user). Note that `GetComponent<T>()` throws when the component is absent — the codebase frequently relies on this contract.

- Practical example patterns found in the repo:
	- A component registers for `TurnManager.OnTurnStart` to decrement `EffectDuration` and removes the entity when duration ends (see `EffectDuration` usage pattern).
	- When an item is used, `Usable.TryUse(target)` can obtain the `ItemData` via `Owner.GetComponent<ItemData>()` to find the source player and stamina cost, then raise world events or call `Printer` helpers to show results.

## Quick checklist for the agent

1. Read `src/core/World.cs`, `src/core/Entity.cs`, `src/core/Component.cs` before modifying game state.
2. Prefer event-based wiring via `RegisterSubscriptions()` inside a component.
3. Use `Printer` and `InputHandler` for any console I/O to maintain UX consistency.
4. When adding or querying entities use `World.Instance` helpers instead of scanning internal lists.
