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
    }
}