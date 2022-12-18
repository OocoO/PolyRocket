using System;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower.Battle
{
	public class EnvManager : MonoBehaviour
	{
		public static class Tag
		{
			public const string Castle = "Castle";
			public const string Enemy = "Enemy";
		}

		public Action<Enemy> EnemyEnterCastle;
		// public Action EnemyDestroy;

		public List<Enemy> _enemies;

		public GameObject m_Enemy;

		private void Awake()
		{
			_enemies = new List<Enemy>();
		}

		public Enemy CreateEnemy (Path path, EnemyType type)
		{
			var go = Instantiate(m_Enemy, transform);
			var enemy = go.GetComponent<Enemy>();
			go.SetActive(true);
			enemy.Init(path, type);

			_enemies.Add(enemy);

			return enemy;
		}

		public void DestroyEnemy (Enemy e)
		{
			_enemies.Remove(e);
			Destroy(e.gameObject);
		}

		public List<Enemy> GetAllEnemies()
		{
			return new List<Enemy>(_enemies);
		}
	}
}