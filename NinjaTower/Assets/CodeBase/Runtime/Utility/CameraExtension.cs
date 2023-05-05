using UnityEngine;

namespace Carotaa.Code
{
    public static class CameraExtension
    {
        public static float GetHalfWidth(this Camera camera)
        {
            return camera.aspect * camera.orthographicSize;
        }


        public static Rect GetViewBox(this Camera camera, float scaler = 1f)
        {
            var halfWidth = GetHalfWidth(camera);
            var halfHeight = camera.orthographicSize;
            var pos = camera.transform.position;

            return new Rect(pos, new Vector2(halfWidth, halfHeight) * (2f * scaler));
        }
        
    }
}