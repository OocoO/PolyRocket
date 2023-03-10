using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Carotaa.Code.Editor
{
    public class UrlDownloader : EditorWindow
    {
        private const string TargetUrl =
            @"http://resource.dcoloring.tapque.com/resource/ifoundit/test/Android/";

        private const string TargetName = @"g1-{0}-{1}";

        private const string LoadDir = "/Users/licarotaa/Documents/Note/FindIt";
        private static UrlDownloader s_instance;

        private int _patience;

        public void OnGUI()
        {
            if (GUILayout.Button("Start Download", GUILayout.MaxWidth(75f)))
                MonoHelper.Instance.StartCoroutine(StartDownload());
        }

        [MenuItem("Window/Url Downloader", priority = 2000)]
        private static void ShowWindow()
        {
            if (!s_instance) s_instance = CreateInstance<UrlDownloader>();

            s_instance.Show();
        }

        private IEnumerator StartDownload()
        {
            for (var i = 0; i < 100; i++)
            {
                _patience = 5;
                for (var j = 0; j < 1000; j++)
                {
                    yield return StartDownload(i, j);
                    if (_patience <= 0) break;
                }
            }
        }

        private IEnumerator StartDownload(int i, int j)
        {
            var fileName = string.Format(TargetName, i.ToString("D2"), j.ToString("D3"));
            var target = TargetUrl + fileName;
            var targetPath = Path.Combine(LoadDir, fileName);
            yield return DownloadFile(target, targetPath);
        }

        private IEnumerator DownloadFile(string url, string destPath)
        {
            var www = new UnityWebRequest(url);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                _patience--;
                EventTrack.LogError(www.error + url);
            }
            else
            {
                var data = www.downloadHandler.data;
                File.WriteAllBytes(destPath, data);
                EventTrack.LogTrace($"File Downloader: Save File {destPath}");
            }
        }
    }
}