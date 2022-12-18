using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower.Battle
{
	public class Enemy : PhysicsActor
	{
		public float m_Speed;

		public Path m_Path;

		public EnemyType Type;

		private float _position;
		private int _hp;

		public float Distance => _position;

		public void Init(Path path, EnemyType type)
		{
			m_Path = path;
			Type = type;
			_hp = Type.MaxHp;
		}
		
		protected override void Start()
		{
			base.Start();
			SetPosition(0f);
		}

		private void FixedUpdate()
		{
			var dt = Time.fixedDeltaTime;
			var dx = m_Speed * dt / m_Path.GetLength();
			var pos = _position + dx;
			SetPosition(pos);
		}

		private void SetPosition (float progress)
		{
			_position = progress;
			m_Rb.MovePosition(m_Path.GetPosition(_position));
		}


		private void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag(EnvManager.Tag.Castle))
			{
				m_Manager.EnemyEnterCastle?.Invoke(this);
				Debug.Log("Enemy Enter Castle");
				m_Manager.DestroyEnemy(this);
			}
		}

		public void Damage (int hp)
		{
			_hp -= hp;
			if (_hp <= 0)
			{
				m_Manager.DestroyEnemy(this);
			}
		}
	}
}