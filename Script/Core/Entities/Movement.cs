using System.Collections.Generic;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Core.Entities;

public static class Movement
{
    public static IEnumerable<(AxialCoordinate, double)> ComputeReachable(this Unit unit, IHexLayer<Tile> tiles)
    {
        // The current position can always be reached without any movement
        yield return (unit.Coordinates, 0.0);

        Queue<AxialCoordinate> openQueue = new();
        HexLayer<double> costLayer = new();
        
        // Add the initial coordinate
        costLayer.Add(0.0, unit.Coordinates);
        openQueue.Enqueue(unit.Coordinates);

        while (openQueue.TryDequeue(out var coordinates))
        {
            var tile = tiles.Get(coordinates);
            if (tile is null) continue;

            // Compute all candidate tiles the unit could move to
            List<AxialCoordinate> candidates = new();
            if (tile.IsConnected(HexDirection.N) && tiles.Contains(coordinates.N))
                candidates.Add(coordinates.N);
            if (tile.IsConnected(HexDirection.NW) && tiles.Contains(coordinates.NW))
                candidates.Add(coordinates.NW);
            if (tile.IsConnected(HexDirection.NE) && tiles.Contains(coordinates.NE))
                candidates.Add(coordinates.NE);
            if (tile.IsConnected(HexDirection.S) && tiles.Contains(coordinates.S))
                candidates.Add(coordinates.S);
            if (tile.IsConnected(HexDirection.SW) && tiles.Contains(coordinates.SW))
                candidates.Add(coordinates.SW);
            if (tile.IsConnected(HexDirection.SE) && tiles.Contains(coordinates.SE))
                candidates.Add(coordinates.SE);
            
            // Update the costs for all candidates tiles
            foreach (var candidate in candidates)
            {
                var candidateTile = tiles.Get(candidate);
                if (candidateTile is null) continue;

                double movementCosts = candidateTile.Key switch
                {
                    TileType.Tundra => unit.Configuration.MovementCosts?.Tundra ?? 1.0,
                    TileType.Rocks => unit.Configuration.MovementCosts?.Rocks ?? 1.0,
                    TileType.Dunes => unit.Configuration.MovementCosts?.Dunes ?? 1.0,
                    TileType.Canyon => unit.Configuration.MovementCosts?.Canyon ?? 1.0,
                    
                    // TODO (MM): Needs to be handled properly, these are all invalid
                    _ => double.PositiveInfinity
                };

                double currentCosts = costLayer.Get(coordinates);
                double cachedCosts = costLayer.Contains(candidate) ? costLayer.Get(candidate) : double.PositiveInfinity;
                double totalCosts = currentCosts + movementCosts;
                
                if (cachedCosts > totalCosts && totalCosts <= unit.Water)
                {
                    costLayer.Remove(candidate);
                    costLayer.Add(totalCosts, candidate);
                    openQueue.Enqueue(candidate);
                }
            }
        }

        foreach (var entry in costLayer)
            yield return (entry.Key, entry.Value);
    }
}
