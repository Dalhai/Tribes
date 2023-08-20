using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Utils;

namespace TribesOfDust.Core;

public class MapRepository(TileConfigurationRepository tileConfigurationRepository) : Repository<string, Map>
{
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

            if (mapName is null || mapTilesJson is null) 
                return false;
            
            asset = new Map(mapName);
            
            // Extract all the tiles
            foreach (var tileJson in mapTilesJson)
            {
                var tileNode = tileJson?.AsObject();
                
                var q = tileNode?["q"]?.GetValue<int>();
                var r = tileNode?["r"]?.GetValue<int>();

                if (q is null || r is null)
                    continue;

                var tileType = tileNode?["type"]?.GetValue<int>();
                if (tileType is null)
                    continue;

                var config = tileConfigurationRepository.GetAsset((TileType)tileType.Value);
                var tile = new Tile(config);

                asset.Tiles.Add(new(q.Value, r.Value), tile);
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

            foreach (var tile in asset.Tiles)
            {
                var jsonTile = new JsonObject
                {
                    { "type", (int)tile.Value.Configuration.Key },
                    { "q", tile.Key.Q },
                    { "r", tile.Key.R }
                };
                jsonTiles.Add(jsonTile);
            }
            
            jsonObject.Add("name", asset.Name);
            jsonObject.Add("tiles", jsonTiles);

            var jsonString = jsonObject.ToJsonString();
            
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
}
