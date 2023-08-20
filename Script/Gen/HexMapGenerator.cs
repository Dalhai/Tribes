using System.Collections.Generic;
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
    /// <param name="placements">The newly created placements.</param>
    /// 
    /// <returns>True, if the layer was filled.</returns>
    /// <returns>False, if the layer could not be filled.</returns>
    public bool Generate(IHexLayer<Tile> layer, IDictionary<ulong, Placement> placements)
    {
        AxialCoordinate difference = _end - _start;
        for (int q = 0; q < difference.Q; ++q)
        for (int r = 0; r < difference.R; ++r)
        {
            var config = _repository.GetAsset(TileType.Tundra);
            var placement = new Placement();
            var tile = new Tile(config, placement);

            var location = new AxialCoordinate(_start.Q + q, _start.R + r);
            placement.Location = location;
            
            placements.Add(tile.Identity, placement);
            layer.Add(location, tile);
        }

        return true;
    }

    private readonly AxialCoordinate _start;
    private readonly AxialCoordinate _end;
    private readonly TileConfigurationRepository _repository;
}