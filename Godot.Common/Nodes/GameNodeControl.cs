using Godot.Common.Extensions;

namespace Godot.Common.Nodes
{
    public abstract partial class GameNodeControl<T> : Control where T : Node
    {
        public T Node { get; set; }

        [Export]
        public NodePath NodePath { get; set; }

        public override void _Ready()
        {
            base._Ready();

            if (NodePath != null)
                Node = GetNode<T>(NodePath);
        }
    }
}
