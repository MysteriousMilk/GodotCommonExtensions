using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godot.Common.Extensions
{
    public static class Node2DExtensions
    {
        /// <summary>
        /// Appends a scene to the <see cref="Node2D"/> as a child.
        /// </summary>
        /// <param name="sceneResource">The <see cref="PackedScene"/> resource to instance.</param>
        /// <param name="globalPos">Position to place the new "sub-scene" at.</param>
        public static void AppendScene(this Node2D parent, PackedScene sceneResource, Vector2 globalPos)
        {
            var node = sceneResource.Instantiate<Node>();

            if (node is Node2D n2d)
                n2d.GlobalPosition = globalPos;

            parent.AddChild(node);
        }

        public static void FlipVertical(this Node2D n2d)
        {
            n2d.Scale = new Vector2(n2d.Scale.x, n2d.Scale.y * -1f);
        }

        public static void FlipHorizontal(this Node2D n2d)
        {
            n2d.Scale = new Vector2(n2d.Scale.x * -1f, n2d.Scale.y);
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
