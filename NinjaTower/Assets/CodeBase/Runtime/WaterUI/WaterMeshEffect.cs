using UnityEngine;
using UnityEngine.UI;

namespace Carotaa.Code
{
    public class WaterMeshEffect : RectMeshEffect
    {
        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                graphic.SetVerticesDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var count = vh.currentVertCount;
            var mat = GetDynamicShapeMatrix();
            UIVertex vert = UIVertex.simpleVert;
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.position = mat.MultiplyPoint3x4(vert.position);
                vh.SetUIVertex(vert, i);
            }
        }
        
        private Matrix4x4 GetDynamicShapeMatrix()
        {
            if (Velocity.magnitude < 1f)
            {
                return Matrix4x4.identity;
            }
            
            var vv = Velocity;
            var ss = vv.magnitude;

            var dd = (Sigmoid(ss * 0.01f) - 0.5f) * 0.3f;

            var scale = new Vector3(1 + dd, 1 - dd, 0f);
            var rotate = Quaternion.FromToRotation(vv, Vector3.right);
            var reverse = Quaternion.FromToRotation(Vector3.right, vv);
            var localToWorld = rectTransform.localToWorldMatrix;
            var rotM1 = Matrix4x4.Rotate(rotate);
            var scaleMat = Matrix4x4.Scale(scale);
            var rotM2 = Matrix4x4.Rotate(reverse);
            var worldToLocal = rectTransform.worldToLocalMatrix;

            return worldToLocal * rotM2 * scaleMat * rotM1 * localToWorld;
        }
        
        private static float Sigmoid(float value) {
            return 1.0f / (1.0f + Mathf.Exp(-value));
        }
    }
}