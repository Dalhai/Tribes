using Godot;
using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.Hex
{
    public static class Intersections
    {

        public static List<AxialCoordinate> Line(Vector2 from, Vector2 to)
        {
            Vector2 direction = from.DirectionTo(to);
            float distance = from.DistanceTo(to);
            float sample = 0.0f;

            // Sampled output points

            HashSet<AxialCoordinate> coordinates = new();

            do
            {
                var samplePosition = from + direction * sample;
                var sampledPosition = HexConversions.UnitToHex(samplePosition);
                coordinates.Add(sampledPosition);

                sample += HexConversions.SideDistance;
            } while(sample < distance);

            var finalSample = HexConversions.UnitToHex(to);
            coordinates.Add(finalSample);

            return coordinates.ToList();
        }
    }
}