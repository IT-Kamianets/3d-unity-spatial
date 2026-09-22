# IT-Kamianets 3D Engine: Unity Spatial

`3d-unity-spatial` connects the **IT-Kamianets 3D Engine** with real physical environments.

It provides reusable spatial-computing functionality such as room understanding, physical surfaces, boundaries, anchors, and environment scanning.

### Logic

While `3d-unity-xr` handles interaction with XR hardware, `3d-unity-spatial` handles **understanding the physical space around the user**.

The resulting spatial information should be converted into engine-independent scene data whenever possible.

For VRoom, this package can be used to turn a real room into the initial digital scene.

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
