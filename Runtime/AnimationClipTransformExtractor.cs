using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script to extract transform values from an animation at specified time intervals.
/// Works in editor mode only.
/// </summary>
public class AnimationClipTransformExtractor : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("The root GameObject that the animation is applied to")]
    public GameObject AnimatedRoot;

    [Tooltip("The transform to track during animation playback")]
    public Transform TransformToTrack;

    [Tooltip("Time interval between samples (in seconds)")]
    public float TimeStep = 0.1f;

    [Tooltip("Animation clip to extract values from")]
    public AnimationClip AnimationSource;

    [Header("Path Visualization")]
    [Tooltip("Prefab to place at each point along the extracted path")]
    public GameObject PathStepPrefab;

    [Tooltip("Apply position values from the extracted data")]
    public bool UsePosition = true;

    [Tooltip("Apply rotation values from the extracted data")]
    public bool UseRotation = true;

    [Tooltip("Apply scale values from the extracted data")]
    public bool UseScale = true;

    [System.Serializable]
    public struct TransformValues
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public float Time;

        public override string ToString()
        {
            return $"Time: {Time:F2}s, Pos: {Position}, Rot: {Rotation}, Scale: {Scale}";
        }
    }

    [Header("Results")]
    [Tooltip("Sequence of transform values extracted from the animation")]
    public List<TransformValues> TransformValuesSequence = new List<TransformValues>();

    // Reference to created path objects for cleanup
    private List<GameObject> _pathObjects = new List<GameObject>();

    #if UNITY_EDITOR
    /// <summary>
    /// Extracts transform values from the animation at specified time intervals
    /// </summary>
    public void Extract()
    {
        // Validate required components
        if (AnimatedRoot == null)
        {
            Debug.LogError("Animated Root is not assigned!");
            return;
        }

        if (TransformToTrack == null)
        {
            Debug.LogError("Transform to track is not assigned!");
            return;
        }

        if (AnimationSource == null)
        {
            Debug.LogError("Animation source is not assigned!");
            return;
        }

        if (TimeStep <= 0)
        {
            Debug.LogError("Time step must be greater than 0!");
            return;
        }

        // Clear previous results
        TransformValuesSequence.Clear();
        
        // Store original transform values to restore later
        // Store the entire transform hierarchy state
        TransformState[] originalStates = StoreHierarchyState(AnimatedRoot.transform);
        
        // Sample the animation at specified intervals
        float animationLength = AnimationSource.length;
        float currentTime = 0f;
        
        Debug.Log($"Starting extraction from animation '{AnimationSource.name}' (Length: {animationLength}s)");
        
        while (currentTime <= animationLength)
        {
            // Sample the animation at the current time
            AnimationMode.StartAnimationMode();
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(AnimatedRoot, AnimationSource, currentTime);
            AnimationMode.EndSampling();
            
            // Record the transform values
            TransformValues values = new TransformValues
            {
                Position = TransformToTrack.position,
                Rotation = TransformToTrack.eulerAngles,
                Scale = TransformToTrack.localScale,
                Time = currentTime
            };
            
            TransformValuesSequence.Add(values);
            
            // Move to the next time step
            currentTime += TimeStep;
        }
        
        AnimationMode.StopAnimationMode();
        
        // Restore original transform values
        RestoreHierarchyState(originalStates);
        
        Debug.Log($"Extraction complete. Extracted {TransformValuesSequence.Count} samples.");
    }
    
    /// <summary>
    /// Places prefabs along the path defined by the extracted transform values
    /// </summary>
    public void PlacePathPrefabs()
    {
        if (TransformValuesSequence == null || TransformValuesSequence.Count == 0)
        {
            Debug.LogError("No transform values available. Run Extract first!");
            return;
        }
        
        if (PathStepPrefab == null)
        {
            Debug.LogError("Path Step Prefab is not assigned!");
            return;
        }
        
        // Create a parent object to hold all path steps with the specified naming format
        string transformName = TransformToTrack != null ? TransformToTrack.name : "Unknown";
        string animationName = AnimationSource != null ? AnimationSource.name : "Unknown";
        string parentName = $"StepsFor_{transformName}_in_{animationName}";
        
        GameObject pathParent = new GameObject(parentName);
        Undo.RegisterCreatedObjectUndo(pathParent, "Create Path Parent");
        _pathObjects.Add(pathParent);
        
        // Get the prefab's original transform values
        Vector3 prefabOriginalPosition = Vector3.zero;
        Quaternion prefabOriginalRotation = Quaternion.identity;
        Vector3 prefabOriginalScale = Vector3.one;
        
        // Try to get the prefab's original transform values
        GameObject tempPrefab = PrefabUtility.InstantiatePrefab(PathStepPrefab) as GameObject;
        if (tempPrefab != null)
        {
            prefabOriginalPosition = tempPrefab.transform.position;
            prefabOriginalRotation = tempPrefab.transform.rotation;
            prefabOriginalScale = tempPrefab.transform.localScale;
            DestroyImmediate(tempPrefab);
        }
        
        Debug.Log($"Placing {TransformValuesSequence.Count} path prefabs in '{parentName}'...");
        string transformSettings = $"Using: {(UsePosition ? "Position " : "")}{(UseRotation ? "Rotation " : "")}{(UseScale ? "Scale" : "")}";
        Debug.Log(transformSettings);
        
        // Place prefabs at each point in the path
        for (int i = 0; i < TransformValuesSequence.Count; i++)
        {
            TransformValues values = TransformValuesSequence[i];
            
            // Create the prefab instance
            GameObject pathStep = PrefabUtility.InstantiatePrefab(PathStepPrefab) as GameObject;
            if (pathStep == null)
            {
                pathStep = Instantiate(PathStepPrefab);
            }
            
            // Set the name to include the time
            pathStep.name = $"PathStep_{i:D3}_Time_{values.Time:F2}s";
            
            // Set the transform values based on flags
            if (UsePosition)
            {
                pathStep.transform.position = values.Position;
            }
            else
            {
                pathStep.transform.position = prefabOriginalPosition;
            }
            
            if (UseRotation)
            {
                pathStep.transform.eulerAngles = values.Rotation;
            }
            else
            {
                pathStep.transform.rotation = prefabOriginalRotation;
            }
            
            if (UseScale)
            {
                pathStep.transform.localScale = values.Scale;
            }
            else
            {
                pathStep.transform.localScale = prefabOriginalScale;
            }
            
            // Parent to the path parent
            pathStep.transform.SetParent(pathParent.transform, true);
            
            // Register for undo
            Undo.RegisterCreatedObjectUndo(pathStep, "Create Path Step");
            
            _pathObjects.Add(pathStep);
        }
        
        // Select the parent in the hierarchy
        Selection.activeGameObject = pathParent;
        
        Debug.Log($"Path creation complete. Created {TransformValuesSequence.Count} path steps.");
    }
    
    /// <summary>
    /// Cleans up any previously created path objects
    /// </summary>
    private void CleanupPathObjects()
    {
        foreach (GameObject obj in _pathObjects)
        {
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }
        
        _pathObjects.Clear();
    }
    
    // Helper class to store transform state
    private class TransformState
    {
        public Transform Transform;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        
        public TransformState(Transform transform)
        {
            Transform = transform;
            LocalPosition = transform.localPosition;
            LocalRotation = transform.localRotation;
            LocalScale = transform.localScale;
        }
        
        public void Restore()
        {
            Transform.localPosition = LocalPosition;
            Transform.localRotation = LocalRotation;
            Transform.localScale = LocalScale;
        }
    }
    
    // Store the state of the entire hierarchy
    private TransformState[] StoreHierarchyState(Transform root)
    {
        Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
        TransformState[] states = new TransformState[allTransforms.Length];
        
        for (int i = 0; i < allTransforms.Length; i++)
        {
            states[i] = new TransformState(allTransforms[i]);
        }
        
        return states;
    }
    
    // Restore the state of the entire hierarchy
    private void RestoreHierarchyState(TransformState[] states)
    {
        foreach (var state in states)
        {
            state.Restore();
        }
    }
    #endif
} 