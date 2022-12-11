using UnityEngine;

namespace NinjaTower
{
	// rb & collider
	[RequireComponent(typeof(Rigidbody2D))]
	public abstract class PhysicsActor : MonoBehaviour
	{
		public const float SpeedScaler = 0.001f;

		public EnvManager m_Manager;
		public Rigidbody2D m_Rb;
		public Collider2D m_Collider;
		public Renderer m_Renderer;

		protected virtual void Start()
		{
		}
	}
}