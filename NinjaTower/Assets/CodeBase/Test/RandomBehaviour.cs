using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carotaa.Code.Test
{
	public class RandomBehaviour : MonoBehaviour
	{
		[SerializeField] public float someValue1;
		[SerializeField] public float m_SomeValue2;
		[SerializeField] private float m_SomeValue3;


		private float someValue2
		{
			get => m_SomeValue2;
			set => m_SomeValue2 = value;
		}
		
		
		private float SomeValue3
		{
			get => m_SomeValue3;
			set => m_SomeValue3 = value;
		}


		private float _someValue11;
		private float _someValue2;
		private float _someValue3;
	}
}