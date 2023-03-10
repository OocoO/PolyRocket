using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Carotaa.Code;

namespace RLTest
{
    public class UGUIEnvironment : Singleton<UGUIEnvironment>, IEnvironment
    {
        private List<BaseRaycaster> s_Raycasters;

        public IState GetState()
        {
            throw new NotImplementedException();
        }

        public List<IAction> GetAction()
        {
            throw new NotImplementedException();
        }

        public void Update(IAction action)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            // get Raycaster Rigester
            var assembly = typeof(GraphicRaycaster).Assembly;
            var type = assembly.GetType("UnityEngine.EventSystems.RaycasterManager");
            var method = type.GetMethod("GetRaycasters", BindingFlags.Static);
            s_Raycasters = (List<BaseRaycaster>) method?.Invoke(null, null);

            if (s_Raycasters == null) throw new Exception("Unable to Get RaycasterManager.s_Raycasters");
        }
    }
}