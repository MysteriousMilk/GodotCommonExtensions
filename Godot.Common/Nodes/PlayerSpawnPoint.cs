namespace Godot.Common.Nodes
{
    /// <summary>
    /// Represents a spawn point for the player.
    /// </summary>
    public partial class PlayerSpawnPoint : Node2D
    {
        /// <summary>
        /// Icon to show in the editor for reference.
        /// </summary>
        [Export]
        public Sprite2D EditorIcon
        {
            get;
            set;
        }

        /// <summary>
        /// Called when the <see cref="Node"/> enters the <see cref="SceneTree"/> for the first and and all of its children are ready.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();

            EditorIcon.Visible = Engine.IsEditorHint();
        }
    }
}
