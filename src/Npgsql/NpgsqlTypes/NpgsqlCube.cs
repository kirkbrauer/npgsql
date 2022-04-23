using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql.Util;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    public readonly struct NpgsqlCube : IEquatable<NpgsqlCube>
    {
        /// <summary>
        /// The lower-left coordinates of the cube.
        /// </summary>
        public List<double> LowerLeft { get; }
        
        /// <summary>
        /// The upper-right coordinates of the cube.
        /// </summary>
        public List<double> UpperRight { get; }

        /// <summary>
        /// Validates the coordinates in the cube.
        /// </summary>
        /// <param name="lowerLeft">The lower-left coordinate list.</param>
        /// <param name="upperRight">The upper-right coordinate list.</param>
        /// <exception cref="FormatException">
        /// Thrown if the lower-left and upper-right lists have different numbers of coordinates.
        /// </exception>
        static void ValidateCube(List<double> lowerLeft, List<double> upperRight)
        {
            if (lowerLeft.Count != upperRight.Count)
                throw new FormatException($"Not a valid cube: Different point dimensions in {lowerLeft} and {upperRight}.");
        }

        /// <summary>
        /// Creates a new point cube.
        /// </summary>
        /// <param name="coords">The coordinates of the cube.</param>
        public static NpgsqlCube CreatePoint(params double[] coords) => CreatePoint(coords.ToList());
        
        /// <summary>
        /// Creates a new point cube.
        /// </summary>
        /// <param name="coords">The coordinates of the cube.</param>
        public static NpgsqlCube CreatePoint(List<double> coords) => new(coords, coords);

        /// <summary>
        /// Constructs a cube from coordinates.
        /// </summary>
        /// <param name="lowerLeft">The lower-left coordinates.</param>
        /// <param name="upperRight">The upper-right coordinates.</param>
        /// <exception cref="FormatException">
        /// Thrown if the number of dimensions in the upper-left and lower-right points do not match.
        /// </exception>
        public NpgsqlCube(List<double> lowerLeft, List<double> upperRight)
        {
            ValidateCube(lowerLeft, upperRight);
            LowerLeft = lowerLeft;
            UpperRight = upperRight;
        }

        /// <summary>
        /// The number of dimensions of the cube.
        /// </summary>
        public int Dimensions
        {
            get
            {
                ValidateCube(LowerLeft, UpperRight);
                return LowerLeft.Count;
            }
        }

        /// <summary>
        /// True if the cube is a point, that is, the two defining corners are the same.
        /// </summary>
        public bool Point
        {
            get
            {
                ValidateCube(LowerLeft, UpperRight);
                return ReferenceEquals(LowerLeft, UpperRight) || LowerLeft.SequenceEqual(UpperRight);
            }
        }

        /// <inheritdoc />
        public bool Equals(NpgsqlCube other)
        {
            if (ReferenceEquals(LowerLeft, other.LowerLeft) && ReferenceEquals(UpperRight, other.UpperRight))//Short cut for shallow copies.
                return true;
            if (Dimensions != other.Dimensions)
                return false;
            return LowerLeft.SequenceEqual(other.LowerLeft) && UpperRight.SequenceEqual(other.UpperRight);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is NpgsqlCube other && Equals(other);

        /// <inheritdoc cref="IEquatable{T}" />
        public static bool operator ==(NpgsqlCube x, NpgsqlCube y) => x.Equals(y);

        /// <inheritdoc cref="IEquatable{T}" />
        public static bool operator !=(NpgsqlCube x, NpgsqlCube y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            for (var i = 0; i < Dimensions; i++)
            {
                hashCode.Add(LowerLeft[i]);
                hashCode.Add(UpperRight[i]);
            }
            return hashCode.ToHashCode();
        }
    }
}
