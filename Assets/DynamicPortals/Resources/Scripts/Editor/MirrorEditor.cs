using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DynamicPortals
{
    [CustomEditor(typeof(Mirror)), CanEditMultipleObjects]
    public class MirrorEditor : PortalBaseEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
