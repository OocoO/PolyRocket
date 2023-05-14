using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public static class MaterialUtility
    {
        [Conditional("UNITY_EDITOR")]
        public static void RebindMaterial(Renderer renderer)
        {
            var duplicate = Object.Instantiate(renderer.material);
            renderer.material = duplicate;
        }

        [Conditional("UNITY_EDITOR")]
        public static void RebindMaterial(Graphic renderer)
        {
            var duplicate = Object.Instantiate(renderer.material);
            renderer.material = duplicate;
        }
    }
}