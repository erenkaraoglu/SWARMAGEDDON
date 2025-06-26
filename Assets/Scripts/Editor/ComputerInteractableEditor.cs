using UnityEngine;
using UnityEditor;
using Interaction;

[CustomEditor(typeof(ComputerInteractable))]
public class ComputerInteractableEditor : BaseInteractableEditor
{
    private bool showComputerSettings = true;
    private bool showChairSettings = true;
    private bool showCameraSettings = true;
    
    // Computer-specific properties
    private SerializedProperty sitText;
    private SerializedProperty standText;
    private SerializedProperty isInUse;
    private SerializedProperty chairPosition;
    private SerializedProperty sitAnimationDuration;
    private SerializedProperty sitAnimationEase;
    private SerializedProperty computerCamera;
    private SerializedProperty computerCameraPriority;
    private SerializedProperty defaultCameraPriority;
    private SerializedProperty sitSoundIndex;
    private SerializedProperty standSoundIndex;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Cache computer-specific properties
        sitText = serializedObject.FindProperty("sitText");
        standText = serializedObject.FindProperty("standText");
        isInUse = serializedObject.FindProperty("isInUse");
        chairPosition = serializedObject.FindProperty("chairPosition");
        sitAnimationDuration = serializedObject.FindProperty("sitAnimationDuration");
        sitAnimationEase = serializedObject.FindProperty("sitAnimationEase");
        computerCamera = serializedObject.FindProperty("computerCamera");
        computerCameraPriority = serializedObject.FindProperty("computerCameraPriority");
        defaultCameraPriority = serializedObject.FindProperty("defaultCameraPriority");
        sitSoundIndex = serializedObject.FindProperty("sitSoundIndex");
        standSoundIndex = serializedObject.FindProperty("standSoundIndex");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        DrawInteractionSettings();
        EditorGUILayout.Space();
        DrawComputerSettings();
        EditorGUILayout.Space();
        DrawChairSettings();
        EditorGUILayout.Space();
        DrawCameraSettings();
        EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawComputerSettings()
    {
        showComputerSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showComputerSettings, "Computer Settings");
        if (showComputerSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(sitText, new GUIContent("Sit Text", "Text shown when computer can be used"));
            EditorGUILayout.PropertyField(standText, new GUIContent("Stand Text", "Text shown when player can stand up"));
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(isInUse, new GUIContent("Is In Use", "Whether computer is currently being used"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    private void DrawChairSettings()
    {
        showChairSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showChairSettings, "Chair & Animation Settings");
        if (showChairSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(chairPosition, new GUIContent("Chair Position", "Transform where player will sit"));
            
            if (chairPosition.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Chair Position is required for the computer to work properly.", MessageType.Warning);
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sitAnimationDuration, new GUIContent("Sit Duration", "Duration of sit/stand animation"));
            EditorGUILayout.PropertyField(sitAnimationEase, new GUIContent("Animation Ease", "Animation curve type"));
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    private void DrawCameraSettings()
    {
        showCameraSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showCameraSettings, "Camera Settings");
        if (showCameraSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(computerCamera, new GUIContent("Computer Camera", "Cinemachine camera for computer view"));
            
            if (computerCamera.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Computer Camera is required for camera switching to work.", MessageType.Warning);
            }
            
            EditorGUILayout.PropertyField(computerCameraPriority, new GUIContent("Computer Priority", "Priority when using computer"));
            EditorGUILayout.PropertyField(defaultCameraPriority, new GUIContent("Default Priority", "Priority when not using computer"));
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    // Override to add computer-specific audio settings
    protected override void DrawInteractionSettings()
    {
        base.DrawInteractionSettings();
        
        if (showInteractionSettings && !string.IsNullOrEmpty(interactionSoundId.stringValue))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(sitSoundIndex, new GUIContent("Sit Sound Index", "Sound index for sitting down"));
            EditorGUILayout.PropertyField(standSoundIndex, new GUIContent("Stand Sound Index", "Sound index for standing up"));
            EditorGUI.indentLevel--;
        }
    }
}
