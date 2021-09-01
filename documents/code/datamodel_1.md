# Datamodel 1

Quick brainstorming sprint to figure out how to work with a potential future API.

This approach is based around the idea of not storing everything on the tiles, but instead using the tile coordiantes
as an index into various hashmaps that can be distributed and encapsulated individually. The approach is very much inspired by
battle for wesnoth, although slightly less convoluted to make it easier for ourselves.

```csharp
var assets = Repository<TerrainType, TerrainAsset>.Instance;

// The basic map is just another tile storage, mapping coordinates to terrain.
TileStorage<Terrain> terrain = new();
terrain.Add(0,  0, assets.GetRandomVariation(TerrainType.Tundra);
terrain.Add(0,  1, assets.GetRandomVariation(TerrainType.Tundra);
terrain.Add(1,  1, assets.GetRandomVariation(TerrainType.Tundra);
terrain.Add(0, -1, assets.GetRandomVariation(TerrainType.Tundra);

// The map can be completely disconnected.
terrain.Add(3,  3, assets.GetRandomVariation(TerrainType.Mountain));

// We can assign units to tiles by creating another tile storage, this time storing units.
// To ensure units are only placed on valid tiles, we can constrain a tile storage by another tile storage.
// Constrained tile storages hook into another tile storage's events to keep themselves validated.
TileStorage<Unit> units = TileStorage<Unit>.ConstrainTo(terrain);
units.Add(-1, -1, new Unit(...)); // This will throw an exception.
units.TryAdd(-1, -1, new Unit(...)); // This will just return `false` instead.
```

Speaking about events, yes - these tile storages will support events.

```csharp
terrain.ItemRemoved += tile => Console.WriteLine($"Terrain {tile} has been removed.");
units.ItemRemoved += unit => Console.WriteLine($"Unit {unit} has been removed." );

units.Add(0, 0, new Unit(...));
units.Add(0, 1, new Unit(...));

// Prints 'Unit (0, 1, ...) has been removed.
units.Remove(0, 1);

// Prints 'Unit (0, 0, ...) has been removed.
// Prints 'Terrain (0, 0, ...) has been removed.
terrain.Remove(0, 0);
```

The unit is removed as well because the unit tile storage is constrained by the terrain tile storage. Note that the unit removed
event is fired _before_ the terrain removed event to ensure that whenever a tile storage removes one of its items based on a constraint, the
rest of the tile storages is still in a valid state. Having this generalized tile storage abstraction allows us to keep the coordinate based
access basic and provide enumerators and filters externally.

```csharp
// Creates an enumerable with all items with a 0 as x-coordinate.
// This is an extension method and not defined directly on the tile storage.
// This is the general pattern for accessing additional functionality.
foreach(var (coords, terrain) in terrain.X(0))
{
    Console.WriteLine($"Tile {coords} holds terrain {terrain}.");
}

// Accessing a neighborhood around a tile is just as easy.
// In this case, we request a manhattan neighborhood around the tile (0, 0) with a maximum distance of 2
// and we also include the tile itself in the enumeration.
foreach(var (coords, terrain) in terrain.ManhattanNeighborhood(0, 0, 2, true))
{
    Console.WriteLine($"Neighbor of tile (0, 0) at {coords} is terrain {terrain}.");
}

// Note that all of these implement the ITileStorageView<T> interface and can be reused after instantiation.
var neighborhood = terrain.ManhattanNeighborhood(0, 0, 2, true);

// Parameters can be changed after instantiation.
neighborhood.Distance = 1;
neighborhood.IncludeCenter = false;

// We make the view explicit and drop access to all manhattan neighborhood members.
ITileStorageView<Terrain> view = neighborhood;
view.Get(-1, -1); // Same a for a tile storage, this will throw an exception.
view.Get( 3,  3); // Here we also get an exception. While the tile exists, it is not part of this view.

// All views are enumerable, so we can use the full power of Linq on them.
// Of course, these are not views anymore after that.
var filtered = neighborhood.Where((AxialCoordinate coords) => coords.q > 0);
```

This design suggests a very simple storage scheme using a hashmap for storage. For a first design, this is fine, we don't need to
worry about allocation schemes and memory alignment so early on. I like this. I think it could work. The question about how we distribute this afterwards is an engineering issue and not a design issue anymore. We need to figure out data distribution anyway.