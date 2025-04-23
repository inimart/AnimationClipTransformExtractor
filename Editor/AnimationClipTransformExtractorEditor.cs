#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for ExtractTransformValuesFromAnimation that adds an Extract button to the inspector
/// </summary>
[CustomEditor(typeof(AnimationClipTransformExtractor))]
public class AnimationClipTransformExtractorEditor : Editor
{
    private AnimationClipTransformExtractor _extractor;
    private bool _showResults = false;
    private bool _showPathSettings = true;
    
    private void OnEnable()
    {
        _extractor = (AnimationClipTransformExtractor)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Extract Transform Values From Animation", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This component extracts transform values from an animation at specified time intervals.", MessageType.Info);
        
        // Draw animation settings
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimatedRoot"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TransformToTrack"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationSource"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeStep"));
        
        EditorGUILayout.Space(10);
        
        // Path visualization settings
        _showPathSettings = EditorGUILayout.Foldout(_showPathSettings, "Path Visualization Settings", true);
        if (_showPathSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PathStepPrefab"));
            
            // Transform flags with toggle controls
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Transform Values to Use");
            
            GUILayout.FlexibleSpace();
            
            SerializedProperty positionProp = serializedObject.FindProperty("UsePosition");
            SerializedProperty rotationProp = serializedObject.FindProperty("UseRotation");
            SerializedProperty scaleProp = serializedObject.FindProperty("UseScale");
            
            positionProp.boolValue = GUILayout.Toggle(positionProp.boolValue, "Position", EditorStyles.miniButton);
            rotationProp.boolValue = GUILayout.Toggle(rotationProp.boolValue, "Rotation", EditorStyles.miniButton);
            scaleProp.boolValue = GUILayout.Toggle(scaleProp.boolValue, "Scale", EditorStyles.miniButton);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space(10);
        
        // Extract button
        GUI.enabled = _extractor.AnimatedRoot != null && _extractor.TransformToTrack != null && 
                     _extractor.AnimationSource != null && _extractor.TimeStep > 0;
        
        if (GUILayout.Button("Extract Transform Values", GUILayout.Height(30)))
        {
            _extractor.Extract();
            _showResults = true;
            EditorUtility.SetDirty(_extractor);
        }
        
        if (!GUI.enabled)
        {
            EditorGUILayout.HelpBox("The Extract button is disabled because one or more required fields are not set correctly.", MessageType.Warning);
        }
        
        GUI.enabled = true;
        
        // Place Path Prefabs button
        EditorGUILayout.Space(5);
        GUI.enabled = _extractor.TransformValuesSequence != null && 
                     _extractor.TransformValuesSequence.Count > 0 && 
                     _extractor.PathStepPrefab != null;
                     
        if (GUILayout.Button("Place Path Prefabs", GUILayout.Height(30)))
        {
            _extractor.PlacePathPrefabs();
        }
        
        if (!GUI.enabled)
        {
            if (_extractor.TransformValuesSequence == null || _extractor.TransformValuesSequence.Count == 0)
            {
                EditorGUILayout.HelpBox("Extract transform values first before placing path prefabs.", MessageType.Info);
            }
            else if (_extractor.PathStepPrefab == null)
            {
                EditorGUILayout.HelpBox("Assign a Path Step Prefab to place along the path.", MessageType.Info);
            }
        }
        
        GUI.enabled = true;
        
        // Results section
        if (_extractor.TransformValuesSequence != null && _extractor.TransformValuesSequence.Count > 0)
        {
            EditorGUILayout.Space(10);
            
            _showResults = EditorGUILayout.Foldout(_showResults, $"Results ({_extractor.TransformValuesSequence.Count} samples)", true);
            
            if (_showResults)
            {
                EditorGUI.indentLevel++;
                
                SerializedProperty sequenceProperty = serializedObject.FindProperty("TransformValuesSequence");
                EditorGUILayout.PropertyField(sequenceProperty, true);
                
                EditorGUI.indentLevel--;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif 