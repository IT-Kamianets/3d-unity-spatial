using System;
using ITKamianets.Engine.Scene;
using ITKamianets.Engine.Scene.Model;
using UnityEngine;

#if MRUK_PRESENT
using Meta.XR.MRUtilityKit;
#endif

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace ITKamianets.Engine.Spatial
{
    /// <summary>
    /// Requests spatial (Scene API) permission, loads the current Meta Quest room via
    /// MR Utility Kit, and converts it into a SceneData. This is the only conversion
    /// direction version A needs -- physical room -> SceneData -> upload. It does not
    /// load a SceneData back into MRUK/Unity.
    ///
    /// NOTE: written against the MR Utility Kit API as of the Meta XR SDK generation
    /// available when this was authored. Verify member names (MRUK.Instance,
    /// SceneLoadedEvent, MRUKAnchor.Label/PlaneRect/VolumeBounds) against whatever SDK
    /// version is actually installed in the project and adjust if the Editor reports
    /// compile errors here.
    /// </summary>
    public class RoomScanner : MonoBehaviour
    {
        private const string ScenePermission = "com.oculus.permission.USE_SCENE";

#if !MRUK_PRESENT
#pragma warning disable CS0067 // Raised only when the MR Utility Kit is installed.
#endif
        public event Action<SceneData> ScanCompleted;
#if !MRUK_PRESENT
#pragma warning restore CS0067
#endif
        public event Action<string> ScanFailed;

        public void StartScan()
        {
#if !MRUK_PRESENT
            ScanFailed?.Invoke("MR Utility Kit (com.meta.xr.mrutilitykit) is not installed in this project.");
#else
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(ScenePermission))
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += _ => LoadRoom();
                callbacks.PermissionDenied += _ => ScanFailed?.Invoke($"Permission denied: {ScenePermission}");
                Permission.RequestUserPermission(ScenePermission, callbacks);
                return;
            }
#endif
            LoadRoom();
#endif
        }

#if MRUK_PRESENT
        private void LoadRoom()
        {
            MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
            MRUK.Instance.LoadSceneFromDevice();
        }

        private void OnSceneLoaded()
        {
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);

            var room = MRUK.Instance.GetCurrentRoom();
            if (room == null)
            {
                ScanFailed?.Invoke("MRUK reported the scene as loaded, but no room was found.");
                return;
            }

            ScanCompleted?.Invoke(BuildScene(room));
        }

        private SceneData BuildScene(MRUKRoom room)
        {
            var scene = SceneData.CreateEmpty();

            scene.Objects.Add(new SceneObjectData
            {
                Id = room.name,
                Type = SemanticLabel.ROOM,
                Transform = TransformData.Identity
            });

            foreach (var anchor in room.Anchors)
            {
                scene.Objects.Add(BuildSceneObject(anchor, room.name));
            }

            return scene;
        }

        private SceneObjectData BuildSceneObject(MRUKAnchor anchor, string roomId)
        {
            var sceneObject = new SceneObjectData
            {
                Id = anchor.name,
                ParentId = roomId,
                Type = MrukLabelMap.ToSemanticLabel(anchor),
                Transform = new TransformData
                {
                    Position = CoordinateConversion.ToSchema(anchor.transform.position),
                    Rotation = CoordinateConversion.ToSchema(anchor.transform.rotation)
                }
            };

            if (anchor.PlaneRect.HasValue)
            {
                var rect = anchor.PlaneRect.Value;
                sceneObject.Boundary = new System.Collections.Generic.List<Vector2Data>
                {
                    new Vector2Data(rect.xMin, rect.yMin),
                    new Vector2Data(rect.xMax, rect.yMin),
                    new Vector2Data(rect.xMax, rect.yMax),
                    new Vector2Data(rect.xMin, rect.yMax)
                };
            }

            if (anchor.VolumeBounds.HasValue)
            {
                var bounds = anchor.VolumeBounds.Value;

                // Flipping z for the schema swaps which corner is smaller; keep min <= max.
                var a = CoordinateConversion.ToSchema(bounds.min);
                var b = CoordinateConversion.ToSchema(bounds.max);
                sceneObject.Volume = new BoundsData
                {
                    Min = new Vector3Data(System.Math.Min(a.X, b.X), System.Math.Min(a.Y, b.Y), System.Math.Min(a.Z, b.Z)),
                    Max = new Vector3Data(System.Math.Max(a.X, b.X), System.Math.Max(a.Y, b.Y), System.Math.Max(a.Z, b.Z))
                };
            }

            return sceneObject;
        }
#endif
    }
}
