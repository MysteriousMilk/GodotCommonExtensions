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
}
