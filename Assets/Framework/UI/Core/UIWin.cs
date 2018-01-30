using UnityEngine;
using UnityEngine.UI;


    /// <summary>
    /// window基类
    /// </summary>
	/// 
    public class UIWin : MonoBehaviour
    {
        private bool _log = false;
        [SerializeField] public winType winType;
        [SerializeField] private Button closeBtn;

        #region var

        /// <summary>
        /// ui的初始数据
        /// </summary>
        protected object _context { get; private set; }

        private Transform _transform;
        public Transform transform
        {
            get
            {
                if (_transform == null)
                    _transform = gameObject.transform;
                return _transform;
            }
        }
        /// <summary>
        /// winName
        /// </summary>
        public string name { get { return gameObject.name; } }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool inited = false;

        #endregion


        #region public

        protected virtual void Awake()
        {
        }

        /// <summary>
        /// 打开ui
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cb"></param>
        public void Open(object context)
        {
            if(context!=null)
                _context = context;
            gameObject.SetActive(true);

            _AddUIEvent();
            OnOpened();
        }

        /// <summary>
        /// 关闭ui
        /// </summary>
        public void Close()
        {
            _RemoveUIEvent();
            OnClosed();
            Destroy(gameObject);
        }

        /// <summary>
        /// ui关闭自己
        /// </summary>
        protected void CloseSelf()
        {
            UIManager.Instance.CloseWindow(name);
        }

        void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        #endregion

        #region not public

        private void _AddUIEvent()
        {
            if (closeBtn != null)
            {
                RegisterCloseSelf(closeBtn);
            }
            OnAddUIEvent();
        }

        private void _RemoveUIEvent()
        {
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveAllListeners();
            }
            OnRemoveUIEvent();
        }

        #endregion

        #region virtual (life time)
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInit()
        {
            LogError("onInit");
        }

        /// <summary>
        /// 界面打开完毕
        /// </summary>
        protected virtual void OnOpened()
        {
            LogError("OnOpened");
        }

        protected virtual void OnAddUIEvent()
        {
            
        }

        protected virtual void OnRemoveUIEvent()
        {
            
        }

        /// <summary>
        /// update
        /// mainui、fixedui 一直有效
        /// popui 只有当前界面才有效
        /// </summary>
        /// <param name="delta"></param>
        protected virtual void OnUpdate(float delta)
        {
        }

        protected virtual void OnClosed()
        {
            LogError("OnClosed");
        }

        protected virtual void OnCloseSelf()
        {
            CloseSelf();
        }

        #endregion

        #region button & event

        protected void RegisterCloseSelf(Button btn)
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnCloseSelf);
            }
        }

        #endregion

        #region log

        private void Log(string msg)
        {
            if (_log)
            {
                Debug.Log(string.Format("{0} : {1}", name, msg));
            }
        }
        private void LogWarning(string msg)
        {
            if (_log)
            {
                Debug.LogWarning(string.Format("{0} : {1}", name, msg));
            }
        }
        private void LogError(string msg)
        {
            if (_log)
            {
                Debug.LogError(string.Format("{0} : {1}", name, msg));
/*
                //record
                Dictionary<string, int> dic;
                if (funcCallDic.TryGetValue(name, out dic))
                {
                    if (dic.ContainsKey(msg))
                        dic[msg]++;
                    else
                        dic[msg] = 1;
                }
                else
                {
                    dic = new Dictionary<string, int>();
                    dic[msg] = 1;
                    funcCallDic.Add(name, dic);
                }
*/
            }
        }
/*
        private static Dictionary<string, Dictionary<string, int>> funcCallDic = new Dictionary<string, Dictionary<string, int>>();

        private void ShowCallRecord()
        {
            foreach (var kv in funcCallDic)
            {
                Debug.Log(string.Format("--------------------{0}-----------------", kv.Key));
                foreach (var kv2 in kv.Value)
                {
                    Debug.Log(string.Format("{0}     {1}", kv2.Key, kv2.Value));
                }
            }
        }
*/
        #endregion
    }
