using System;
using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;

namespace PolyRocket.Game
{
    public class PrLauncher : MonoBehaviour
    {
        private void Awake()
        {
            PrGameManager.Instance.WakeUp();
            UIManager.Instance.WakeUp();
        }
    }
}