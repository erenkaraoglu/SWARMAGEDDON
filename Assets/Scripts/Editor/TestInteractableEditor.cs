using UnityEngine;
using UnityEditor;
using Interaction;

[CustomEditor(typeof(TestInteractable))]
public class TestInteractableEditor : BaseInteractableEditor
{
    private bool showTestSettings = true;
    
    private SerializedProperty debugMessage;
    private SerializedProperty logInteractor;
    private SerializedProperty interactionCount;
    private SerializedProperty alternativeSoundId;
    private SerializedProperty useAlternativeSound;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        debugMessage = serializedObject.FindProperty("debugMessage");
        logInteractor = serializedObject.FindProperty("logInteractor");
        interactionCount = serializedObject.FindProperty("interactionCount");
        alternativeSoundId = serializedObject.FindProperty("alternativeSoundId");
        useAlternativeSound = serializedObject.FindProperty("useAlternativeSound");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        DrawInteractionSettings();
        EditorGUILayout.Space();
        DrawTestSettings();
        EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawTestSettings()
    {
        showTestSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTestSettings, "Test Settings");
        if (showTestSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(debugMessage, new GUIContent("Debug Message", "Message to log when interacted"));
            EditorGUILayout.PropertyField(logInteractor, new GUIContent("Log Interactor", "Include interactor name in debug message"));
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(interactionCount, new GUIContent("Interaction Count", "Number of times this has been interacted with"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space(5);
            
            // Alternative Audio in same section
            EditorGUILayout.LabelField("Alternative Audio", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useAlternativeSound, new GUIContent("Use Alternative Sound", "Use alternative sound instead of base interaction sound"));
            
            if (useAlternativeSound.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(alternativeSoundId, new GUIContent("Alternative Sound ID", "ID of alternative sound to play"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}