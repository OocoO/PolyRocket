using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Carotaa.Code
{
    public static class ScreenUtility
    {
        public static Vector2Int Size => new Vector2Int(Width, Height);

        public static int Width
        {
            get
            {
#if UNITY_EDITOR
                return (int) Handles.GetMainGameViewSize().x;
#else
				return Screen.width;
#endif
            }
        }

        public static int Height
        {
            get
            {
#if UNITY_EDITOR
                return (int) Handles.GetMainGameViewSize().y;
#else
				return Screen.height;
#endif
            }
        }

        public static Matrix4x4 World2ScreenMatrix(Camera cam)
        {
            var view2Screen = Matrix4x4.TRS (new Vector3 (0.5f * cam.pixelWidth, 0.5f * cam.pixelHeight, 0f),
                Quaternion.identity, new Vector3 (0.5f * cam.pixelWidth, 0.5f * cam.pixelHeight, 1f));

            var mat = view2Screen
                                  * cam.projectionMatrix
                                  * cam.worldToCameraMatrix;

            return mat;
        }
    }
}