namespace Godot.Common.Extensions;

public static class Vector2Extensions
{
    /// <summary>
    /// Extends or increases the magnitude of a vector through an angle by a fixed scalar amount.
    /// </summary>
    /// <param name="vec">The vector to extend.</param>
    /// <param name="angle">The angle to extend the vector through.</param>
    /// <param name="amount">Scalar amount to extend the vector by.</param>
    /// <returns>The new "extended" vector.</returns>
    public static Vector2 Extend(this Vector2 vec, float angle, float amount)
    {
        return new Vector2(
            vec.X + amount * Mathf.Cos(angle),
            vec.Y + amount * Mathf.Sin(angle));
    }

    /// <summary>
    /// Returns a new <see cref="Vector2"/> that is this vector multiplied by -1.
    /// </summary>
    /// <param name="vec">The vector to negate.</param>
    /// <returns>A new vector that is the current vector, negated.</returns>
    public static Vector2 Negate(this Vector2 vec)
    {
        return vec * -1.0f;
    }

    /// <summary>
    /// Returns a new <see cref="Vector2I"/> that is this vector multiplied by -1.
    /// </summary>
    /// <param name="vec">The vector to negate.</param>
    /// <returns>A new vector that is the current vector, negated.</returns>
    public static Vector2I Negate(this Vector2I vec)
    {
        return vec * -1;
    }

    /// <summary>
    /// Returns a new <see cref="Vector3"/> that is this vector multiplied by -1.
    /// </summary>
    /// <param name="vec">The vector to negate.</param>
    /// <returns>A new vector that is the current vector, negated.</returns>
    public static Vector3 Negate(this Vector3 vec)
    {
        return vec * -1.0f;
    }

    /// <summary>
    /// Returns a new <see cref="Vector3I"/> that is this vector multiplied by -1.
    /// </summary>
    /// <param name="vec">The vector to negate.</param>
    /// <returns>A new vector that is the current vector, negated.</returns>
    public static Vector3I Negate(this Vector3I vec)
    {
        return vec * -1;
    }
}
