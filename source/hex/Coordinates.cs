using System.Runtime.Serialization;
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
    /// <param name="X">Top Left to Bottom Right</param>
    /// <param name="Y">Top Right to Bottom Left</param>
    /// <param name="Z">Top to Bottom</param>
    [DataContract]
    public record CubeCoordinate(
        [property: DataMember] int X,
        [property: DataMember] int Y,
        [property: DataMember] int Z)
    {
        #region Constants

        public static readonly CubeCoordinate Zero = From(0, 0);

        public static readonly CubeCoordinate DirNW = TileDirectionOffset.GetCubeOffset(TileDirection.NW);
        public static readonly CubeCoordinate DirN = TileDirectionOffset.GetCubeOffset(TileDirection.N);
        public static readonly CubeCoordinate DirNE = TileDirectionOffset.GetCubeOffset(TileDirection.NE);
        public static readonly CubeCoordinate DirSE = TileDirectionOffset.GetCubeOffset(TileDirection.SE);
        public static readonly CubeCoordinate DirS  = TileDirectionOffset.GetCubeOffset(TileDirection.S);
        public static readonly CubeCoordinate DirSW = TileDirectionOffset.GetCubeOffset(TileDirection.SW);

        public CubeCoordinate NW => this + DirNW;
        public CubeCoordinate N  => this + DirN;
        public CubeCoordinate NE => this + DirNE;
        public CubeCoordinate SE => this + DirSE;
        public CubeCoordinate S  => this + DirS;
        public CubeCoordinate SW => this + DirSW;

        #endregion
        #region Factory

        public static CubeCoordinate From(int q, int r) => new (q, -(q + r), r);
        public static CubeCoordinate From(AxialCoordinate coordinate) => coordinate.ToCubeCoordinate();
        public static CubeCoordinate From(Vector3 vector)
        {
            var rounded = vector.Round();
            var x = (int) rounded.X;
            var y = (int) rounded.Y;
            var z = (int) rounded.Z;

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
    /// <param name="Q">Top Left to Bottom Right</param>
    /// <param name="R">Top to Bottom</param>
    [DataContract]
    public record AxialCoordinate(
        [property: DataMember] int Q,
        [property: DataMember] int R)
    {
        #region Constants

        public static readonly AxialCoordinate Zero = new (0, 0);

        public static readonly AxialCoordinate DirNW = TileDirectionOffset.GetAxialOffset(TileDirection.NW);
        public static readonly AxialCoordinate DirN = TileDirectionOffset.GetAxialOffset(TileDirection.N);
        public static readonly AxialCoordinate DirNE = TileDirectionOffset.GetAxialOffset(TileDirection.NE);
        public static readonly AxialCoordinate DirSE = TileDirectionOffset.GetAxialOffset(TileDirection.SE);
        public static readonly AxialCoordinate DirS  = TileDirectionOffset.GetAxialOffset(TileDirection.S);
        public static readonly AxialCoordinate DirSW = TileDirectionOffset.GetAxialOffset(TileDirection.SW);

        public AxialCoordinate NW => this + DirNW;
        public AxialCoordinate N  => this + DirN;
        public AxialCoordinate NE => this + DirNE;
        public AxialCoordinate SE => this + DirSE;
        public AxialCoordinate S  => this + DirS;
        public AxialCoordinate SW => this + DirSW;

        #endregion
        #region Factory

        public static AxialCoordinate From(CubeCoordinate coordinate) => coordinate.ToAxialCoordinate();
        public static AxialCoordinate From(Vector2 vector)
        {
            var rounded = vector.Round();
            var q = (int) rounded.X;
            var r = (int) rounded.Y;

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