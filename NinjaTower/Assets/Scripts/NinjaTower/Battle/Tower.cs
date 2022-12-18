using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower.Battle
{
	public class Tower : MonoBehaviour
	{
		public GameObject m_BulletTemplate;
		public float m_Interval;

		public float m_Range;
		public EnvManager m_Manager; // temp use

		private float _coolDown;
		private bool _isReady;

		private EnvManager _manager;

		void Start()
		{
			_manager = m_Manager;
			_isReady = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (_isReady)
			{
				// fire
				var enemies = _manager.GetAllEnemies();
				enemies.Sort((a, b) => a.Distance - b.Distance >= 0f ? -1 : 1);
				foreach (var enemy in enemies)
				{
					var dis = (enemy.m_Rb.position - (Vector2)transform.position).magnitude;
					if (dis <= m_Range)
					{
						// fie to e
						var go = Instantiate(m_BulletTemplate, transform);
						var bullet = go.GetComponent<Bullet>();
						go.SetActive(true);
						bullet.Init(enemy);
						_isReady = false;
						break;
					}
				}
			} 
			else
			{
				_coolDown -= Time.deltaTime;
				if (_coolDown <= 0f)
				{
					_coolDown = m_Interval;
					_isReady = true;
				}
			}
		}
	}
}