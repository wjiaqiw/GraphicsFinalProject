using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace DynamicPortals
{
    [CustomEditor(typeof(PortalBase))]
    public class PortalBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PortalBase portalBase = (PortalBase)target;
            serializedObject.Update();
            
            SerializedProperty renderTextureSize = serializedObject.FindProperty("_renderTextureSize");
            renderTextureSize.vector2IntValue = EditorGUILayout.Vector2IntField(renderTextureSize.displayName, renderTextureSize.vector2IntValue);

            EditorGUILayout.Space();

            SerializedProperty clipPlaneOffset = serializedObject.FindProperty("_clipPlaneOffset");
            SerializedProperty showAdvancedSettings = serializedObject.FindProperty("_showAdvancedSettings");
            showAdvancedSettings.boolValue = EditorGUILayout.Foldout(showAdvancedSettings.boolValue, "Advanced Settings");
            if (showAdvancedSettings.boolValue)
            {
                clipPlaneOffset.floatValue = EditorGUILayout.FloatField(clipPlaneOffset.displayName, clipPlaneOffset.floatValue);
                SerializedProperty changeObserverCam = serializedObject.FindProperty("_changeObserverCam");
                changeObserverCam.boolValue = EditorGUILayout.Toggle(changeObserverCam.displayName, changeObserverCam.boolValue);
                if (changeObserverCam.boolValue)
                {
                    SerializedProperty alternativeCam = serializedObject.FindProperty("_alternativeCam");
                    alternativeCam.objectReferenceValue = EditorGUILayout.ObjectField(alternativeCam.displayName, alternativeCam.objectReferenceValue, typeof(Camera), true);
                }
            }

            EditorGUILayout.Space();

            SerializedProperty isInEditorPreview = serializedObject.FindProperty("_isInEditorPreview");
            if (isInEditorPreview.boolValue) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Editor Preview")) portalBase.IsInEditorPreview = !isInEditorPreview.boolValue;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
