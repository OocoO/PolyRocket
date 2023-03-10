using UnityEngine;

namespace Carotaa.Code
{
    public static class MathV
    {
        public static Vector2 Clamp(Vector2 target, Vector2 min, Vector2 max)
        {
            var xx = Mathf.Clamp(target.x, min.x, max.x);
            var yy = Mathf.Clamp(target.y, min.y, max.y);
            return new Vector2(xx, yy);
        }

        public static Vector3 Clamp(Vector3 target, Vector3 min, Vector3 max)
        {
            var xx = Mathf.Clamp(target.x, min.x, max.x);
            var yy = Mathf.Clamp(target.y, min.y, max.y);
            var zz = Mathf.Clamp(target.z, min.z, max.z);
            return new Vector3(xx, yy, zz);
        }

        public static Vector4 Clamp(Vector4 target, Vector4 min, Vector4 max)
        {
            var xx = Mathf.Clamp(target.x, min.x, max.x);
            var yy = Mathf.Clamp(target.y, min.y, max.y);
            var zz = Mathf.Clamp(target.z, min.z, max.z);
            var ww = Mathf.Clamp(target.w, min.w, max.w);
            return new Vector4(xx, yy, zz, ww);
        }
    }
}