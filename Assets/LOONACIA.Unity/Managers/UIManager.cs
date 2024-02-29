using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace LOONACIA.Unity.Managers
{
    public class UIManager
    {
        private readonly Stack<UIPopup> _popupStack = new();

        private Transform _root;

        private UIScene _sceneUI;

        private int _order = 0;

        private Transform Root
        {
            get
            {
                Init();
                return _root;
            }
        }

        public void Init()
        {
            if (_root == null)
            {
                _root = new GameObject { name = "@UI_Root" }.transform;
            }
        }

        public int GetOrder()
        {
            return _order;
        }

        public void SetCanvas(GameObject gameObject, bool sort = true)
        {
            Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = sort ? ++_order : 0;

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        public void SetOverlayCanvas(GameObject gameObject)
        {
            Canvas overlayCanvas = gameObject.GetOrAddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.WorldSpace;
        }
        public void SetScreenSpaceCameraCanvas(GameObject gameObject)
        {
            Canvas overlayCanvas = gameObject.GetOrAddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        }

        public T ShowSceneUI<T>(string name = null)
            where T : UIScene
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject gameObject = ManagerRoot.Resource.Instantiate($"UI/Scene/{name}",usePool: false);
            var sceneUI = gameObject.GetOrAddComponent<T>();
            _sceneUI = sceneUI;

            gameObject.transform.SetParent(Root.transform);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null, Transform paranet = null, bool usePool = false)
            where T : UIPopup
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject gameObject = ManagerRoot.Resource.Instantiate($"UI/Popup/{name}", usePool: usePool);
            T popup = gameObject.GetOrAddComponent<T>();
            _popupStack.Push(popup);

            if(paranet != null){
                gameObject.transform.SetParent(paranet);
            }else{
                gameObject.transform.SetParent(Root.transform);
            }

            return popup;
        }

        public void ClosePopupUI(UIPopup popup)
        {
            if (_popupStack.Count == 0)
            {
                return;
            }

            if (_popupStack.Peek() != popup)
            {
                Debug.Log($"Can't close popup: {popup.name}");
                return;
            }

            popup = _popupStack.Pop();

            if(popup != null){
                ManagerRoot.Resource.Release(popup.gameObject);
            }

            _order--;
        }

        public void ClosePopupUI()
        {
            if (_popupStack.TryPeek(out var popup))
            {
                ClosePopupUI(popup);
            }
        }

        public void ClearAllPopup()
        {
            while (_popupStack.Count > 0)
            {
                ClosePopupUI();
            }
        }
        
        public void Clear(bool destroyAssociatedObject)
        {
            ClearAllPopup();
            _sceneUI = null;

            if (destroyAssociatedObject)
            {
                Object.Destroy(Root);
            }
            System.Array.Fill(UIWinPopup.IsFamilyImage, false);
        }


        public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UIBase
        {
            if(string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject go = ManagerRoot.Resource.Instantiate($"UI/SubItem/{name}", usePool: false);

            if (parent != null)
            {
                go.transform.SetParent(parent);
            }

            return go.GetOrAddComponent<T>();
        }
    }
}