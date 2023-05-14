using Carotaa.Code;
using PolyRocket.Game.Actor;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLevelCamera : MonoBehaviour
    {
        public Transform m_DeadTran;
        public Transform m_SkyTran;
        
        public SpriteRenderer m_SkyRenderer;
        private Material _skyMat;
        private int _camPosIndex;
        private Camera _levelCamera;

        public void Start()
        {
            var level = GetComponentInParent<PrLevel>();
            var aspect = level.m_LevelCamera.aspect;
            var maxHeight = PrLevel.MaxCameraSize * 2f;
            var maxWidth = maxHeight * aspect;
            _levelCamera = level.m_LevelCamera;
            m_SkyTran.localScale = new Vector3(maxWidth, maxHeight, 1f);

            OnCameraSizeChange(_levelCamera.orthographicSize);
            level.ECameraSizeChange.Subscribe(OnCameraSizeChange);

            MaterialUtility.RebindMaterial(m_SkyRenderer);
            _skyMat = m_SkyRenderer.material;
            _camPosIndex = Shader.PropertyToID("_CamPos");
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            var actor = other.GetComponent<PrActor>();
            if (actor && actor is not PrPlayer)
                // TODO: recycle
                Destroy(actor.gameObject);
        }

        private void OnCameraSizeChange(float halfSize)
        {
            m_DeadTran.localPosition = Vector3.down * halfSize;
        }

        private void Update()
        {
            _skyMat.SetVector(_camPosIndex, _levelCamera.transform.position);
        }
    }
}