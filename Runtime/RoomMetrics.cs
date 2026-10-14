using System.Collections.Generic;
using ITKamianets.Engine.Scene;
using ITKamianets.Engine.Scene.Model;
using UnityEngine;

namespace ITKamianets.Engine.Spatial
{
    /// <summary>
    /// Measurements of a scanned room, read from its scene document: floor area, ceiling height,
    /// overall size, and what was found in it. Works on any scan in 3d-scene-schema form, with or
    /// without a headset (e.g. a scan stored in Firebase).
    /// </summary>
    public class RoomMetrics
    {
        /// <summary>Floor area in square meters, from the floor's boundary.</summary>
        public float FloorArea;

        /// <summary>Distance from floor to ceiling in meters, or 0 when either is missing.</summary>
        public float CeilingHeight;

        /// <summary>Box around every surface and object, in Unity world space.</summary>
        public Bounds Bounds;

        public int Walls;
        public int Doors;
        public int Windows;

        /// <summary>Detected furniture and objects (tables, couches, beds, ...), with their count per label.</summary>
        public Dictionary<SemanticLabel, int> Objects = new Dictionary<SemanticLabel, int>();

        public static RoomMetrics Measure(SceneData scene)
        {
            var metrics = new RoomMetrics();
            var world = WorldMatrices(scene);
            var hasBounds = false;
            float? floorY = null;
            float? ceilingY = null;

            foreach (var item in scene.Objects)
            {
                var matrix = world[item.Id ?? string.Empty];
                var position = matrix.GetColumn(3);

                switch (item.Type)
                {
                    case SemanticLabel.FLOOR:
                        metrics.FloorArea += PolygonArea(item.Boundary) * Mathf.Abs(matrix.lossyScale.x * matrix.lossyScale.y);
                        floorY = floorY.HasValue ? Mathf.Min(floorY.Value, position.y) : position.y;
                        break;
                    case SemanticLabel.CEILING:
                        ceilingY = ceilingY.HasValue ? Mathf.Max(ceilingY.Value, position.y) : position.y;
                        break;
                    case SemanticLabel.WALL_FACE:
                    case SemanticLabel.INVISIBLE_WALL_FACE:
                        metrics.Walls++;
                        break;
                    case SemanticLabel.DOOR_FRAME:
                        metrics.Doors++;
                        break;
                    case SemanticLabel.WINDOW_FRAME:
                        metrics.Windows++;
                        break;
                    case SemanticLabel.ROOM:
                    case SemanticLabel.GROUP:
                    case SemanticLabel.MARKER:
                        break;
                    default:
                        metrics.Objects.TryGetValue(item.Type, out var count);
                        metrics.Objects[item.Type] = count + 1;
                        break;
                }

                foreach (var corner in Corners(item, matrix))
                {
                    if (!hasBounds)
                    {
                        metrics.Bounds = new Bounds(corner, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        metrics.Bounds.Encapsulate(corner);
                    }
                }
            }

            if (floorY.HasValue && ceilingY.HasValue)
            {
                metrics.CeilingHeight = ceilingY.Value - floorY.Value;
            }

            return metrics;
        }

        // Each object's placement in Unity world space, composing parents and the scene origin.
        private static Dictionary<string, Matrix4x4> WorldMatrices(SceneData scene)
        {
            var byId = new Dictionary<string, SceneObjectData>();
            foreach (var item in scene.Objects)
            {
                byId[item.Id ?? string.Empty] = item;
            }

            var origin = Local(scene.Origin);
            var result = new Dictionary<string, Matrix4x4>();

            Matrix4x4 Resolve(SceneObjectData item, int depth)
            {
                var id = item.Id ?? string.Empty;
                if (result.TryGetValue(id, out var known))
                {
                    return known;
                }

                var parent = depth < 32 && item.ParentId != null && byId.TryGetValue(item.ParentId, out var parentData)
                    ? Resolve(parentData, depth + 1)
                    : origin;
                var matrix = parent * Local(item.Transform);
                result[id] = matrix;
                return matrix;
            }

            foreach (var item in scene.Objects)
            {
                Resolve(item, 0);
            }

            return result;
        }

        private static Matrix4x4 Local(TransformData transform)
        {
            if (transform == null)
            {
                return Matrix4x4.identity;
            }

            var scale = transform.Scale.HasValue ? new Vector3(transform.Scale.Value.X, transform.Scale.Value.Y, transform.Scale.Value.Z) : Vector3.one;
            return Matrix4x4.TRS(CoordinateConversion.FromSchema(transform.Position), CoordinateConversion.FromSchema(transform.Rotation), scale);
        }

        private static IEnumerable<Vector3> Corners(SceneObjectData item, Matrix4x4 matrix)
        {
            if (item.Boundary != null)
            {
                foreach (var point in item.Boundary)
                {
                    yield return matrix.MultiplyPoint3x4(new Vector3(point.X, point.Y, 0f));
                }
            }

            if (item.Volume != null)
            {
                var min = item.Volume.Min;
                var max = item.Volume.Max;
                for (var i = 0; i < 8; i++)
                {
                    var x = (i & 1) == 0 ? min.X : max.X;
                    var y = (i & 2) == 0 ? min.Y : max.Y;
                    var z = (i & 4) == 0 ? -min.Z : -max.Z;
                    yield return matrix.MultiplyPoint3x4(new Vector3(x, y, z));
                }
            }
        }

        private static float PolygonArea(List<Vector2Data> polygon)
        {
            if (polygon == null || polygon.Count < 3)
            {
                return 0f;
            }

            var area = 0f;
            for (var i = 0; i < polygon.Count; i++)
            {
                var a = polygon[i];
                var b = polygon[(i + 1) % polygon.Count];
                area += a.X * b.Y - b.X * a.Y;
            }

            return Mathf.Abs(area) * 0.5f;
        }
    }
}
