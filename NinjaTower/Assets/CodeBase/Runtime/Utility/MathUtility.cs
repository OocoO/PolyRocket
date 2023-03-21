using UnityEngine;

namespace Carotaa.Code
{
    public static class MathUtility
    {
        public static float Sigmoid(float x)
        {
            return 1f / (1 + Mathf.Exp(-x));
        }
    }
}