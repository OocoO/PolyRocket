using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTower
{
	public class Path : MonoBehaviour
	{
		public Transform m_StartPos;
		public Transform m_EndPos;

		private Vector2 _start;
		private Vector2 _end;

		private void Start()
		{
			_start = m_StartPos.position;
			_end = m_EndPos.position;
		}

		public Vector2 GetPosition (float progress)
		{
			var pos = (_end - _start) * progress + _start;
			return pos;
		}
	}
}