using System;
using UnityEngine;

namespace NinjaTower.Battle
{
	// rb & collider
	[RequireComponent(typeof(Rigidbody2D))]
	public abstract class PhysicsActor : MonoBehaviour
	{
		public EnvManager m_Manager;
		public Rigidbody2D m_Rb;
		public Collider2D m_Collider;
		public Renderer m_Renderer;

		protected virtual void Start()
		{
		}
	}
}