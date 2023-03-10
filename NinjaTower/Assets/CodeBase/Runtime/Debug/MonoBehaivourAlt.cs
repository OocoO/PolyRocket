using System;
using UnityEngine;

namespace Carotaa.Code
{
    public abstract class MonoBehaviourAlt : MonoBehaviour
    {
        // Child of this monoBehaviourAlt will get some button to trigger
        // rule : All public Function whose name Start with "DebugTrigger"
        // etc:
        // public void DebugTrigger()
        // public void DebugTriggerAlt()
        // public void DebugTrigger0(), DebugTrigger1(), DebugTrigger2()

        // event with replaceable name
        // [DebugButtonName("PopPopup All")]
        // public void DebugTriggerRandomName()
        // {
        //     MgrUI.instance.PopPopup(Int32.MaxValue);
        // }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DebugButtonNameAttribute : Attribute
    {
        public readonly string AltName;

        public DebugButtonNameAttribute(string altName)
        {
            AltName = altName;
        }
    }
}