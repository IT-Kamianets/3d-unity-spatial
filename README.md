# IT-Kamianets 3D Engine: Unity Spatial

`3d-unity-spatial` connects the **IT-Kamianets 3D Engine** with real physical environments.

It provides reusable spatial-computing functionality such as room understanding, physical surfaces, boundaries, anchors, and environment scanning.

### Logic

While `3d-unity-xr` handles interaction with XR hardware, `3d-unity-spatial` handles **understanding the physical space around the user**.

The resulting spatial information should be converted into engine-independent scene data whenever possible.

For VRoom, this package can be used to turn a real room into the initial digital scene.

### Status

Version 0.1.

* `RoomScanner` (Meta Quest, needs the MR Utility Kit package `com.meta.xr.mrutilitykit`): requests the Scene API permission, loads the current room through MRUK, and converts it into a `SceneData` (from `3d-unity-scene`) via `MrukLabelMap`. One `MRUKRoom` → one `SceneData`. Without MRUK it compiles to a no-op (`ScanFailed` explains why). Written against the MRUK API as documented and compiled without the SDK: on-device checks are still to do.
* `RoomMetrics.Measure(scene)`: floor area, ceiling height, overall bounds, and counts of walls, doors, windows and detected objects, from any scan in 3d-scene-schema form (no headset needed, e.g. a scan stored in Firebase).
* `RoomLoader.Load(scene)`: builds a scanned room as solid, visible geometry through `SceneLoader` (invisible walls collide but are not drawn), and `FloorHeight` to stand a player on it.

Checked in Play mode with `3d-scene-schema`'s room-scan example (no MRUK): 30 m² floor, 2.7 m ceiling, 1 wall, 1 door, 1 window, built with 5 colliders. Compiles without warnings when MRUK is not installed.

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
* Surface classification
* Spatial mesh support
* Save/load scanned environments
* Rescan/update environment
* Alignment between virtual and physical scenes
* Stable room-scanning API
