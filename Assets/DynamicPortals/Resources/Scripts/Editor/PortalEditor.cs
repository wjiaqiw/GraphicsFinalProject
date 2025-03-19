using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DynamicPortals
{
    [CustomEditor(typeof(Portal)), CanEditMultipleObjects]
    public class PortalEditor : PortalBaseEditor
    {
        public override void OnInspectorGUI()
        {
            PortalBase portalBase = (PortalBase)target;
            serializedObject.Update();

            SerializedProperty renderTextureSize = serializedObject.FindProperty("_renderTextureSize");
            renderTextureSize.vector2IntValue = EditorGUILayout.Vector2IntField(renderTextureSize.displayName, renderTextureSize.vector2IntValue);
            SerializedProperty targetPortal = serializedObject.FindProperty("_targetPortal");
            targetPortal.objectReferenceValue = EditorGUILayout.ObjectField(targetPortal.displayName, targetPortal.objectReferenceValue, typeof(Portal), true);
            SerializedProperty maxRecursion = serializedObject.FindProperty("_maxRecursion");
            maxRecursion.intValue = EditorGUILayout.IntField(maxRecursion.displayName, maxRecursion.intValue);

            EditorGUILayout.Space();

            SerializedProperty clipPlaneOffset = serializedObject.FindProperty("_clipPlaneOffset");
            SerializedProperty renderWhen = serializedObject.FindProperty("_renderWhen");
            SerializedProperty showAdvancedSettings = serializedObject.FindProperty("_showAdvancedSettings");
            showAdvancedSettings.boolValue = EditorGUILayout.Foldout(showAdvancedSettings.boolValue, "Advanced Settings");
            if (showAdvancedSettings.boolValue)
            {
                clipPlaneOffset.floatValue = EditorGUILayout.FloatField(clipPlaneOffset.displayName, clipPlaneOffset.floatValue);
                renderWhen.enumValueIndex = (int)(Portal.RenderWhen)EditorGUILayout.EnumPopup(renderWhen.displayName, (Portal.RenderWhen)renderWhen.enumValueIndex);
                SerializedProperty changeObserverCam = serializedObject.FindProperty("_changeObserverCam");
                changeObserverCam.boolValue = EditorGUILayout.Toggle(changeObserverCam.displayName, changeObserverCam.boolValue);
                if (changeObserverCam.boolValue)
                {
                    SerializedProperty alternativeCam = serializedObject.FindProperty("_alternativeCam");
                    alternativeCam.objectReferenceValue = EditorGUILayout.ObjectField(alternativeCam.displayName, alternativeCam.objectReferenceValue, typeof(Camera), true);
                }

                EditorGUILayout.Space();

                SerializedProperty playerVelocityMultiplier = serializedObject.FindProperty("_playerVelocityMultiplier");
                playerVelocityMultiplier.floatValue = EditorGUILayout.Slider(playerVelocityMultiplier.displayName, playerVelocityMultiplier.floatValue, 0, 10);
                SerializedProperty objectVelocityMultiplier = serializedObject.FindProperty("_objectVelocityMultiplier");
                objectVelocityMultiplier.floatValue = EditorGUILayout.Slider(objectVelocityMultiplier.displayName, objectVelocityMultiplier.floatValue, 0, 10);
                SerializedProperty portalVelocityMultiplier = serializedObject.FindProperty("_portalVelocityMultiplier");
                portalVelocityMultiplier.floatValue = EditorGUILayout.Slider(portalVelocityMultiplier.displayName, portalVelocityMultiplier.floatValue, 0, 10);
            }

            EditorGUILayout.Space();

            SerializedProperty isInEditorPreview = serializedObject.FindProperty("_isInEditorPreview");
            if (isInEditorPreview.boolValue) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Editor Preview")) portalBase.IsInEditorPreview = !isInEditorPreview.boolValue;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
