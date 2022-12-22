using System;
using Godot;

namespace Godot.Extensions
{
    /// <summary>
    /// Provides extensions methods for the <see cref="RandomNumberGenerator"/> class.
    /// </summary>
    public static class RngExtensions
    {
        /// <summary>
        /// Uses the <see cref="RandomNumberGenerator"/> to shuffle the contents of an array.
        /// </summary>
        /// <typeparam name="T">Generic collection type.</typeparam>
        /// <param name="rng">The <see cref="RandomNumberGenerator"/> object.</param>
        /// <param name="array">The collection / array to be shuffled.</param>
        public static void Shuffle<T>(this RandomNumberGenerator rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.RandiRange(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        /// <summary>
        /// Creates a vector with its components randomized between the given min and max values (inclusive).
        /// </summary>
        /// <param name="rng">The <see cref="RandomNumberGenerator"/> object.</param>
        /// <param name="min">The minimum possible value to use.</param>
        /// <param name="max">The maximum possible value to use.</param>
        /// <returns>A vector randomzied between min and max.</returns>
        public static Vector2 RandVec(this RandomNumberGenerator rng, float min, float max)
        {
            return RandVec(rng, min, max, min, max);
        }

        /// <summary>
        /// Creates a vector with it's x-component randomized between xMin and xMax and its y-component randomzied between
        /// yMin and yMax.
        /// </summary>
        /// <param name="rng">The <see cref="RandomNumberGenerator"/> object.</param>
        /// <param name="xMin">The minimum possible value for the x-component.</param>
        /// <param name="xMax">The maximum possible value for the x-component.</param>
        /// <param name="yMin">The minimum possible value for the y-component.</param>
        /// <param name="yMax">The maximum possible value for the y-component.</param>
        /// <returns>A randomized vector.</returns>
        public static Vector2 RandVec(this RandomNumberGenerator rng, float xMin, float xMax, float yMin, float yMax)
        {
            float x = rng.RandfRange(xMin, xMax);
            float y = rng.RandfRange(yMin, yMin);
            return new Vector2(x, y);
        }
    }
}