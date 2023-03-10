using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public abstract class RectMeshEffect : BaseMeshEffect
    {
        private RectTransform _rect;

        // ReSharper disable once InconsistentNaming
        public RectTransform rectTransform => _rect ? _rect : _rect = (transform as RectTransform);
    }
}