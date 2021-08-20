using Godot;

namespace TribesOfDust.Hex
{

    /// <summary>
    /// Cube coordinates based index.
    /// </summary>
    ///
    /// <remarks>
    /// Cube coordinates operate in three dimensions, with the constraint that the three
    /// dimensions <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> must yield a sum of zero.
    ///
    /// Note that working with a <see cref="CubeCoordinate"/> facilitates some computations,
    /// but the third dimension is ambiguous. For storage, consider using a <see cref="AxialCoordinate"/> instead.
    /// </remarks>
    ///
    /// <param name="X">Left to right</param>
    /// <param name="Y">Bottom right to top left</param>
    /// <param name="Z">Top right to bottom left</param>
    public record CubeCoordinate(int X, int Y, int Z)
    {
        #region Constants

        public static readonly CubeCoordinate Zero = From(0, 0);

        public static readonly CubeCoordinate NW = TileDirectionOffset.GetCubeOffset(TileDirection.NW);
        public static readonly CubeCoordinate N  = TileDirectionOffset.GetCubeOffset(TileDirection.N);
        public static readonly CubeCoordinate NE = TileDirectionOffset.GetCubeOffset(TileDirection.NE);
        public static readonly CubeCoordinate SE = TileDirectionOffset.GetCubeOffset(TileDirection.SE);
        public static readonly CubeCoordinate S  = TileDirectionOffset.GetCubeOffset(TileDirection.S);
        public static readonly CubeCoordinate SW = TileDirectionOffset.GetCubeOffset(TileDirection.SW);

        #endregion
        #region Factory

        public static CubeCoordinate From(int q, int r) => new (q, -(q + r), r);
        public static CubeCoordinate From(AxialCoordinate coordinate) => coordinate.ToCubeCoordinate();
        public static CubeCoordinate From(Vector3 vector)
        {
            var rounded = vector.Round();
            var x = (int) rounded.x;
            var y = (int) rounded.y;
            var z = (int) rounded.z;

            return new (x, y, z);
        }

        #endregion
        #region Conversions

        /// <summary> Converts a <see cref="CubeCoordinate"/> to a <see cref="AxialCoordinate"/>. </summary>
        /// <remarks> Conversion is marked as implicit to facilitate switching between coordinate types. </remarks>
        /// <param name="coordinate">The coordiante to convert.</param>
        public static implicit operator AxialCoordinate(CubeCoordinate coordinate) => coordinate.ToAxialCoordinate();

        /// <summary>
        /// Converts the <see cref="CubeCoordinate"/> to the matching <see cref="AxialCoordinate"/>.
        /// </summary>
        public AxialCoordinate ToAxialCoordinate() => new(X, Z);

        #endregion
        #region Operators

        public static CubeCoordinate operator+(CubeCoordinate lhs, CubeCoordinate rhs) => new (lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        public static CubeCoordinate operator-(CubeCoordinate lhs, CubeCoordinate rhs) => new (lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);

        #endregion

        public override int GetHashCode()
        {
            return (X.GetHashCode(),
                    Y.GetHashCode(),
                    Z.GetHashCode())
                .GetHashCode();
        }

        public override string ToString() => $"({X}, {Y}, {Z})";
    }

    /// <summary>
    /// Axial coordinates based index.
    /// </summary>
    ///
    /// <remarks>
    /// Axial coordiantes operate in two dimensions. The third dimension is implicit.
    /// While the <see cref="Q"/> property describes the offset horizontally, the <see cref="R"/> property
    /// describes the offset along the bottom-left to top-right. This gives you all the axis you need to properly
    /// describe coordinates in a two dimensional system.
    /// </remarks>
    ///
    /// <param name="Q">Left to right</param>
    /// <param name="R">Top right to bottom left</param>
    public record AxialCoordinate(int Q, int R)
    {
        #region Constants

        public static readonly AxialCoordinate Zero = new (0, 0);

        public static readonly AxialCoordinate NW = TileDirectionOffset.GetAxialOffset(TileDirection.NW);
        public static readonly AxialCoordinate N  = TileDirectionOffset.GetAxialOffset(TileDirection.N);
        public static readonly AxialCoordinate NE = TileDirectionOffset.GetAxialOffset(TileDirection.NE);
        public static readonly AxialCoordinate SE = TileDirectionOffset.GetAxialOffset(TileDirection.SE);
        public static readonly AxialCoordinate S  = TileDirectionOffset.GetAxialOffset(TileDirection.S);
        public static readonly AxialCoordinate SW = TileDirectionOffset.GetAxialOffset(TileDirection.SW);

        #endregion
        #region Factory

        public static AxialCoordinate From(CubeCoordinate coordinate) => coordinate.ToAxialCoordinate();
        public static AxialCoordinate From(Vector2 vector)
        {
            var rounded = vector.Round();
            var q = (int) rounded.x;
            var r = (int) rounded.y;

            return new (q, r);
        }

        #endregion
        #region Conversions

        /// <summary> Converts a <see cref="AxialCoordinate"/> to a <see cref="CubeCoordinate"/>. </summary>
        /// <remarks> Conversion is marked as implicit to facilitate switching between coordinate types. </remarks>
        /// <param name="coordinate">The coordiante to convert.</param>
        public static implicit operator CubeCoordinate(AxialCoordinate coordinate) => coordinate.ToCubeCoordinate();

        /// <summary>
        /// Converts the <see cref="AxialCoordinate"/> to the matching <see cref="CubeCoordinate"/>.
        /// </summary>
        public CubeCoordinate ToCubeCoordinate() => CubeCoordinate.From(Q, R);

        #endregion
        #region Operators

        public static AxialCoordinate operator+(AxialCoordinate lhs, AxialCoordinate rhs) => new (lhs.Q + rhs.Q, lhs.R + rhs.R);
        public static AxialCoordinate operator-(AxialCoordinate lhs, AxialCoordinate rhs) => new (lhs.Q - rhs.Q, lhs.R - rhs.R);

        #endregion

        public override int GetHashCode()
        {
            return (Q.GetHashCode(),
                    R.GetHashCode())
                .GetHashCode();
        }

        public override string ToString() => $"({Q}, {R})";
    }
}