using UnityEngine;
using UnityEditor;
using Interaction;

[CustomEditor(typeof(BaseInteractable), true)]
public class BaseInteractableEditor : Editor
{
    protected bool showInteractionSettings = true;
    
    protected SerializedProperty interactionText;
    protected SerializedProperty canInteract;
    protected SerializedProperty highlightOnHover;
    protected SerializedProperty interactionSoundId;
    protected SerializedProperty playSound3D;
    protected SerializedProperty highlightColor;
    
    protected virtual void OnEnable()
    {
        // Cache serialized properties
        interactionText = serializedObject.FindProperty("interactionText");
        canInteract = serializedObject.FindProperty("canInteract");
        highlightOnHover = serializedObject.FindProperty("highlightOnHover");
        interactionSoundId = serializedObject.FindProperty("interactionSoundId");
        playSound3D = serializedObject.FindProperty("playSound3D");
        highlightColor = serializedObject.FindProperty("highlightColor");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        DrawInteractionSettings();
        EditorGUILayout.Space();
        
        // Draw remaining properties that aren't handled by base class
        DrawRemainingProperties();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    protected virtual void DrawInteractionSettings()
    {
        showInteractionSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showInteractionSettings, "Interaction Settings");
        if (showInteractionSettings)
        {
            EditorGUI.indentLevel++;
            
            // Basic Interaction Settings
            EditorGUILayout.PropertyField(interactionText, new GUIContent("Interaction Text", "Text displayed to the player"));
            EditorGUILayout.PropertyField(canInteract, new GUIContent("Can Interact", "Whether this object can be interacted with"));
            
            EditorGUILayout.Space(5);
            
            // Highlight Settings
            EditorGUILayout.LabelField("Highlight", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(highlightOnHover, new GUIContent("Highlight On Hover", "Enable highlighting when player looks at this object"));
            
            if (highlightOnHover.boolValue)
            {
                EditorGUILayout.PropertyField(highlightColor, new GUIContent("Highlight Color", "Color of the outline highlight"));
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.PropertyField(interactionSoundId, new GUIContent("Sound ID", "ID of the sound to play on interaction"));
            
            if (!string.IsNullOrEmpty(interactionSoundId.stringValue))
            {
                EditorGUILayout.PropertyField(playSound3D, new GUIContent("3D Audio", "Whether to play sound at object position"));
            }
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    // This is now a legacy method kept for compatibility
    protected virtual void DrawAudioAndHighlightSettings()
    {
        // No longer needed - settings are now in DrawInteractionSettings
    }
    
    protected virtual void DrawRemainingProperties()
    {
        // Override in derived classes to draw additional properties
    }
    
    protected void DrawHelpBox(string message, MessageType messageType)
    {
        EditorGUILayout.HelpBox(message, messageType);
    }
}