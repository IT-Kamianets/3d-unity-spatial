using ITKamianets.Engine.Scene;
using ITKamianets.Engine.Scene.Model;
using UnityEngine;

namespace ITKamianets.Engine.Spatial
{
    /// <summary>
    /// Builds a scanned room in Unity from its scene document: floor, walls, ceiling and furniture
    /// as solid, visible geometry (invisible walls collide but are not drawn). For showing a stored
    /// scan, planning in a virtual copy of a real room, or testing without a headset.
    /// </summary>
    public static class RoomLoader
    {
        public static LoadedScene Load(SceneData room, Transform parent = null, Material surfaceMaterial = null, ISceneAssetResolver assets = null)
        {
            var loaded = SceneLoader.Load(room, parent, new SceneLoadOptions
            {
                BuildSurfaces = true,
                BuildColliders = true,
                SurfaceMaterial = surfaceMaterial,
                MarkerVolumesAsTriggers = true,
                AssetResolver = assets,
            });

            loaded.name = string.IsNullOrEmpty(room.SceneId) ? "Room" : $"Room {room.SceneId}";
            return loaded;
        }

        /// <summary>Loads and measures a room in one step.</summary>
        public static LoadedScene Load(SceneData room, out RoomMetrics metrics, Transform parent = null, Material surfaceMaterial = null)
        {
            metrics = RoomMetrics.Measure(room);
            return Load(room, parent, surfaceMaterial);
        }

        /// <summary>The floor's world height of a loaded room, e.g. to stand a player on it. 0 when it has no floor.</summary>
        public static float FloorHeight(LoadedScene room)
        {
            var floors = room.FindByType(SemanticLabel.FLOOR);
            return floors.Count > 0 ? floors[0].transform.position.y : 0f;
        }
    }
}
