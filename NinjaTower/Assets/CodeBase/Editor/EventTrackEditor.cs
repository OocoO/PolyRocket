using UnityEditor;

namespace Carotaa.Code.Editor
{
    [CustomEditor(typeof(EventTrack))]
    public class EventTrackEditor: UnityEditor.Editor
    {
        private string _serachPattern = "";
        
        public override bool RequiresConstantRepaint() => true;
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("---Start DebugParams---");
            var track = target as EventTrack;
            if (!track)
            {
                return;
            }

            foreach (var kvp in track.DebugParams)
            {
                EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value}");
            }
            
            EditorGUILayout.LabelField("---End DebugParams---");
        }
    }
}