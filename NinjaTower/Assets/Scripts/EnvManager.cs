using System;
using UnityEngine;

namespace NinjaTower
{
	public class EnvManager : MonoBehaviour
	{
		public static class Tag
		{
			public const string Castle = "Castle";
		}
		
		public Action<Enemy> EnemyEnterCastle;
		// public Action EnemyDestroy;
	}
}