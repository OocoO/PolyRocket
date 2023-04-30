using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Carotaa.Code
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private const int PanelInterval = 100;

        // [SerializeField] private Camera m_camera;
        [SerializeField] private Canvas m_rootCanvas;

        private LinkedList<PageBase> _pageStack;

        private void Awake()
        {
            _pageStack = new LinkedList<PageBase>();
        }


        public void Push<T>(params object[] pushParam) where T : PageBase
        {
            Push(typeof(T), pushParam);
        }

        public void Push(Type pageType, object[] pushParam)
        {
            if (ContainsPage(pageType))
            {
                throw new Exception($"Duplicate Page {pageType}");
            }

            var panelPath = GetPageAddress(pageType);
            var prefab = Resources.Load<GameObject>(panelPath);
            var panel = Object.Instantiate(prefab, m_rootCanvas.transform);
            var page = panel.GetComponent<PageBase>();
            _pageStack.AddLast(page);
            
            InvokePageFunction(page.OnPush, pushParam);
        }

        public void Pop<T>(params object[] popParam) where T : PageBase
        {
            Pop(typeof(T), popParam);
        }

        public void Pop(Type pageType, object[] popParam)
        {
            if (!ContainsPage(pageType))
            {
                throw new Exception($"Page not exist!");
            }

            var pageNode = FindPageNode(pageType);
            var page = pageNode.Value;
            InvokePageFunction(page.OnPop, popParam);
            
            Object.Destroy(page.gameObject);
            _pageStack.Remove(pageNode);
        }

        private bool ContainsPage(Type pageType)
        {
            return FindPageNode(pageType) != null;
        }

        private LinkedListNode<PageBase> FindPageNode(Type pageType)
        {
            return _pageStack.Find(x => x.GetType() == pageType);
        }

        private void InvokePageFunction(Action<object[]> action, object[] param)
        {
            try
            {
                action.Invoke(param);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static string GetPageAddress(Type pageType)
        {
            var address = pageType.GetCustomAttribute(typeof(PageAddressAttribute), false) as PageAddressAttribute;
            // ReSharper disable once PossibleNullReferenceException
            return address.Address;
        }
    }
}