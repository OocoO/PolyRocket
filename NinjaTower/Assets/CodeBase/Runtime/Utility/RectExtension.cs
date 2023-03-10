using UnityEngine;

namespace Carotaa.Code
{
    public static class RectExtension
    {
        public static Vector2 LeftTop(this Rect self)
        {
            return new Vector2(self.xMin, self.yMax);
        }

        public static Vector2 RightDown(this Rect self)
        {
            return new Vector2(self.xMax, self.yMin);
        }
    }
}