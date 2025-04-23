# Animation Clip Transform Extractor – User Guide

## 1. Adding the Component

1. Select a GameObject in the hierarchy.  
2. **Add Component ▸ Animation Clip Transform Extractor**.

---

## 2. Inspector Fields

| Field                                   | Purpose                                                                                                  |
| --------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| **Animated Root**                       | The GameObject that the animation will be sampled on. Must match the hierarchy used by the clip.         |
| **Transform To Track**                  | Specific `Transform` whose position / rotation / scale you want to record.                               |
| **Animation Source**                    | The `AnimationClip` to be sampled. Works with legacy, generic or humanoid clips.                         |
| **Time Step**                           | Time between samples in seconds (e.g. `0.05`). Smaller values = more samples, slower extraction.         |
| **Path Step Prefab**                    | Prefab that will be instantiated at each sample position when *Place Path Prefabs* is pressed. Optional. |
| **Position / Rotation / Scale toggles** | Choose which components of the transform should be stored / applied to spawned prefabs.                  |

---

## 3. Extracting Data

1. Fill out the required fields. The **Extract Transform Values** button becomes enabled once everything is valid.  
2. Click **Extract Transform Values**.  
   * The clip is sampled from `0` s to its `length` in increments of `Time Step`.  
   * A serialised list called **Transform Values Sequence** appears at the bottom of the Inspector showing every sample.

> The extractor temporarily puts the Editor into `AnimationMode` to evaluate the clip. Your scene objects are restored to their original state afterwards.

---

## 4. Visualising the Path (optional)

1. Assign **Path Step Prefab** – any prefab will do (sphere, cube, custom indicator, etc.).
2. Press **Place Path Prefabs**.  
   * A parent GameObject named `StepsFor_<Transform>_in_<Clip>` is created.  
   * One prefab instance per sample is spawned and positioned/rotated/scaled according to your toggles.
3. You can delete or undo the parent GameObject to remove the markers.

---

## 5. Typical Use‑Cases

* Visually debug root motion or IK targets along an animation.
* Export the list to a file or another system for custom playback.
* Evaluate the positional accuracy of motion‑capture clips.

---

## 6. Tips & Limitations

* Make sure **Time Step** is reasonable; `0.01` on a 10 s clip creates 1000 samples which may take some time.
* Works only in Editor.
* The extractor samples the clip without playing the **Play Mode** – no need to enter Play.

---

Happy sampling!
