using System;
using System.Linq;

namespace Godot.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if the calling <see cref="Type"/> inherits from the passed <see cref="Type"/>.
        /// </summary>
        /// <param name="child">The calling <see cref="Type"/> (possible child).</param>
        /// <param name="parent">The passed in <see cref="Type"/> (possible parrent of the child).</param>
        /// <returns>True if the child <see cref="Type"/> inherits the parent <see cref="Type"/>.</returns>
        public static bool InheritsFrom(this Type child, Type parent)
        {
            var currentChild = parent.IsGenericTypeDefinition && child.IsGenericType ? child.GetGenericTypeDefinition() : child;

            while (currentChild != typeof(object))
            {
                if (parent == currentChild)
                    return true;

                currentChild = currentChild.BaseType != null && parent.IsGenericTypeDefinition && currentChild.BaseType.IsGenericType
                             ? currentChild.BaseType.GetGenericTypeDefinition()
                             : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Determines if the calling <see cref="Type"/> inherits from or implements the passed <see cref="Type"/>.
        /// </summary>
        /// <param name="child">The calling <see cref="Type"/> (possible child).</param>
        /// <param name="parent">The passed in <see cref="Type"/> (possible parent of the child).</param>
        /// <returns>True if the child <see cref="Type"/> inherits or implements the parent <see cref="Type"/>.</returns>
        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            var currentChild = parent.IsGenericTypeDefinition && child.IsGenericType ? child.GetGenericTypeDefinition() : child;

            while (currentChild != typeof(object))
            {
                if (parent == currentChild || currentChild.HasAnyInterfaces(parent))
                    return true;

                currentChild = currentChild.BaseType != null && parent.IsGenericTypeDefinition && currentChild.BaseType.IsGenericType
                             ? currentChild.BaseType.GetGenericTypeDefinition()
                             : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the calling <see cref="Type"/> has any interfaces of the interface <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The calling <see cref="Type"/>.</param>
        /// <param name="interfaceType">The interface <see cref="Type"/>.</param>
        /// <returns>True if it has the interface, false if not.</returns>
        public static bool HasAnyInterfaces(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(childInterface =>
            {
                var currentInterface = interfaceType.IsGenericTypeDefinition && childInterface.IsGenericType
                                     ? childInterface.GetGenericTypeDefinition()
                                     : childInterface;
                return currentInterface == interfaceType;
            });
        }
    }

}
