using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Carotaa.Code
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private const int PanelInterval = 100;

        // [SerializeField] private Camera m_camera;
        [SerializeField] private Transform m_RootTrans;
        private bool _isQuit = false;

        private LinkedList<UIPage> _pageStack = new();

        private void OnApplicationQuit()
        {
            _isQuit = true;
        }

        public void Push<T>(params object[] pushParam) where T : UIPage
        {
            Push(typeof(T), pushParam);
        }

        public void Push(Type pageType, object[] pushParam)
        {
            if (_isQuit) return;

            if (ContainsPage(pageType)) throw new Exception($"Duplicate Page {pageType}");

            var panelPath = GetPageAddress(pageType);
            var prefab = Resources.Load<GameObject>(panelPath);
            var panel = Instantiate(prefab, m_RootTrans);
            var page = panel.GetComponent<UIPage>();
            _pageStack.AddLast(page);

            page.PushParam = pushParam;
            InvokePageFunction(page.OnPush);
        }

        public void Pop<T>() where T : UIPage
        {
            Pop(typeof(T));
        }

        public void Pop(UIPage page)
        {
            Pop(page.GetType());
        }

        public void Pop(Type pageType)
        {
            if (_isQuit) return;

            if (!ContainsPage(pageType)) throw new Exception($"Page not exist!");

            var pageNode = FindPageNode(pageType);
            var page = pageNode.Value;
            InvokePageFunction(page.OnPop);

            Destroy(page.gameObject);
            _pageStack.Remove(pageNode);
        }

        public T Find<T>() where T : UIPage
        {
            return FindPageNode(typeof(T)).Value as T;
        }

        private bool ContainsPage(Type pageType)
        {
            var pageNode = FindPageNode(pageType);
            return pageNode != null && pageNode.Value;
        }

        private LinkedListNode<UIPage> FindPageNode(Type pageType)
        {
            return _pageStack.Find(x => x.GetType() == pageType);
        }

        private static void InvokePageFunction(Action action)
        {
            try
            {
                action.Invoke();
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