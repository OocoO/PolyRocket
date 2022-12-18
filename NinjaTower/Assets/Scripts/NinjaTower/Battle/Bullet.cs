using System;
using UnityEngine;

namespace NinjaTower.Battle
{
	public class Bullet : PhysicsActor
	{
		public float m_Speed;

		public int m_Damage;
		
		private Enemy _target;
		private Vector2 _lastTargetPos;
		public void Init (Enemy target)
		{
			_target = target;
		}


		private void FixedUpdate()
		{
			if (_target)
			{
				_lastTargetPos = _target.m_Rb.position;
			} 
			
			var dt = Time.fixedDeltaTime;
			var dx = dt * m_Speed;
			var position = m_Rb.position;

			var angle = _lastTargetPos - position;
			var nextPos = angle.normalized * dx + position;
				
			m_Rb.MovePosition(nextPos);

			if (angle.magnitude <= dx)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag(EnvManager.Tag.Enemy))
			{
				var enemy = other.gameObject.GetComponent<Enemy>();
				enemy.Damage(m_Damage);
				Destroy(gameObject);
			}
		}
	}
}