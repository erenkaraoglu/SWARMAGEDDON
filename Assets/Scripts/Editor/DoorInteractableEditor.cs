using UnityEngine;
using UnityEditor;
using Interaction;

[CustomEditor(typeof(DoorInteractable))]
public class DoorInteractableEditor : BaseInteractableEditor
{
    private bool showDoorSettings = true;
    
    // Door-specific properties
    private SerializedProperty isOpen;
    private SerializedProperty openText;
    private SerializedProperty closeText;
    private SerializedProperty canToggle;
    private SerializedProperty isLocked;
    private SerializedProperty lockedText;
    private SerializedProperty doorType;
    private SerializedProperty doorTransform;
    private SerializedProperty animationDuration;
    private SerializedProperty animationEase;
    private SerializedProperty openAngle;
    private SerializedProperty openOffset;
    private SerializedProperty openSoundIndex;
    private SerializedProperty closeSoundIndex;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Cache door-specific properties
        isOpen = serializedObject.FindProperty("isOpen");
        openText = serializedObject.FindProperty("openText");
        closeText = serializedObject.FindProperty("closeText");
        canToggle = serializedObject.FindProperty("canToggle");
        isLocked = serializedObject.FindProperty("isLocked");
        lockedText = serializedObject.FindProperty("lockedText");
        doorType = serializedObject.FindProperty("doorType");
        doorTransform = serializedObject.FindProperty("doorTransform");
        animationDuration = serializedObject.FindProperty("animationDuration");
        animationEase = serializedObject.FindProperty("animationEase");
        openAngle = serializedObject.FindProperty("openAngle");
        openOffset = serializedObject.FindProperty("openOffset");
        openSoundIndex = serializedObject.FindProperty("openSoundIndex");
        closeSoundIndex = serializedObject.FindProperty("closeSoundIndex");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        DrawInteractionSettings();
        EditorGUILayout.Space();
        DrawDoorAndAnimationSettings();
        EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawDoorAndAnimationSettings()
    {
        showDoorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showDoorSettings, "Door & Animation Settings");
        if (showDoorSettings)
        {
            EditorGUI.indentLevel++;
            
            // Door Type and Basic Settings
            EditorGUILayout.PropertyField(doorType, new GUIContent("Door Type", "Type of door animation"));

            // Door type specific settings
            DoorType currentDoorType = (DoorType)doorType.enumValueIndex;
            
            if (currentDoorType == DoorType.Rotational)
            {
                EditorGUILayout.PropertyField(openAngle, new GUIContent("Open Angle", "Angle to rotate when opening (degrees)"));
            }
            else if (currentDoorType == DoorType.Sliding)
            {
                EditorGUILayout.PropertyField(openOffset, new GUIContent("Open Offset", "Position offset when fully open"));
            }

            EditorGUILayout.PropertyField(doorTransform, new GUIContent("Door Transform", "Transform to animate (leave empty to use this object)"));
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.PropertyField(isOpen, new GUIContent("Is Open", "Initial state of the door"));
            EditorGUILayout.PropertyField(openText, new GUIContent("Open Text", "Text shown when door can be opened"));
            EditorGUILayout.PropertyField(closeText, new GUIContent("Close Text", "Text shown when door can be closed"));
            EditorGUILayout.PropertyField(canToggle, new GUIContent("Can Toggle", "Allow closing an open door"));
            EditorGUILayout.PropertyField(isLocked, new GUIContent("Is Locked", "Whether the door is locked"));
            
            if (isLocked.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(lockedText, new GUIContent("Locked Text", "Text shown when door is locked"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(5);
            
            // Animation Settings
            EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(animationDuration, new GUIContent("Duration", "Duration of open/close animation"));
            EditorGUILayout.PropertyField(animationEase, new GUIContent("Ease", "Animation curve type"));
            

            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    // Override to add door-specific audio settings to the interaction settings
    protected override void DrawInteractionSettings()
    {
        base.DrawInteractionSettings();
        
        if (showInteractionSettings && !string.IsNullOrEmpty(interactionSoundId.stringValue))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(openSoundIndex, new GUIContent("Open Sound Index", "Sound index for opening"));
            EditorGUILayout.PropertyField(closeSoundIndex, new GUIContent("Close Sound Index", "Sound index for closing"));
            EditorGUI.indentLevel--;
        }
    }
}
