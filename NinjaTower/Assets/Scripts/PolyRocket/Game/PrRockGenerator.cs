using System;
using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace PolyRocket.Game
{
    public class PrRocket
    {
        public GameObject Go;

        private float _timer;
        
        public bool m_isDead;

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > 5f)
            {
                m_isDead = true;
            }
        }
        
    }
    public class PrRockGenerator
    {
        private readonly LinkedList<PrRocket> _rockets = new LinkedList<PrRocket>();

        private readonly GameObject _prefab;

        private readonly Transform _parent;

        private readonly float _interval;

        private float _timer;
        private Random _random;
        private Camera _camera;

        public PrRockGenerator(GameObject prefab, Transform parent, Camera camera, float interval)
        {
            _prefab = prefab;
            _parent = parent;
            _interval = interval;
            _camera = camera;

            _random = new Random(7);
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                var rocket = Generate();
                _rockets.AddLast(rocket);
            }

            var prt = _rockets.First;
            while (prt != null)
            {
                if (prt.Value.m_isDead)
                {
                    var next = prt.Next;
                    Object.Destroy(prt.Value.Go);
                    _rockets.Remove(prt);
                    prt = next;
                }
                else
                {
                    prt.Value.Update();
                    prt = prt.Next;
                }
            }
        }

        private PrRocket Generate()
        {
            var mainCam = _camera;
            var x = _random.Next(-100, 100) / 100f * mainCam.GetHalfWidth();
            var y = mainCam.transform.position.y + mainCam.orthographicSize + 10f;

            var go = Object.Instantiate(_prefab, _parent);
            go.transform.position = new Vector3(x, y);
            var rocket = new PrRocket()
            {
                Go = go,
            };
            return rocket;
        }
    }
}