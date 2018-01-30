using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TheNextMoba.UI
{
    public class UIScrollView : MonoBehaviour, ICellNotifier
    {

        [SerializeField] private bool defaultSelect = false;//是否默认选择第一个cell
        [SerializeField] private GameObject Template;
        [SerializeField] private GameObject leftPageFlag;
        [SerializeField] private GameObject rightPageFlag;
        [SerializeField] private GameObject upPageFlag;
        [SerializeField] private GameObject downPageFlag;
        private Transform _poolRoot;
        private Dictionary<GameObject, UISimplePool> _poolDic;
        private UISimplePool _curPool;
        private Vector2 contentResetPos = new Vector2(0, 1);
            /// <summary>
        /// cell的数量上限，默认无限
        /// </summary>
        [SerializeField] private int CellMaxCount = -1;

        private ScrollRect _scrollRect;
        protected ScrollRect scrollRect
        {
            get
            {
                if (_scrollRect == null)
                    _scrollRect = gameObject.GetComponent<ScrollRect>();
                return _scrollRect;
            }
        }

        private RectTransform _content;
        protected RectTransform content
        {
            get
            {
                if (_content == null)
                    _content = scrollRect.content;
                return _content;
            }
        }

        private RectTransform _viewPort;
        protected RectTransform viewPort
        {
            get
            {
                if (_viewPort == null)
                    _viewPort = scrollRect.viewport;
                return _viewPort;
            }
        }

        private List<UICell> _cells = new List<UICell>();

        void Awake()
        {
            if (defaultSelect)
                _curSelectIndex = 0;
            scrollRect.onValueChanged.AddListener(OnScrollRecValueChanged);
        }

        void Start()
        {
            if (Template != null)
            {
                Template.SetActive(false);
            }
        }
        private void Update()
        {
            if (pullDownFrameCount > 0)
            {
                pullDownFrameCount --;
                if (pullDownFrameCount <= 0)
                {
                    OnPullContentDown();
                }
            }
            if (revValueFrameCount > 0)
            {
                revValueFrameCount--;
                if (revValueFrameCount <= 0)
                {
                    OnScrollRecValueChanged(contentResetPos);
                }
            }
        }

        void OnDestroy()
        {
            Clear();
            clearPool();
            onCellEvent.RemoveAllListeners();
            scrollRect.onValueChanged.RemoveListener(OnScrollRecValueChanged);
            _scrollRect = null;
            _content = null;
        }

        private int revValueFrameCount;
        void ChangeRecValueToZero()
        {
            revValueFrameCount = 2;
        }
        void OnScrollRecValueChanged(Vector2 vec2)
        {
            if(upPageFlag != null || downPageFlag != null || leftPageFlag != null || rightPageFlag != null)
            {
                RefreshPageBtn(vec2);
            }
        }
        void RefreshPageBtn(Vector2 vec2)
        {
            bool showLeft = true;
            bool showRight = true;
            bool showUp = true;
            bool showDown = true;

            if (content.rect.height <= scrollRect.viewport.rect.height)
            {
                showUp = false;
                showDown = false;
            }
            else
            {
                if (vec2.y <= 0)
                {
                    showUp = true;
                    showDown= false;
                }
                else if (vec2.y >= 0.9f)
                {
                    showUp= false;
                    showDown = true;
                }
                else
                {
                    showUp= true;
                    showDown= true;
                }
            }

            if (content.rect.width <= scrollRect.viewport.rect.width)
            {
                showLeft = false;
                showRight = false;
            }
            else
            {
                if (vec2.x <= 0)
                {
                    showLeft = true;
                    showRight = false;
                }
                else if (vec2.x >= 1)
                {
                    showLeft = false;
                    showRight = true;
                }
                else
                {
                    showLeft = true;
                    showRight = true;
                }
            }

            if (leftPageFlag != null) { leftPageFlag.SetActive(showLeft); }
            if (rightPageFlag != null) { rightPageFlag.SetActive(showRight); }
            if (upPageFlag != null) { upPageFlag.SetActive(showUp); }
            if (downPageFlag != null) { downPageFlag.SetActive(showDown); }
        }
        void HidePageBtn()
        {
            if (leftPageFlag != null) { leftPageFlag.SetActive(false); }
            if (rightPageFlag != null) { rightPageFlag.SetActive(false); }
            if (upPageFlag != null) { upPageFlag.SetActive(false); }
            if (downPageFlag != null) { downPageFlag.SetActive(false); }
        }
        #region position/sort

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="comparer"></param>
        public void Sort(int index, int count, IComparer<UICell> comparer)
        {
            _cells.Sort(index, count, comparer);
            ResetPosition();
        }

        public void Sort(Comparison<UICell> comparer)
        {
            _cells.Sort(comparer);
            ResetPosition();
        }

        public void Sort(IComparer<UICell> comparer)
        {
            _cells.Sort(comparer);
            ResetPosition();
        }

        //排序
        private void ResetPosition()
        {
            int depth = 0;
            UICell cel;
            int count = _cells.Count;
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    cel = _cells[i];
                    cel.transform.SetAsFirstSibling();
                    depth = cel.transform.GetSiblingIndex();
                }
                else
                {
                    cel = _cells[i];
                    cel.transform.SetSiblingIndex(depth);
                }
                depth++;
            }
        }

        /// <summary>
        /// 指定一个 item让其定位到ScrollRect中间
        /// </summary>
        /// <param name="target">需要定位到的目标</param>
        public void CenterOnItem(RectTransform target, float tweenTime = 0)
        {
            StartCoroutine(CenterOn(target, tweenTime));
        }

        IEnumerator CenterOn(RectTransform target, float tweenTime)
        {
            yield return new WaitForSeconds(0.01f);
            // Item is here
            var itemCenterPositionInScroll = GetWorldPointInWidget(scrollRect.GetComponent<RectTransform>(), GetWidgetWorldPoint(target));
            Debug.Log("Item Anchor Pos In Scroll: " + itemCenterPositionInScroll);
            // But must be here
            var targetPositionInScroll = GetWorldPointInWidget(scrollRect.GetComponent<RectTransform>(), GetWidgetWorldPoint(scrollRect.viewport));
            Debug.Log("Target Anchor Pos In Scroll: " + targetPositionInScroll);
            // So it has to move this distance
            var difference = targetPositionInScroll - itemCenterPositionInScroll;
            difference.z = 0f;

            var newNormalizedPosition = new Vector2(difference.x / (content.rect.width - scrollRect.viewport.rect.width),
                difference.y / (content.rect.height - scrollRect.viewport.rect.height));

            newNormalizedPosition = scrollRect.normalizedPosition - newNormalizedPosition;

            newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
            newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);

            if (tweenTime > 0)
            {
                DOTween.To(() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, newNormalizedPosition, tweenTime);
            }
            else
            {
                scrollRect.normalizedPosition = newNormalizedPosition;
            }
        }

        /// <summary>
        /// 将list位置设置从头开始
        /// </summary>
        public void ReturnToStart()
        {
            scrollRect.content.localPosition = Vector3.zero;
            scrollRect.StopMovement();
        }

        #endregion

        #region prarent/child

        public void RefreshAll()
        {
            if (_cells.Count <= 0) return;
            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].Refresh();
            }
        }

        public Transform GetCellParent()
        {
            return scrollRect.content;
        }

        public int ChildCount
        {
            get
            {
                return scrollRect.content.childCount;
            }
        }

        public UICell GetCellByIndex(int index)
        {
            if (index < 0 || index >= _cells.Count)
                return null;
            return _cells[index];
        }

        public List<Transform> GetChildren()
        {
            int count = _cells.Count;
            List<Transform> ret = new List<Transform>(count);
            for (int i = 0; i < count; i++)
            {
                ret.Add(_cells[i].transform);
            }
            return ret;
        }

        /// <summary>
        /// 返回复制的list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAllCells<T>() where T : UICell
        {
            int count = _cells.Count;
            List<T> cells = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                cells.Add(_cells[i] as T);
            }
            return cells;
        }

        /// <summary>
        /// 返回cells自身对象，切勿对list进行增删操作
        /// </summary>
        /// <returns></returns>
        public List<UICell> GetAllCells()
        {
            return _cells;
        }

        public RectTransform GetChild(int index)
        {
            return scrollRect.content.GetChild(index) as RectTransform;
        }

        public T GetChild<T>(int index) where T : UICell
        {
            if (index >= 0 && index < _cells.Count)
            {
                return _cells[index] as T;
            }
            return null;
        }

        public void Foreach(Action<Transform> cb)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                cb(GetChild(i));
            }
        }

        public void Foreach<T>(Action<T> cb) where T : UICell
        {
            int count = _cells.Count;
            for (int i = 0; i < count; i++)
            {
                cb(_cells[i] as T);
            }
        }


        #endregion

        #region public add/remove item

        /// <summary>
        /// 设置新模板，将会清除当前item
        /// </summary>
        /// <param name="tmp"></param>
        public void SetTemplate(GameObject tmp)
        {
            if (tmp != null)
            {
                Template = tmp;
                if(_curPool == null)
                    initPool();
                Clear();
                createPool(tmp);
            }
        }

        /// <summary>
        /// 加入一个item， 要手动调用ResetPosition 排序
        /// </summary>
        public UICell AddItem(object context)
        {
            if (Template == null)
            {
                Debug.LogError("list template is null");
                return null;
            }
            if (_curPool == null)
                initPool();
            UICell cel = createItem(context, ChildCount + 1);
            if (CellMaxCount > 0 && ChildCount > CellMaxCount)
                RemoveFirst();
            _cells.Add(cel);
            return cel;
        }

        public void RemoveFirst()
        {
            Transform tf = GetChild(0);
            if (tf != null)
                RemoveCell(tf);
        }

        public void RemoveCell(Transform item)
        {
            if (item == null) return;
            UICell cell = item.GetComponent<UICell>();
            RemoveCell(cell);
        }

        public void RemoveCell(int index)
        {
            Transform tf = GetCellParent();
            RemoveCell(tf.GetChild(index));
        }

        public void RemoveCell(UICell cell)
        {
            _cells.Remove(cell);
            unSpawn(cell.transform);
        }

        public void Clear()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].Despawn();
                unSpawn(_cells[i].transform);
            }
            _cells.Clear();
        }

        public void SetData(object[] datas, bool returnToStart = true)
        {
            HidePageBtn();

            if (Template == null)
            {
                Debug.LogError("template is null");
                return;
            }
            if (_curPool == null)
                initPool();
            _curSelectIndex = defaultSelect ? 0 : -1;//set default select state
            if (returnToStart)
                ReturnToStart();

            PrepareCells(datas);
            ChangeRecValueToZero();
        }

        public void SetData<T>(List<T> datas, bool returnToStart = true)
        {
            HidePageBtn();

            if (Template == null)
            {
                Debug.LogError("template is null");
                return;
            }
            if (_curPool == null)
                initPool();
            _curSelectIndex = defaultSelect ? 0 : -1;//set default select state
            if (returnToStart)
                ReturnToStart();

            PrepareCells(datas);

            ChangeRecValueToZero();
        }

        private void PrepareCells(object[] datas)
        {
            int dataCount = datas.Length;
            int childCount = _cells.Count;
            if (dataCount == childCount)
            {
                int len = childCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
            }
            else if (dataCount > childCount)
            {
                int len = childCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
                for (int i = len; i < dataCount; i++)
                {
                    createItem(datas[i], i);
                }
            }
            else
            {
                int len = dataCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
                len = childCount - len;//remove count
                while (len > 0)
                {
                    int index = _cells.Count - 1;
                    var c = _cells[index];
                    c.Despawn();
                    unSpawn(c.transform);
                    _cells.RemoveAt(index);
                    len--;
                }
            }
        }

        private void PrepareCells<T>(List<T> datas)
        {
            int dataCount = datas.Count;
            int childCount = _cells.Count;
            if (dataCount == childCount)
            {
                int len = childCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
            }
            else if (dataCount > childCount)
            {
                int len = childCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
                for (int i = len; i < dataCount; i++)
                {
                    createItem(datas[i], i);
                }
            }
            else
            {
                int len = dataCount;
                for (int i = 0; i < len; i++)
                {
                    _cells[i].SetData(i, datas[i], _curSelectIndex == i);
                }
                len = childCount - len;//remove count
                while(len>0)
                {
                    int index = _cells.Count - 1;
                    var c = _cells[index];
                    c.Despawn();
                    unSpawn(c.transform);
                    _cells.RemoveAt(index);
                    len--;
                }
            }
        }

        protected virtual UICell createItem(object context, int i)
        {
            Transform tf = spawn();
            if (tf == null) return null;
            GameObject go = tf.gameObject;
            go.SetActive(true);
            tf.SetParent(content);
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            UICell t = go.GetComponent<UICell>();
            //go.name = i.ToString();
            t.Init(this);
            t.SetData(i, context, i == _curSelectIndex);
            _cells.Add(t);
            return t;
        }

        private void initPool()
        {
            _poolDic = new Dictionary<GameObject, UISimplePool>();
            GameObject poolGo = new GameObject();
            _poolRoot = poolGo.transform;
            _poolRoot.SetParent(transform);
            _poolRoot.localPosition = Vector3.zero;
            _poolRoot.localScale = Vector3.one;

            createPool(Template);
        }

        private void createPool(GameObject template)
        {
            if (template == null) return;
            if (_poolDic.ContainsKey(template))
            {
                _curPool = _poolDic[template];
                return;
            }
            //create
            UISimplePool pool = new UISimplePool("ScrollView", _poolRoot, template);
            _poolDic.Add(template, pool);
            _curPool = pool;
        }

        private Transform spawn()
        {
            if (_curPool == null)
            {
                Debug.LogError("cur pool is null");
                return null;
            }
            return _curPool.Spawn();
        }

        private void unSpawn(Transform tf)
        {
            if (_curPool == null)
            {
                Debug.LogError("cur pool is null");
                return;
            }
            _curPool.Despawn(tf);
        }

        private void clearPool()
        {
            
        }

        private float pullDownFrameCount = 0;
        public void PullContentDown()
        {
            pullDownFrameCount = 2;//do pull content down after 2 frame, waiting scroll view rendering to refresh content delta size
        }
        public void OnPullContentDown()
        {
            RectTransform rectTrans = _content.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(0, rectTrans.sizeDelta.y);
        }

        #endregion

        #region select state

        private int _curSelectIndex = -1;

        public void ClearSelectFlag()
        {
            _curSelectIndex = -1;
            gameObject.BroadcastMessage("OnBroadcastSelectIndex", _curSelectIndex, SendMessageOptions.DontRequireReceiver);
        }

        public void SetSelectIndex(int index)
        {
            _curSelectIndex = index;
            gameObject.BroadcastMessage("OnBroadcastSelectIndex", _curSelectIndex, SendMessageOptions.DontRequireReceiver);
        }

        #endregion

        #region cell event

        /// <summary>
        /// 监听cell中的事件
        /// 通过string区分是哪个对象产生事件
        /// </summary>
        public readonly CellEvent onCellEvent = new CellEvent();

        /// <summary>
        /// 由cell派发消息
        /// </summary>
        /// <param name="buttonId"></param>
        /// <param name="cell"></param>
        public void NotifyEvent(string eventName, UICell cell, bool flag = true)
        {
            if (eventName.Equals(UICell.ClickCellEvent))
            {
                _curSelectIndex = cell.index;
            }
            gameObject.BroadcastMessage("OnBroadcastSelectIndex", _curSelectIndex, SendMessageOptions.DontRequireReceiver);
            onCellEvent.Invoke(eventName, cell, flag);
        }

        #endregion

        #region utils

        private Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            //pivot position + item size has to be included
            var pivotOffset = new Vector3(
                (0.5f - target.pivot.x) * target.rect.size.x,
                (0.5f - target.pivot.y) * target.rect.size.y,
                0f);
            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }

        #endregion
    }
}
