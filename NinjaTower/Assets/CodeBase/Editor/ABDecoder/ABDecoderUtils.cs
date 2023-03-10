using System.IO;
using UnityEngine;

namespace Carotaa.Code.Editor
{
    public static class ABDecoderUtils
    {
        public static void ExportAssetBundle(AssetBundle self, string targetPath)
        {
            var objs = self.LoadAllAssets();
            foreach (var obj in objs)
                if (obj is Sprite sTarget)
                    ExportSprite(sTarget, targetPath);
                else if (obj is TextAsset tTarget) ExportText(tTarget, targetPath);
        }

        public static void Decode(string sourcePath)
        {
            var fileDir = Path.GetDirectoryName(sourcePath);
            Decode(sourcePath, fileDir);
        }

        public static void Decode(string sourcePath, string destPath)
        {
            var ab = LoadAssetBundle(sourcePath);
            if (!ab)
            {
                EventTrack.LogError("Decode Failed: File not exist");
                return;
            }

            var fileName = Path.GetFileName(sourcePath);
            var destDir = Path.Combine(destPath, $"{fileName}_BundleOutput");

            ExportAssetBundle(ab, destDir);
            ab.Unload(true);
        }

        public static void DecodeBatch(string sourceDir)
        {
            var files = Directory.GetFiles(sourceDir);
            foreach (var file in files) Decode(file);
        }

        public static void DecodeBatch(string sourceDir, string destDir)
        {
            var files = Directory.GetFiles(sourceDir);
            foreach (var file in files) Decode(file, destDir);
        }

        private static AssetBundle LoadAssetBundle(string path)
        {
            return AssetBundle.LoadFromFile(path);
        }

        private static void ExportSprite(Sprite sprite, string targetDir)
        {
            var texture = CreateReadableTexture(sprite.texture);
            var bytes = texture.EncodeToPNG();
            var targetName = Path.Combine(targetDir, $"{sprite.name}.png");

            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            File.WriteAllBytes(targetName, bytes);
        }

        private static Texture2D CreateReadableTexture(Texture2D texture2d)
        {
            var renderTexture = RenderTexture.GetTemporary(
                texture2d.width,
                texture2d.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture2d, renderTexture);

            var previous = RenderTexture.active;

            RenderTexture.active = renderTexture;

            var readableTextur2D = new Texture2D(texture2d.width, texture2d.height);

            readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

            readableTextur2D.Apply();

            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(renderTexture);

            return readableTextur2D;
        }

        private static void ExportText(TextAsset target, string targetPath)
        {
            var fileName = Path.Combine(targetPath, target.name);
            File.WriteAllText(fileName, target.text);
        }
    }
}