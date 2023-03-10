using UnityEditor;

namespace Carotaa.Code.Editor
{
    public class MultiScreenRectEditor : UnityEditor.Editor
    {
        private SerializedProperty _presetType;
        private SerializedProperty _xNormalProperty;
        private SerializedProperty _yNormalProperty;
        private MultiScreenRect Target => (MultiScreenRect) target;

        private void OnEnable()
        {
            _presetType = serializedObject.FindProperty("m_ProfileType");
            _xNormalProperty = serializedObject.FindProperty("m_IsNormalizeHorizontal");
            _yNormalProperty = serializedObject.FindProperty("m_IsNormalizeVertical");
        }

        public override void OnInspectorGUI()
        {
            var selected = (MultiScreenRect.Device) _presetType.enumValueIndex;
            var change = (MultiScreenRect.Device) EditorGUILayout.EnumPopup("Profile Type", selected);
            if (change != selected)
            {
                _presetType.enumValueIndex = (int) change;
                // set value by editor
                // serializedObject.ApplyModifiedProperties();
                Target.SetProfileEditor(change);
            }

            var xOrigin = _xNormalProperty.boolValue;
            var xChange = EditorGUILayout.Toggle("Scale X Axis via Screen Size", xOrigin);
            if (xChange != xOrigin)
            {
                _xNormalProperty.boolValue = xChange;
                serializedObject.ApplyModifiedProperties();
            }

            var yOrigin = _yNormalProperty.boolValue;
            var yChange = EditorGUILayout.Toggle("Scale Y Axis via Screen Size", yOrigin);
            if (yOrigin != yChange)
            {
                _yNormalProperty.boolValue = yChange;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}