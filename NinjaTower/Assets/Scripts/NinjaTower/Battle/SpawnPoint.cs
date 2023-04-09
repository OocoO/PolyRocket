using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower.Battle.Battle
{
	public class SpawnPoint : MonoBehaviour
	{
		public EnvManager m_Manager;
		public NtPath m_Path;
		public int m_Count;
		public int m_Interval;

		private float _countDown;
		private int _eCount;
		
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
			if (_eCount >= m_Count)
			{
				return;
			}
			
			var dt = Time.deltaTime;
			_countDown -= dt;
			if (_countDown <= 0f)
			{
				_countDown = m_Interval;
				m_Manager.CreateEnemy(m_Path, EnemyType.Red);
				_eCount++;
			}
		}
	}
}