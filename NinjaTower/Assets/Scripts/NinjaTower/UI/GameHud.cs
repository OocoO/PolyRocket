using System;
using System.Collections;
using System.Collections.Generic;
using NinjaTower.Battle;
using TMPro;
using UnityEngine;

namespace NinjaTower.UI
{
	public class GameHud : MonoBehaviour
	{
		public TMP_Text m_PlayerHp;
		private EnvManager _manager;

		public void Init (EnvManager manager)
		{
			_manager = manager;
			OnHpChange(_manager.PlayerHp.Value);
			_manager.PlayerHp.Subscribe(OnHpChange);
		}

		private void OnHpChange (int value)
		{
			m_PlayerHp.text = $"HP: {value}";
		}
	}
}
