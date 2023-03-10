using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public class DynamicImage : MaskableGraphic
    {
        private DynamicSprite _sprite;

        public DynamicSprite Sprite
        {
            get => _sprite;
            set
            {
                if (value == _sprite)
                {
                    return;
                }

                _sprite = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (Sprite != null)
                    return Sprite.Texture;

                return s_WhiteTexture;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (_sprite == null)
            {
                base.OnPopulateMesh(vh);
                return;
            }
            
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

            var uv = Sprite.Uv;
            Color32 color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, uv.min);
            vh.AddVert(new Vector3(v.x, v.w), color32, uv.LeftTop());
            vh.AddVert(new Vector3(v.z, v.w), color32, uv.max);
            vh.AddVert(new Vector3(v.z, v.y), color32, uv.RightDown());

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}