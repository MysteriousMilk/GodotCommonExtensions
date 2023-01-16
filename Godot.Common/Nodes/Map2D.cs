using Godot.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Godot.Common.Nodes
{
    public partial class Map2D : Node2D
    {
        /// <summary>
        /// The <see cref="Godot.TileMap"/> for this <see cref="Map2D"/>.
        /// </summary>
        [Export]
        public TileMap TileMap { get; set; }

        /// <summary>
        /// Layer that entities belong to.
        /// </summary>
        [Export]
        public Node2D EntityLayer { get; set; }
        
        /// <summary>
        /// Layer that decal effects belong to.
        /// </summary>
        [Export]
        public Node2D DecalLayer { get; set; }

        /// <summary>
        /// Signal emitted when the map is loaded.
        /// </summary>
        /// <param name="map">Provides this <see cref="Map2D"/> as a reference.</param>
        [Signal]
        public delegate void MapLoadedEventHandler(Map2D map);

        /// <summary>
        /// Signal emitted when the map is modified (like changes to the <see cref="Godot.TileMap"/>.)
        /// </summary>
        /// <param name="map">Provides this <see cref="Map2D"/> as a reference.</param>
        /// <param name="cellPos"><see cref="Godot.TileMap"/> position of the modified cell.</param>
        [Signal]
        public delegate void MapModifiedEventHandler(Map2D map, Vector2i cellPos);

        /// <summary>
        /// Called every time the <see cref="Map2D"/> is loaded. Used to spawn the player and do
        /// other startup tasks.
        /// </summary>
        public virtual void Startup()
        {
            EmitSignal(nameof(MapLoaded), this);
        }

        /// <summary>
        /// Adds an entity to the <see cref="Map2D"/> at the given position.
        /// </summary>
        /// <param name="ent">The entity to add.</param>
        /// <param name="globalPos">Global position to add the entity at.</param>
        public void SpawnEntity(Node2D ent, Vector2 globalPos)
        {
            if (ent != null)
            {
                // if the entity already has a parent, remove it (since a new parent is assigned here)
                var oldParent = ent.GetParent();
                if (oldParent != null)
                    oldParent.RemoveChild(ent);

                ent.GlobalPosition = globalPos;
                EntityLayer.AddChild(ent);
            }
        }

        public Node2D GetEntityByName(string entityName)
        {
            return EntityLayer.FindChildByName<Node2D>(entityName);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : Node
        {
            foreach (var node in EntityLayer.GetChildren())
            {
                if (node.GetType().InheritsOrImplements(typeof(T)))
                    yield return (T)node;
            }
        }

        public void DespawnEntity(Node2D entity)
        {
            entity.QueueFree();
        }

        /// <summary>
        /// Adds a decal to the decal layer. The decal is provided as a <see cref="PackedScene"/>.
        /// </summary>
        /// <typeparam name="T">Node type for the instantiated decal.</typeparam>
        /// <param name="sceneToInstance">Decal scene to instantiate.</param>
        /// <param name="globalPos">Global position to draw the decal at.</param>
        /// <param name="scale">The scale to apply to the decal.</param>
        /// <returns>A reference to the instantiated decal node.</returns>
        public Node2D AddDecal<T>(PackedScene sceneToInstance, Vector2 globalPos, Vector2 scale) where T : Node2D
        {
            T node = sceneToInstance.Instantiate<T>();
            node.GlobalPosition = globalPos;
            node.Scale = scale;
            DecalLayer.AddChild(node);
            return node;
        }

        public Vector2 GetRandomPlayerSpawnPoint(RandomNumberGenerator rng)
        {
            Vector2 spawnPoint = Vector2.Zero;

            if (Multiplayer.IsServer())
            {
                // find all player spawn points and pick a random one to spawn the player at
                var spawnPoints = this.FindDescendantNodesByType<PlayerSpawnPoint>().ToArray();
                if (spawnPoints.Length > 0)
                {
                    int randomIndex = rng.RandiRange(0, spawnPoints.Length - 1);
                    spawnPoint = spawnPoints[randomIndex].GlobalPosition;
                }
            }

            return spawnPoint;
        }

        /// <summary>
        /// Gets the nearest entity to the given position. Optional list of entities to exclude.
        /// </summary>
        /// <param name="globalPos">The position to look for a near by entity.</param>
        /// <param name="excludeList">List of entity objects to skip.</param>
        /// <returns>The entity nearest to the given position.</returns>
        public Node2D GetNearestEntity(Vector2 globalPos, IEnumerable<Node2D> excludeList = null)
        {
            Node2D nearestEnt = null;
            float minDist = float.MaxValue;

            List<Node2D> entList;
            if (excludeList == null)
                entList = EntityLayer.FindNodesByType<Node2D>().ToList();
            else
                entList = EntityLayer.FindNodesByType<Node2D>().Where(e => !excludeList.Any(e2 => e2 == e)).ToList();

            foreach (var ent in entList)
            {
                float dist = ent.GlobalPosition.DistanceTo(globalPos);
                if (dist < minDist)
                {
                    nearestEnt = ent;
                    minDist = dist;
                }
            }

            return nearestEnt;
        }

        /// <summary>
        /// Returns the distance to the nearest entity. Optional list of entities to exclude.
        /// </summary>
        /// <param name="globalPos">The position to look for a near by entity.</param>
        /// <param name="excludeList">List of entity objects to skip.</param>
        /// <returns>Distance to the nearest entity.</returns>
        public float DistanceToNearestEntity(Vector2 globalPos, IEnumerable<Node2D> excludeList = null)
        {
            float minDist = float.MaxValue;

            List<Node2D> entList;
            if (excludeList == null)
                entList = EntityLayer.FindNodesByType<Node2D>().ToList();
            else
                entList = EntityLayer.FindNodesByType<Node2D>().Where(e => !excludeList.Any(e2 => e2 == e)).ToList();

            foreach (var ent in entList)
            {
                float dist = ent.GlobalPosition.DistanceTo(globalPos);
                if (dist < minDist)
                    minDist = dist;
            }

            return minDist;
        }

        /// <summary>
        /// Converts global position to <see cref="Godot.TileMap"/> coordinates.
        /// </summary>
        /// <param name="globalPos">The global position.</param>
        /// <returns>The "tile map" coordinate.</returns>
        public Vector2i LocalToMap(Vector2 globalPos)
        {
            if (TileMap == null)
                return Vector2i.Zero;

            return TileMap.LocalToMap(TileMap.ToLocal(globalPos));
        }

        /// <summary>
        /// Gets the tile size of the <see cref="Godot.TileMap"/>.
        /// </summary>
        /// <returns>The <see cref="Godot.TileMap"/>'s tile size.</returns>
        public Vector2 GetTileSize()
        {
            return TileMap != null ? TileMap.TileSet.TileSize : Vector2.Zero;
        }
    }
}
