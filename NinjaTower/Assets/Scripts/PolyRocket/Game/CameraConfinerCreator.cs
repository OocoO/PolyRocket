using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PolyRocket.Game
{
    [RequireComponent(typeof(PathCreator))]
    [RequireComponent(typeof(PolygonCollider2D))]
    [ExecuteAlways]
    public class CameraConfinerCreator: MonoBehaviour
    {
        private PathCreator _creator;
        private PolygonCollider2D _collider;
        public PolygonCollider2D PolygonCollider => _collider;

        private void Awake()
        {
            _creator = GetComponent<PathCreator>();
            _collider = GetComponent<PolygonCollider2D>();

            _creator.bezierPath.IsClosed = true;

            if (Application.isPlaying)
            {
                var path = _creator.path;
                var list = new List<Vector2>();
                foreach (var point in path.localPoints)
                {
                    list.Add((Vector2) point);
                }

                _collider.points = list.ToArray();
            }
        }
    }
}