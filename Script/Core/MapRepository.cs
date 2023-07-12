using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Core;

public class MapRepository : Repository<string, Map>
{
    #region Constructor
    
    public MapRepository(TerrainRepository terrainRepository)
    {
        _terrainRepository = terrainRepository;
    }
    
    #endregion
    #region Defaults
    
    private static readonly string DefaultPath = "res://Assets/Maps";
    
    #endregion
    #region Loading

    /// <summary>
    /// Loads the default maps of the repository.
    /// </summary>
    ///
    /// <returns>A list of loaded maps.</returns>
    protected override List<Map> LoadAll() => LoadAll(DefaultPath);

    protected override bool TryLoad(string resourcePath, out Map? asset)
    {
        asset = null;
        
        var targetFile = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
        if (targetFile is not null && targetFile.GetError() == Godot.Error.Ok)
        {
            var jsonString = targetFile.GetAsText();
            var jsonObject = JsonSerializer.Deserialize<JsonObject>(jsonString);
            if (jsonObject is null) 
                return false;
            
            // Extract the top level objects
            var mapName = jsonObject["name"]?.GetValue<string>();
            var mapTilesJson = jsonObject["tiles"]?.AsArray();
            
            // Extract all the tiles
            JsonObject? ToObj(JsonNode? node) => node?.AsObject();
            Tile? ToTile(JsonObject? obj)
            {
                var q = obj?["q"]?.GetValue<int>();
                var r = obj?["r"]?.GetValue<int>();
                
                if (q is null || r is null)
                    return null;

                var t = obj?["type"]?.GetValue<int>();
                if (t is null)
                    return null;

                return new Tile(new(q.Value, r.Value), _terrainRepository.GetAsset((TileType)t.Value));
            }

            var mapTiles = mapTilesJson
                ?.Select(ToObj)
                ?.Select(ToTile)
                ?.Cast<Tile>();

            if (mapTiles is not null && mapName is not null)
            {
                asset = new Map(mapName);
                foreach (var tile in mapTiles)
                    asset.Hexes.Add(tile, tile.Coordinates);
            }
        }

        return asset != null;
    }
    
    #endregion
    #region Saving

    public bool TrySave(Map asset)
    {
        var targetPath = DefaultPath + "/" + asset.Name.ToLower().Replace(" ", "_") + ".json";
        var targetFile = FileAccess.Open(targetPath, FileAccess.ModeFlags.Write);

        if (targetFile is not null && targetFile.GetError() == Godot.Error.Ok)
        {
            var jsonObject = new JsonObject();
            var jsonTiles = new JsonArray();

            foreach (var tile in asset.Hexes)
            {
                var jsonTile = new JsonObject
                {
                    { "type", (int)tile.Value.Key },
                    { "q", tile.Key.Q },
                    { "r", tile.Key.R }
                };
                jsonTiles.Add(jsonTile);
            }
            
            jsonObject.Add("name", asset.Name);
            jsonObject.Add("tiles", jsonTiles);

            var jsonString = jsonObject.ToJsonString();
            
            GD.Print(jsonString);
            targetFile.StoreString(jsonString);
            targetFile.Flush();
            targetFile.Close();
            
            if (HasVariations(asset.Key))
                RemoveAll(asset.Key);
            
            AddVariation(asset);
            
            return true;
        }

        return false;
    }
    
    #endregion

    private readonly TerrainRepository _terrainRepository;
}
