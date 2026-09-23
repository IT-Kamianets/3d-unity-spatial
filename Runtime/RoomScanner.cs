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

        public event Action<SceneData> ScanCompleted;
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
                sceneObject.Volume = new BoundsData
                {
                    Min = CoordinateConversion.ToSchema(bounds.min),
                    Max = CoordinateConversion.ToSchema(bounds.max)
                };
            }

            return sceneObject;
        }
#endif
    }
}
