using System;
using System.Collections.Generic;
using System.Linq;

namespace Godot.Extensions
{
    /// <summary>
    /// Extension methods for Godot Node objects.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Clears all children of the given node.
        /// </summary>
        /// <remarks>
        /// Only use this method when there is still a reference to the
        /// child nodes. The removed child nodes will need to be freed at some
        /// point later. Use ClearAndFreeChildren to remove and free all children nodes at once.
        /// </remarks>
        /// <param name="parent">The parent node to clear.</param>
        public static void ClearChildren(this Node parent)
        {
            foreach (Node n in parent.GetChildren())
                parent.RemoveChild(n);
        }

        /// <summary>
        /// Clears all children of the given node and queues them for deletion
        /// at the end of the current frame.
        /// </summary>
        /// <param name="parent">The parent node to clear.</param>
        public static void ClearAndFreeChildren(this Node parent)
        {
            foreach (Node n in parent.GetChildren())
            {
                parent.RemoveChild(n);
                n.QueueFree();
            }
        }

        /// <summary>
        /// Gets the first child in the nodes list of children.
        /// </summary>
        /// <param name="node">The node to get the first child node from.</param>
        /// <returns>The first child of the given node, null if the node has no children.</returns>
        public static Node FirstChild(this Node node)
        {
            Node child = null;
            if (node.GetChildCount() > 0)
                child = node.GetChild(0);
            return child;
        }

        /// <summary>
        /// Gets the first child in the nodes list of children.
        /// </summary>
        /// <param name="node">The node to get the first child node from.</param>
        /// <param name="predicate">The first node to be returned must meet this condition.</param>
        /// <returns>The first child of the given node, null if the node has no children.</returns>
        public static Node FirstChild(this Node node, Func<Node, bool> predicate)
        {
            Node child = null;

            if (node.GetChildCount() == 0)
                return null;

            foreach (Node c in node.GetChildren())
            {
                if (predicate.Invoke(c))
                {
                    child = c;
                    break;
                }
            }

            return child;
        }

        /// <summary>
        /// Looks up the node hierarchy trying to find a parent of the given type.
        /// </summary>
        /// <typeparam name="T">The search type.</typeparam>
        /// <param name="node">The node to start the search on.</param>
        /// <returns>Null if parent is not found, otherwise, the first parent that matches the given type.</returns>
        public static T FindParentOfType<T>(this Node node) where T : Node
        {
            if (node is T)
                return (T)node;
            if (node is null)
                return default(T);
            return FindParentOfType<T>(node.GetParent());
        }

        /// <summary>
        /// Gets the first child node of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of node to get.</typeparam>
        /// <param name="node">The node for which to search its children.</param>
        /// <returns>A node of a specified type, or null if not found.</returns>
        public static T GetFirstNode<T>(this Node node) where T : Node
        {
            T firstNode = default;
            var type = typeof(T);

            foreach (var childNode in node.GetChildren())
            {
                if (childNode.GetType().Equals(type) ||
                    type.IsInstanceOfType(childNode))
                {
                    firstNode = (T)childNode;
                    break;
                }
            }

            return firstNode;
        }

        /// <summary>
        /// Finds the first node in the node hierarchy of the given type 
        /// with the given name.
        /// </summary>
        /// <typeparam name="T">Type of node to find.</typeparam>
        /// <param name="node">Node to search from.</param>
        /// <param name="nodeName">Name of the node to find.</param>
        /// <returns></returns>
        public static T FindNode<T>(this Node node, string nodeName) where T : Node
        {
            T foundNode = null;
            var type = typeof(T);

            foreach (var childNode in node.GetChildren().Cast<Node>())
            {
                if (childNode.GetType().Equals(type) &&
                    childNode.Name.Equals(nodeName))
                {
                    foundNode = (T)childNode;
                }
                else
                {
                    foundNode = childNode.FindNode<T>(nodeName);
                }

                if (foundNode != null)
                    break;
            }

            return foundNode;
        }

        /// <summary>
        /// Finds the first node in the hierarchy of the given type.
        /// </summary>
        /// <param name="node">Node to search from.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>The first node found in the hierarchy of the given type, or null if not found.</returns>
        public static T FindNode<T>(this Node node) where T : Node
        {
            T foundNode = node.GetFirstNode<T>();

            if (foundNode == null)
            {
                foreach (var childNode in node.GetChildren().Cast<Node>())
                {
                    foundNode = childNode.FindNode<T>();
                    if (foundNode != null)
                        break;
                }
            }

            return foundNode;
        }

        /// <summary>
        /// Finds all children nodes (direct descendants) that are of the given type.
        /// </summary>
        /// <typeparam name="T">The search type.</typeparam>
        /// <param name="node">The node whose children should be searched.</param>
        /// <returns>The sequence of nodes that match the given criteria.</returns>
        public static IEnumerable<T> FindNodesByType<T>(this Node node) where T : Node
        {
            List<T> nodesFound = new List<T>();
            var nodeType = typeof(T);

            foreach (var childNode in node.GetChildren())
            {
                if (childNode.GetType().Equals(nodeType))
                    nodesFound.Add((T)childNode);
            }

            return nodesFound;
        }

