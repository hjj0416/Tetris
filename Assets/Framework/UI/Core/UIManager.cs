using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public enum winType
{
    FullScreen,
    Popup,
    Tip,
}

public class UIManager : MonoBehaviour, EventDispatcherProxy
    {
        private static string uiRootResPath = "Prefab/UIRoot";

        public const int PERLOAD_WINDOW_CACHE_TIME = 120;

        public Camera uiCamera;

        public Transform fullScreenLayer;
        public Transform popupLayer;
        public Transform tipsLayer;
        public EventSystem defaultEvtSys;
        public UIPoolManager poolMgr;
        public UITip uiTip;

    #region 单例
        private static UIManager mInstance;
        /// <summary>
        /// 获取资源加载实例
        /// </summary>
        /// <returns></returns>
        public static UIManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = GameObject.Find("UIRoot");
                    if (go == null)
                    {
                        GameObject prefab = Resources.Load<GameObject>(uiRootResPath) as GameObject;
                        go = Instantiate(prefab);
                        Transform rootTf = go.transform;
                        rootTf.SetParent(null);
                    }
                    mInstance = go.GetComponent<UIManager>();
                    mInstance.OnInit();
                    DontDestroyOnLoad(go);
                }
                return mInstance;
            }
        }

        #endregion

        #region Static
        public static string GetParameterDictionary(string url, out Dictionary<string, string> parameterDic)
        {
            parameterDic = null;

            var splitedString = url.Split('?');
            var winName = splitedString[0];

            if (splitedString.Length > 1)
            {
                var parameters = splitedString[1].Split('&');
                if (parameters.Length > 0)
                {
                    parameterDic = new Dictionary<string, string>();

                    foreach (var parameter in parameters)
                    {
                        if (string.IsNullOrEmpty(parameter))
                            continue;

                        var temp = parameter.Split('=');
                        if (temp.Length == 2)
                        {
                            if (parameterDic.ContainsKey(temp[0]))
                            {
                                parameterDic[temp[0]] = temp[1];
                            }
                            else
                            {
                                parameterDic.Add(temp[0], temp[1]);
                            }
                        }
                        else
                        {
                            Debug.LogError("UIManager GetParameterDictionary 解析参数失败 (" + url + ")");
                            break;
                        }
                    }
                }
            }

            return winName;
        }

        public static string GetWinNameByUrl(string url)
        {
            var splitedString = url.Split('?');

            if (splitedString.Length >= 1)
            {
                return splitedString[0];
            }

            return string.Empty;
        }

        #endregion

        #region var

        public Camera UICamera { get { return uiCamera; } }
        public Transform UIFullScreenLayer { get { return fullScreenLayer; } }
        public Transform UIPopupLayer { get { return popupLayer; } }
        public Transform TipLayer { get { return tipsLayer; } }

        //ui对象池管理
        private UIPoolManager _poolMgr {
            get { return poolMgr; }
        }
        //是否需要资源释放
        private bool _needUnloadUnused = false;
        //资源释放倒计时
        private float _unloadCountDown;

        EventDispatcher _dispatcher;
        public EventDispatcher Dispatcher
        {
            get { return _dispatcher == null ? (_dispatcher = new EventDispatcher()) : _dispatcher; }
        }

        #endregion

    #region init

        public void Init()
        {
        }

        protected void OnInit()
        {
            Input.multiTouchEnabled = false;
			if (!Application.isEditor) {
				uiCamera.eventMask = 0;
			}

        }


        public void Dispose()
        {
        }

        #endregion

        #region view pool

        public void Perload(string poolName, string prefabPath, int count = 1)
        {
            _poolMgr.Perload(poolName, prefabPath, count);
        }

        public void Perload(string poolName, GameObject prefab, int count = 1)
        {
            _poolMgr.Perload(poolName, prefab, count);
        }

        public Transform Spawn(string poolName, string prefabPath)
        {
            return _poolMgr.Spawn(poolName, prefabPath);
        }

        public Transform Spawn(string poolName, GameObject prefab)
        {
            return _poolMgr.Spawn(poolName, prefab);
        }

        public void UnSpawn(string poolName, Transform tf)
        {
            _poolMgr.UnSpawn(poolName, tf);
        }

        public void Clear(string poolName)
        {
            _poolMgr.Clear(poolName);
        }

        public void ClearAllPool()
        {
            _poolMgr.ClearAll();
        }

        #endregion

        #region api

        public void ShowTips(string text)
        {
            uiTip.ShowTip(text);
        }

        public void Alert(string title, string msg, AlertMode mode, string okBtnContent, string cancelBtnContent, Action<AlertResult> cb=null)
        {
            UIAlertBoxContext context = new UIAlertBoxContext(title, msg, mode, okBtnContent, cancelBtnContent);
            Instance.ShowWindow(WindowID.UIAlertBox, context, (w) =>
            {
                UIAlertBox win = w as UIAlertBox;
                win.OnCommit = cb;
            });
        }

        public void Alert(string title, string msg, AlertMode mode, Action<AlertResult> cb = null)
        {
            UIAlertBoxContext context = new UIAlertBoxContext(title, msg, mode);
            Instance.ShowWindow(WindowID.UIAlertBox, context, (w) =>
            {
                UIAlertBox win = w as UIAlertBox;
                win.OnCommit = cb;
            });
        }

        public void Alert(string msg, AlertMode mode, Action<AlertResult> cb = null)
        {
            UIAlertBoxContext context = new UIAlertBoxContext("", msg, mode);
            Instance.ShowWindow(WindowID.UIAlertBox, context, (w) =>
            {
                if (w == null)
                {
                    return;
                }
                UIAlertBox win = w as UIAlertBox;
                win.OnCommit = cb;
            });
        }

        #endregion

        #region public

        /// <summary>
        /// 正在打开或者已经打开了UI
        /// </summary>
        /// <param name="winName"></param>
        /// <returns></returns>
        public bool IsOpen(string winName, bool includeOpining = true)
        {
            //IUICtrl ctrl = GetUICtrl(winName);
            //if (ctrl != null)
            //    return ctrl.IsOpen(winName, includeOpining);
            return false;
        }

        /// <summary>
        /// 打开window
        /// </summary>
        //public void OpenWin(string winName, UIContext context = null, Action<UIWin> cb = null)
        public void ShowWindow(string winName, object data = null, Action<UIWin> cb = null)
        {
            if (string.IsNullOrEmpty(winName))
            {
                Debug.Log("window name is null");
                return;
            }
            LoadUI(winName, data, cb);
        }

        /// <summary>
        /// 关闭window
        /// </summary>
        /// <param name="winName"></param>
        /// <param name="cb"></param>
        public void CloseWindow(string winName)
        {
            if (string.IsNullOrEmpty(winName))
            {
                Debug.Log("window name is null");
                return;
            }
            GameObject go = FindWindow(winName);
            if (go != null)
            {
                UIWin win = go.GetComponent<UIWin>();
                if(win!=null)
                    win.Close();
            }
        }

        /// <summary>
        /// 关闭所有类型ui
        /// </summary>
        public void CloseAll()
        {
            //SendEvent(UIEventType.CLOSE_ALL);
            //int count = UICtrls.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    UICtrls[i].CloseAll();
            //}
        }

        /// <summary>
        /// 主动请求释放无用资源
        /// </summary>
        public void UnloadUnused()
        {
            _needUnloadUnused = true;
            _unloadCountDown = 0.5f;
        }

        /// <summary>
        /// 加载prefab，创建uiwin，绑定gameobject、config
        /// </summary>
        /// <param name="winName"></param>
        /// <param name="cb"></param>
        public void LoadUI(string winName, object data = null, Action<UIWin> cb = null)
        {
            string prefabPath = string.Format("{0}/{1}", PathDefine.DIR_UI_PREFAB, winName);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("prefab is null : "+prefabPath);
                if (cb != null) cb(null);
                return;
            }
            GameObject uiObject = Instantiate(prefab);
            uiObject.name = winName;
            UIWin win = uiObject.GetComponent<UIWin>();
            if (win == null)
            {
                Debug.LogError("prefab should add UIWin script");
                if (cb != null) cb(null);
                return;
            }
            Transform winTrans = uiObject.transform;
            uiObject.SetActive(true);
            Transform parent = GetParent(win);
            if (parent == null)
            {
                Debug.LogError("parent is null");
                if (cb != null) cb(null);
                return;
            }
            winTrans.SetParent(parent);
            RectTransform rect = winTrans as RectTransform;
            if (rect != null)
            {
                if (rect.anchorMin == rect.anchorMax)
                {
                    rect.anchoredPosition = Vector2.zero;
                }
                else
                {
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                rect.SetAsLastSibling();
            }
            win.Open(data);
            if(cb!=null)cb(win);
        }

    Transform GetParent(UIWin win)
    {
        if (win == null) return null;
        switch (win.winType)
        {
            case winType.FullScreen:return fullScreenLayer;
            case winType.Popup:return fullScreenLayer;
            case winType.Tip:return fullScreenLayer;
        }
        return null;
    }

    GameObject FindWindow(string winName)
    {
        GameObject go = FindWindow(UIFullScreenLayer, winName);
        if (go != null) return go;
        go = FindWindow(UIPopupLayer, winName);
        if (go != null) return go;
        go = FindWindow(TipLayer, winName);
        if (go != null) return go;
        return null;
    }

    GameObject FindWindow(Transform parent, string winName)
    {
        int len = parent.childCount;
        for (int i = 0; i < len; i++)
        {
            var child = parent.GetChild(i);
            if (child.name.Equals(winName))
                return child.gameObject;
        }
        return null;
    }

        #endregion

        #region update

        void Update()
        {
        }

        void LateUpdate()
        {
            if (_needUnloadUnused)
            {
                _unloadCountDown -= Time.deltaTime;
                if (_unloadCountDown <= 0)
                {
                    _needUnloadUnused = false;
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    //UIManager.Instance.ShowTips("clear ");
                }
            }
        }

        private void OnQuitHandle(AlertResult result)
        {
            if (result == AlertResult.OK)
                Application.Quit();
        }
        #endregion

        #region event

        public static void SendEvent(short type)
        {
            Instance.Dispatcher.SendEvent(type);
        }

        public static void SendEvent<T>(short type, T msg)
        {
            Instance.Dispatcher.SendEvent<T>(type, msg);
        }

        public static void AddEventHandler(short type, EventHandler handler)
        {
            Instance.Dispatcher.AddEventHandler(type, handler);
        }

        public static void AddEventHandler<T>(short type, EventHandler<T> handler)
        {
            Instance.Dispatcher.AddEventHandler<T>(type, handler);
        }

        public static void AddOneShotEventHandler(short type, EventHandler handler)
        {
            Instance.Dispatcher.AddOneShotEventHandler(type, handler);
        }

        public static void AddOneShotEventHandler<T>(short type, EventHandler<T> handler)
        {
            Instance.Dispatcher.AddOneShotEventHandler<T>(type, handler);
        }

        public static void RemoveEventHandler(short type, EventHandler handler)
        {
            Instance.Dispatcher.RemoveEventHandler(type, handler);
        }

        public static void RemoveEventHandler<T>(short type, EventHandler<T> handler)
        {
            Instance.Dispatcher.RemoveEventHandler<T>(type, handler);
        }

        #endregion


    }