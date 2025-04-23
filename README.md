# Animation Clip Transform Extractor for Unity

A small **Editor‑only** utility that samples an `AnimationClip` at fixed time steps and records the position / rotation / scale of a chosen `Transform`.  Optionally spawns prefab markers along the path for easy visualisation.

---

## Features

* Extracts **position**, **rotation**, **scale** – individually toggleable
* Works with any `AnimationClip` on any hierarchy (humanoid, generic, legacy)
* Custom Inspector with one‑click **Extract Transform Values** and **Place Path Prefabs** buttons
* Generates a list of samples that you can inspect or export
* Creates a parent GameObject grouping all prefab markers for clean hierarchy

---

## Installation

### Using Unity Package Manager

* Open Unity and your project
* Go to Window > Package Manager
* Click the "+" button and select "Install package from git URL..."
* [GitHub - inimart/SessionLogger: Useful Session Logger for Unity3D

---

## Quick Start

1. Add **AnimationClipTransformExtractor** to any GameObject.
2. In the Inspector:
   1. **Animated Root** – drag the root GameObject that the clip animates.
   2. **Transform To Track** – drag the Transform whose curve data you want to capture.
   3. **Animation Source** – assign the `AnimationClip`.
   4. **Time Step** – sampling interval in seconds (e.g. *0.1*).
   5. (Optional) **Path Step Prefab** – prefab that will be instantiated at each sample point.
   6. Toggle which transform components to record (Position / Rotation / Scale).
3. Click **Extract Transform Values** – the list of samples appears at the bottom of the Inspector.
4. (Optional) Click **Place Path Prefabs** to visualise the path in the Scene.

---

## Requirements & Limitations

* Unity **2021.3 LTS** or newer
* Editor‑only – does **not** run in builds
* Extraction is performed on the main thread; very long clips with small time steps may take noticeable time

---

## License

MIT – do what you want, attribution appreciated.
