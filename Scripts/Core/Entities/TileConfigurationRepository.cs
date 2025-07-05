using System.Collections.Generic;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public class TileConfigurationRepository : Repository<TileType, TileConfiguration>
{
    /// <summary>
    /// The default resource path used for tile assets.
    /// </summary>
    private static readonly string DefaultPath = "res://Assets/Tiles";

    /// <summary>
    /// Loads the default assets of the repository.
    /// </summary>
    ///
    /// <remarks>
    /// A repository should provide a default way to initialize.
    /// This will however not be automatically called, but must be executed by users when they
    /// are ready to load - and unload - assets.
    /// </remarks>
    ///
    /// <returns>A list of loaded assets.</returns>
    protected override List<TileConfiguration> LoadAll() => LoadAll(DefaultPath);

    /// <summary>
    /// Try to load a single asset from the specified resource path.
    /// </summary>
    ///
    /// <param name="resourcePath">The path to the resource.</param>
    /// <param name="asset">The asset to be initialized, if found.</param>
    ///
    /// <returns>True, if the asset could be loaded, false otherwise.</returns>
    protected override bool TryLoad(string resourcePath, out TileConfiguration? asset)
    {
        GD.Print($"Loading Terrain: {resourcePath}");
        var resource = GD.Load(resourcePath);
        asset = resource as TileConfiguration;

        return asset is not null;
    }
}