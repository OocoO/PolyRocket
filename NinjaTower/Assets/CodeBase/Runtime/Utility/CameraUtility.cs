using UnityEngine;

namespace Carotaa.Code
{
    public static class CameraUtility
    {
        public const string DEFAULT_LAYER = "Default";
        public const string SLIDE_LAYER = "Slide";
        public const string POP_LAYER = "Pop";
        public const string TOAST_LAYER = "Toast";
        private static Camera _cameraCache;

        public static Camera Main => _cameraCache == null ? _cameraCache = Camera.main : _cameraCache;

        public static int GetLayerID(string name)
        {
            // add some cache?
            return SortingLayer.NameToID(name);
        }
    }
}