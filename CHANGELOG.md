# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `RoomScanner`: MRUK room → `SceneData` (needs `com.meta.xr.mrutilitykit`; a no-op without it), `MrukLabelMap`.
- `RoomMetrics`, `RoomLoader`.

### Fixed

- MRUK volume bounds keep min <= max after the z flip to schema space.
