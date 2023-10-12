using System;
using Godot;

using TribesOfDust.Core;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Gen;

public class HexMapGenerator : IHexLayerGenerator<Tile>
{
    public HexMapGenerator(AxialCoordinate start, AxialCoordinate end, TileConfigurationRepository repository)
    {
        _start = start;
        _end = end;
        _repository = repository;
    }
    
    /// <summary>
    /// Fills the layer according to the generator specification.
    /// </summary>
    /// 
    /// <param name="layer">The layer to be filled.</param>
    /// 
    /// <returns>True, if the layer was filled.</returns>
    /// <returns>False, if the layer could not be filled.</returns>
    public bool Generate(IHexLayer<Tile> layer)
    {
        AxialCoordinate difference = _end - _start;
        FastNoiseLite noise = new FastNoiseLite();
        
        for (int q = 0; q < difference.Q; ++q)
        for (int r = 0; r < difference.R; ++r)
        {
            var hexLocation = new AxialCoordinate(_start.Q + q, _start.R + r);
            var unitLocation = HexConversions.HexToUnit(hexLocation);
            
            var tileBiome = noise.GetNoise2D(unitLocation.X, unitLocation.Y) + 0.5f;
            var tileType = tileBiome switch
            {
                var f when f >= 0.0f && f <= 0.3f => TileType.Tundra,
                var f when f > 0.3f && f <= 0.6f => TileType.Rocks,
                var f when f > 0.6f && f <= 1.0f => TileType.Dunes,
                _ => TileType.Open
            };
            
            var config = _repository.GetAsset(tileType);
            var tile = new Tile(config, hexLocation);

            layer.TryAdd(hexLocation, tile);
        }

        return true;
    }

    private readonly AxialCoordinate _start;
    private readonly AxialCoordinate _end;
    private readonly TileConfigurationRepository _repository;
}