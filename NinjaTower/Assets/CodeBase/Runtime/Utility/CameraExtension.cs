using UnityEngine;

namespace Carotaa.Code
{
    public static class CameraExtension
    {
        public static float GetHalfWidth(this Camera camera)
        {
            return camera.aspect * camera.orthographicSize;
        }


        public static Bounds GetViewBound(this Camera camera, float scaler = 1f)
        {
            var halfWidth = GetHalfWidth(camera);
            var halfHeight = camera.orthographicSize;
            var pos = camera.transform.position;
            pos.z = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
            
            var size = new Vector3(halfWidth, halfHeight);
            scaler *= 2f;

            size.z = camera.farClipPlane - camera.nearClipPlane;

            return new Bounds(pos, size * scaler);
        }
    }
}