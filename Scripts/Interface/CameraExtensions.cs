using Godot;
using TribesOfDust.Core;
using TribesOfDust.Hex;

namespace TribesOfDust.Interface;

public static class CameraExtensions
{
    /// <summary>
    /// Move the camera to an AxialCoordinate position using the HexMap coordinate system.
    /// </summary>
    /// <param name="camera">The camera to move</param>
    /// <param name="coordinate">The hex coordinate to move to</param>
    /// <param name="hexMap">The HexMap to use for coordinate conversion</param>
    public static void MoveTo(this Camera camera, AxialCoordinate coordinate, HexMap hexMap)
    {
        var worldPosition = hexMap.HexToWorldPosition(coordinate);
        camera.MoveTo(worldPosition);
    }

    /// <summary>
    /// Move the camera to a CubeCoordinate position using the HexMap coordinate system.
    /// </summary>
    /// <param name="camera">The camera to move</param>
    /// <param name="coordinate">The cube coordinate to move to</param>
    /// <param name="hexMap">The HexMap to use for coordinate conversion</param>
    public static void MoveTo(this Camera camera, CubeCoordinate coordinate, HexMap hexMap)
    {
        var axialCoordinate = coordinate.ToAxialCoordinate();
        camera.MoveTo(axialCoordinate, hexMap);
    }

    /// <summary>
    /// Move the camera to an OffsetCoordinate position using the HexMap coordinate system.
    /// </summary>
    /// <param name="camera">The camera to move</param>
    /// <param name="coordinate">The offset coordinate to move to</param>
    /// <param name="hexMap">The HexMap to use for coordinate conversion</param>
    public static void MoveTo(this Camera camera, OffsetCoordinate coordinate, HexMap hexMap)
    {
        var axialCoordinate = coordinate.ToAxialCoordinate();
        camera.MoveTo(axialCoordinate, hexMap);
    }

    /// <summary>
    /// Move and zoom the camera to fully contain a Map using the HexMap coordinate system.
    /// </summary>
    /// <param name="camera">The camera to position</param>
    /// <param name="map">The map to fit in view</param>
    /// <param name="hexMap">The HexMap to use for coordinate conversion and size calculations</param>
    public static void FitToMap(this Camera camera, Map map, HexMap hexMap)
    {
        if (map.Tiles.Count == 0)
            return;

        var extents = hexMap.GetMapExtents(map);
        
        var paddedExtents = new Rect2(
            extents.Position - extents.Size * 0.1f,
            extents.Size * 1.2f
        );
        
        camera.ZoomToFitExtents(paddedExtents);
    }
}