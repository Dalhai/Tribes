using System.Collections.Generic;

using Godot;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public class UnitClassRepository : Repository<string, UnitClass>
{
    /// <summary>
    /// The default resource path used for unit class assets.
    /// </summary>
    private static readonly string DefaultPath = "res://Assets/Units";
    
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
    protected override List<UnitClass> LoadAll() => LoadAll(DefaultPath);

    /// <summary>
    /// Try to load a single asset from the specified resource path.
    /// </summary>
    ///
    /// <param name="resourcePath">The path to the resource.</param>
    /// <param name="asset">The asset to be initialized, if found.</param>
    ///
    /// <returns>True, if the asset could be loaded, false otherwise.</returns>
    protected override bool TryLoad(string resourcePath, out UnitClass? asset)
    {
        GD.Print($"Loading Unit Class: {resourcePath}");
        var resource = GD.Load(resourcePath);
        asset = resource as UnitClass;

        if (asset is { Texture2D: not null })
        {
            return true;
        }

        return false;
    }
}