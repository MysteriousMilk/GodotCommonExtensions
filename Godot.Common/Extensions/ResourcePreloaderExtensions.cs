using Godot;

namespace Godot.Common.Extensions;

/// <summary>
/// Extension methods for the Resource Pre-Loader.
/// From Firebelley's GodotUtilities.
/// https://github.com/firebelley/GodotUtilities
/// </summary>
public static class ResourcePreloaderExtension
{
    /// <summary>
    /// Instances a scene with the resource name. Returns null if resource was not found or was not a packed scene.
    /// </summary>
    /// <param name="preloader">The <see cref="ResourcePreloader"/> which contains pre-loaded <see cref="PackedScene"/>s.</param>
    /// <param name="name">The name of the <see cref="Resource"/> within the <see cref="ResourcePreloader"/>.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An instance of the requested <see cref="PackedScene"/> or null if the resource is not found (or invalid).</returns>
    public static T InstantiateSceneOrNull<T>(this ResourcePreloader preloader, string name) where T : Node
    {
        if (!preloader.HasResource(name))
        {
            GD.PrintErr("Preloader did not have a resource with name " + name);
            return null;
        }

        if (!(preloader.GetResource(name) is PackedScene resource))
        {
            GD.PrintErr("Resource with name " + name + " was not a " + nameof(PackedScene));
            return null;
        }

        return resource.InstantiateOrNull<T>();
    }

    /// <summary>
    /// Instances a scene of the given type, using the type name. Returns null if resource was not found or was not a packed scene.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="preloader">The <see cref="ResourcePreloader"/> which contains pre-loaded <see cref="PackedScene"/>s.</param>
    /// <returns>An instance of the requested <see cref="PackedScene"/> or null if the resource is not found (or invalid).</returns>
    public static T InstantiateSceneOrNull<T>(this ResourcePreloader preloader) where T : Node
    {
        return preloader.InstantiateSceneOrNull<T>(typeof(T).Name);
    }
}