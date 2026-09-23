# IT-Kamianets 3D Engine: Unity Spatial

`3d-unity-spatial` connects the **IT-Kamianets 3D Engine** with real physical environments.

It provides reusable spatial-computing functionality such as room understanding, physical surfaces, boundaries, anchors, and environment scanning.

### Logic

While `3d-unity-xr` handles interaction with XR hardware, `3d-unity-spatial` handles **understanding the physical space around the user**.

The resulting spatial information should be converted into engine-independent scene data whenever possible.

For VRoom, this package can be used to turn a real room into the initial digital scene.

### Status (version A)

`Runtime/RoomScanner.cs` requests the Quest Scene API permission, loads the current room via MR Utility Kit, and converts it into a `SceneData` (from `3d-unity-scene`) via `MrukLabelMap`. One `MRUKRoom` → one `SceneData`; no multi-room splitting, no rescan/compare, no loading a `SceneData` back into MRUK -- version A is scan-and-upload only (see `vroom-scanner`'s roadmap for what's deferred).

Compiles to a no-op (`ScanFailed` fires with an explanatory message) if the Meta XR MR Utility Kit package (`com.meta.xr.mrutilitykit`) isn't installed in the consuming project -- this package doesn't hard-depend on it, since it isn't reliably resolvable as a plain `package.json` dependency the way `3d-unity-core`/`3d-unity-scene` are.

This was written against the MRUK API as commonly documented, but not verified against a live installed SDK version in the Editor -- expect to adjust member names if the Unity Editor reports compile errors here.

### Roadmap

* Meta Quest Scene API integration
* MRUK integration
* Room discovery
* Floor detection
* Wall detection
* Ceiling detection
* Door detection
* Window detection
* Physical object detection
* Room boundaries
* Spatial anchors
* Room dimensions
* Surface classification
* Spatial mesh support
* Convert scanned rooms into `3d-scene-schema`
* Save/load scanned environments
* Rescan/update environment
* Alignment between virtual and physical scenes
* Stable room-scanning API
