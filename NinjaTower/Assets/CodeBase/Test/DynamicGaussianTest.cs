using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code.Test
{
	public class DynamicGaussianTest : MonoBehaviourAlt
	{
		public static readonly VertexHelper s_VertexHelper = new VertexHelper();
		private static readonly int TexSize = Shader.PropertyToID("_TexSize");
		
		[SerializeField] private DynamicImage m_BlurImage;
		[SerializeField] private RawImage m_OriginImage;
		[SerializeField] private Sprite m_SourceSprite;
		[SerializeField] private Material m_BlurMat;
		[SerializeField] private RenderTexture m_RT;

		private DynamicSpriteAtlas _atlas;
		private Mesh _mesh;

		private void Start()
		{
			_atlas = new DynamicSpriteAtlas(m_RT);
			_atlas.GenSprites(Vector2Int.one * 124);
			_mesh = new Mesh();
			
			m_BlurMat.SetVector(TexSize, m_SourceSprite.texture.texelSize);

			var vh = s_VertexHelper;
			vh.Clear();
			var color = (Color32) Color.white;
			vh.AddVert(new Vector3(0, 0), color, new Vector4(0, 0));
			vh.AddVert(new Vector3(0, 1), color, new Vector4(0, 1));
			vh.AddVert(new Vector3(1, 1), color, new Vector4(1, 1));
			vh.AddVert(new Vector3(1, 0), color, new Vector4(1, 0));
			
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(0, 2, 3);
			vh.FillMesh(_mesh);

			m_OriginImage.texture = m_SourceSprite.texture;
			DebugTriggerBlur();
		}

		public void DebugTriggerBlur()
		{
			_atlas.Texture.Clear();
			
			m_BlurMat.mainTexture = m_SourceSprite.texture;
			var count = _atlas.sprites.Length / 2;
			for (var i = 0; i < count; i++)
			{
				_atlas.sprites[i].Draw(m_BlurMat, _mesh, false);
			}
			
			m_BlurImage.Sprite = _atlas.sprites[0];
		}
	}
}