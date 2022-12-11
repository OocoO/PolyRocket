using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower
{
	public class Enemy : PhysicsActor
	{
		public float m_Velocity;

		public Path m_Path;

		private float _position;
		
		protected override void Start()
		{
			SetPosition(0f);
		}

		private void FixedUpdate()
		{
			var dt = Time.fixedTime;
			var dx = m_Velocity * SpeedScaler * dt;
			var pos = _position + dx;
			SetPosition(pos);
		}

		private void SetPosition (float progress)
		{
			_position = progress;
			m_Rb.MovePosition(m_Path.GetPosition(_position));
		}


		private void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.CompareTag(EnvManager.Tag.Castle))
			{
				m_Manager.EnemyEnterCastle?.Invoke(this);
				Debug.Log("Enemy Enter Castle");
				Destroy(gameObject);
			}
		}
	}
}