        /// <summary>
        /// Finds the first instance of a specific node, starting with the children of the given node.
        /// Finds the first descendant of the given type.
        /// </summary>
        /// <param name="node">The node to start the search on.</param>
        /// <typeparam name="T">The node type to search for.</typeparam>
        /// <returns>A node whose type matches the generic type parameter and is the first found descendent of the search node.</returns>
        public static T FindFirstDescendantByType<T>(this Node node) where T : Node
        {
            var nodeType = typeof(T);
            T descendant = default(T);

            foreach (var child in node.GetChildren())
            {
                var childNode = child as Node;

                if (childNode != null)
                {
                    if (childNode.GetType().InheritsFrom(nodeType))
                    {
                        descendant = (T)childNode;
                        break;
                    }
                }

                descendant = childNode.FindFirstDescendantByType<T>();

                if (descendant != null)
                    break;
            }

            return descendant;
        }

        /// <summary>
        /// Finds all descendant <see cref="Node"/>s of a specific type.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to begin the search on.</param>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>An enumerable of <see cref="Node"/>s whose type matches the generic type parameter and is a descendent of the search <see cref="Node"/>.</returns>
        public static IEnumerable<T> FindDescendantNodesByType<T>(this Node node) where T : Node
        {
            List<T> nodesFound = new List<T>();
            FindDescendantNodesByType<T>(node, ref nodesFound);
            return nodesFound;
        }

        private static void FindDescendantNodesByType<T>(Node node, ref List<T> nodesFound) where T : Node
        {
            var nodeType = typeof(T);
            foreach (var child in node.GetChildren())
            {
                var childNode = child as Node;

                if (childNode != null)
                {
                    //if (childNode.GetType().Equals(nodeType))
                    if (childNode.GetType().InheritsFrom(nodeType))
                        nodesFound.Add((T)childNode);

                    FindDescendantNodesByType<T>(childNode, ref nodesFound);
                }
            }
        }

        /// <summary>
        /// Finds all descendant <see cref="Node"/>s (of the given <see cref="Node"/>s) that meet a specific condition.
        /// </summary>
        /// <param name="node">The parent <see cref="Node"/>s to conduct the search from.</param>
        /// <param name="predicate">Delegate that evaluates each <see cref="Node"/>s to see if it belongs in the return list.</param>
        /// <returns>List of all <see cref="Node"/>s that meet the given condition.</returns>
        public static IEnumerable<Node> FindDescendantNodesIf(this Node node, Predicate<Node> predicate)
        {
            List<Node> nodesFound = new List<Node>();
            FindDescendantNodesIf(node, predicate, ref nodesFound);
            return nodesFound;
        }

        private static void FindDescendantNodesIf(Node node, Predicate<Node> predicate, ref List<Node> nodesFound)
        {
            foreach (var child in node.GetChildren())
            {
                var childNode = child as Node;

                if (childNode != null)
                {
                    if (predicate.Invoke(childNode))
                        nodesFound.Add(childNode);

                    FindDescendantNodesIf(childNode, predicate, ref nodesFound);
                }
            }
        }

        /// <summary>
        /// Draws a circular arc of a specific radius and width.
        /// </summary>
        /// <param name="node">The node/canvas to draw the arc on.</param>
        /// <param name="center">The center of the circle/arc.</param>
        /// <param name="radius">The radius of the circle/arc.</param>
        /// <param name="angleFrom">Angle at which to begin drawing the arc.</param>
        /// <param name="angleTo">Angle at which to end drawing the arc.</param>
        /// <param name="color">Color of the arc line.</param>
        public static void DrawCircleArc(this Node2D node, Vector2 center, float radius, float angleFrom, float angleTo, Color color, float width = 1f)
        {
            int nbPoints = 32;
            var pointsArc = new Vector2[nbPoints];

            for (int i = 0; i < nbPoints; ++i)
            {
                float anglePoint = Mathf.DegToRad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90f);
                pointsArc[i] = center + new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
            }

            for (int i = 0; i < nbPoints - 1; ++i)
                node.DrawLine(pointsArc[i], pointsArc[i + 1], color, width);
        }

        /// <summary>
        /// Draws a dashed line on the canvas.
        /// </summary>
        /// <param name="node">Canvas Item to draw the dashed line on.</param>
        /// <param name="from">Start point of the line.</param>
        /// <param name="to">End point of the line</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line.</param>
        /// <param name="dashLength">Length of the dashes.</param>
        /// <param name="capEnd">Flag to indicate whether or not to cap the end of the line.</param>
        /// <param name="antialiased">Indicates if the line should be antialiased or not.</param>
        public static void DrawDashedLine(this CanvasItem node, Vector2 from, Vector2 to, Color color, float width = 1f, float dashLength = 5f, bool capEnd = false, bool antialiased = false)
        {
            float length = (to - from).Length();
            Vector2 normal = (to - from).Normalized();
            Vector2 dashStep = normal * dashLength;

            if (length < dashLength) // not long enough to dash
            {
                node.DrawLine(from, to, color, width, antialiased);
                return;
            }
            else
            {
                bool drawFlag = true;
                Vector2 segmentStart = from;
                int steps = (int)(length / dashLength);

                for (int i = 0; i < steps + 1; i++)
                {
                    Vector2 segmentEnd = segmentStart + dashStep;

                    if (drawFlag)
                        node.DrawLine(segmentStart, segmentEnd, color, width, antialiased);

                    segmentStart = segmentEnd;
                    drawFlag = !drawFlag;
                }

                if (capEnd)
                    node.DrawLine(segmentStart, to, color, width, antialiased);
            }
        }
    }
}