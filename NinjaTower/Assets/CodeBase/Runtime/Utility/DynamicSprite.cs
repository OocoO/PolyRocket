using System.Collections.Generic;
using UnityEngine;

namespace Carotaa.Code
{
    public class DynamicSprite
    {
        private readonly RenderTexture _rt;
        public readonly Rect Rect;
        public RenderTexture Texture => _rt;

        // store sprite into single rt
        public DynamicSprite(int width, int height)
        {
            _rt = new RenderTexture(width, height, 0) {
                name = $"DynamicSprite {width}x{height}"
            };

            Rect = new Rect(0, 0, width, height);
        }

        // share same render texture
        public DynamicSprite (RenderTexture rt, Rect rect)
        {
            _rt = rt;
            Rect = rect;
        }
        
        public Rect Uv
        {
            get
            {
                if (!Texture) return Rect.zero;

                var texelSize = Texture.texelSize;
                var pos = Rect.position * texelSize;
                var size = Rect.size * texelSize;

                return new Rect(pos, size);
            }
        }
        
        /// <summary>
        /// Draw normalized mesh into texture
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="mesh"></param>
        /// <param name="clear"></param>
        public void Draw(Material mat, Mesh mesh, bool clear = true)
        {
            var prv = RenderTexture.active;
            var tSize = _rt.texelSize;
            // Maybe: scale mesh to fit the rect cell
            var pos = Rect.position * tSize;
            var scale = Rect.size * tSize;
            var localMatrix = Matrix4x4.TRS(pos, Quaternion.identity, scale);

            if (!_rt.IsCreated())
            {
                _rt.Create();
            }

            RenderTexture.active = _rt;
            mat.SetPass(0); // draw first pass only
            GL.PushMatrix();
            GL.LoadOrtho();

            // Clear the texture
            if (clear)
            {
                GL.Clear(true, true, Color.clear);
            }

            // Draw the mesh!
            Graphics.DrawMeshNow(mesh, localMatrix);

            // Pop the projection matrix to set it back to the previous one
            GL.PopMatrix();

            RenderTexture.active = prv;
        }
    }


    public class DynamicSpriteAtlas
    {
        public const int Padding = 4;

        public RenderTexture Texture;

        public DynamicSprite[] sprites;
        public DynamicSpriteAtlas(RenderTexture texture)
        {
            Texture = texture;
        }

        public void GenSprites(Vector2Int spriteSize)
        {
            var xx = spriteSize.x + Padding;
            var yy = spriteSize.y + Padding;

            // generate sprite
            var hCount = Texture.width / xx;
            var vCount = Texture.height / yy;
            var list = new List<DynamicSprite>();
            for (var i = 0; i < vCount; i++)
            {
                for (var j = 0; j < hCount; j++)
                {
                    var pos = new Vector2(xx * j, yy * i);
                    pos += Vector2.one * Padding * 0.5f;

                    var ds = new DynamicSprite(Texture, new Rect(pos, spriteSize));
                    list.Add(ds);
                }
            }

            sprites = list.ToArray();
        }

        public void Release()
        {
            Texture.Release();
        }
    }

    public static class RenderTextureExtension
    {
        public static void Clear(this RenderTexture rt)
        {
            if (!rt.IsCreated())
            {
                rt.Create();
            }
            
            var prv = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = prv;
        }
    }
}