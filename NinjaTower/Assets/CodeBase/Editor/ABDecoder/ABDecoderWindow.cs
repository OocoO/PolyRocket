using System.IO;
using UnityEditor;
using UnityEngine;

namespace Carotaa.Code.Editor
{
    public class ABDecoderWindow : EditorWindow
    {
        private static ABDecoderWindow s_instance;
        private string _currentFolder;

        private string _currentPath;

        public void OnGUI()
        {
            if (GUILayout.Button("Add File", GUILayout.MaxWidth(75f))) BrowseForFile();

            if (GUILayout.Button("Add Folder", GUILayout.MaxWidth(75f))) BrowseForFolder();

            GUILayout.Label($"Current Bundle: {_currentPath}");

            if (GUILayout.Button("Export")) ABDecoderUtils.Decode(_currentPath);
            GUILayout.Label("Current Folder {_currentFolder}");
            if (GUILayout.Button("Export Folder"))
            {
                var destFolder = Path.Combine(_currentFolder, "Output");
                ABDecoderUtils.DecodeBatch(_currentFolder, destFolder);
            }
        }

        [MenuItem("Window/AssetBundle Decoder", priority = 2000)]
        private static void ShowWindow()
        {
            if (!s_instance) s_instance = CreateInstance<ABDecoderWindow>();

            s_instance.Show();
        }

        private void BrowseForFile()
        {
            var newPath = EditorUtility.OpenFilePanelWithFilters("Bundle Folder", string.Empty, new string[] { });
            if (!string.IsNullOrEmpty(newPath)) _currentPath = newPath;
        }

        private void BrowseForFolder()
        {
            var folderPath = EditorUtility.OpenFolderPanel("Bundle Folder", string.Empty, string.Empty);
            if (!string.IsNullOrEmpty(folderPath)) _currentFolder = folderPath;
        }
    }
}