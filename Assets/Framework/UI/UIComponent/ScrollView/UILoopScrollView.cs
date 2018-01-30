using System;
using System.Collections.Generic;
using TheNextMoba.UI;
using UnityEngine;
using UnityEngine.UI;

public class UILoopScrollView : MonoBehaviour, ICellNotifier
{
    [SerializeField] private bool defaultSelect = false;//是否默认选择第一个cell
    [Tooltip("是否是用于聊天")]
    [SerializeField] private bool chatMode = false;
    [SerializeField] private int dataLimit;
    [SerializeField] private bool needScrollDetail;
    [SerializeField] private GameObject leftBtn;
    [SerializeField] private GameObject rightBtn;
    [SerializeField] private int tweenSpeed = 2000;

    private bool buttonValid = false;

    private LoopScrollRect _scrollRect;
    public LoopScrollRect scrollRect
    {
        get
        {
            if (_scrollRect == null)
                _scrollRect = gameObject.GetComponent<LoopScrollRect>();
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

    public bool reverseDirection {
        get { return scrollRect.reverseDirection; }
        set { scrollRect.reverseDirection = value; }
    }

    public LoopScrollRect.ScrollRectEvent onValueChanged { get { return scrollRect.onValueChanged; }  }
    public Action<int> onNewMessageChanged;
    private int _newMsgCount = 0;

    private int newMsgCount
    {
        get{return _newMsgCount;}
        set
        {
            if (_newMsgCount == value) return;
            _newMsgCount = value;
            if (onNewMessageChanged != null)
                onNewMessageChanged(_newMsgCount);
            Debug.Log("--------  未读消息："+_newMsgCount);
        }
    }

    private bool _isHorizontal;

    private int _curSelectIndex = -1;

    /// <summary>
    /// 列表数据
    /// </summary>
	protected List<object> _data;

    private bool _interactable = true;
    public bool interactable
    {
        get { return _interactable; }
        set
        {
            _interactable = value;
            if (scrollRect != null)
            {
                if (_isHorizontal)
                    scrollRect.horizontal = value;
                else
                    scrollRect.vertical = value;
            }
        }
    }

    void Awake()
    {
        if (leftBtn != null && rightBtn != null)
        {
            buttonValid = true;

            leftBtn.SetActive(false);
            rightBtn.SetActive(false);

        }
        if (scrollRect)
        {
            scrollRect.totalCount = 0;
            scrollRect.OnRefreshItem.AddListener(OnRefreshCell);
            if (needScrollDetail)
            {
                scrollRect.onValueChanged.AddListener(OnScrollChanged);
            }
            if (scrollRect is LoopHorizontalScrollRect)
                _isHorizontal = true;
            else
                _isHorizontal = false;
        }
        if (defaultSelect)
            _curSelectIndex = 0;
    }


    void OnDestroy()
    {
        if (scrollRect)
        {
            scrollRect.OnBeforeClose();
        }
        onCellEvent.RemoveAllListeners();
        if (scrollRect)
        {
            scrollRect.OnRefreshItem.RemoveListener(OnRefreshCell);
            scrollRect.OnDestroy();
        }
    }

    protected virtual void OnSetData()
    {
        
    }

    public void RefreshAllCells()
    {
        int count = content.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform tf = content.GetChild(i);
            UICell cell = tf.GetComponent<UICell>();
            if(cell!=null)cell.Refresh();
        }
    }

    public void SetSelectDefault(int index = 0)
    {
        _curSelectIndex = index;
    }

    #region public add/remove item


    public List<UICell> GetAllCells()
    {
        int count = content.childCount;
        List<UICell>cells = new List<UICell>(count);
        for (int i = 0; i < count; i++)
        {
            Transform tf = content.GetChild(i);
            cells.Add(tf.GetComponent<UICell>());
        }
        return cells;
    }

    public void SetTemplate(GameObject tmp)
    {
        if (scrollRect != null)
        {
            scrollRect.SetTemplate(tmp);
        }
    }

    public void SetData<T>(List<T> datas, bool returnToStart = true)
    {
        RefreshData(datas, returnToStart);
    }

    /// <summary>
    /// 刷新数据，如果list没有item，则填充
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="datas"></param>
    public void RefreshData<T>(List<T> datas, bool returnToStart = true)
    {
        //Debug.LogError("--------- RefreshData  " + datas.Count + "  " + returnToStart);
        if (datas == null || datas.Count <= 0)
        {
            scrollRect.totalCount = 0;
            scrollRect.ClearCells();
            return;
        }
        if (scrollRect!=null)
        {
            if (defaultSelect && _curSelectIndex < 0)
                _curSelectIndex = 0;//set default select state
            scrollRect.StopMovement();
            scrollRect.totalCount = datas.Count;
            scrollRect.Init();
            _data = ConvertToList(datas);
            if (chatMode)
            {
                scrollRect.RefillCellsFromEnd();
            }
            else
            {
                if (returnToStart)
                {
                    scrollRect.RefillCells();
                }
                else
                {
                    scrollRect.RefillCellsAndStay();
                }
            }
            
            OnSetData();
        }

        CheckBtnStatus();
    }

    public void RefillData(int index)
    {
        if (scrollRect!=null)
            scrollRect.RefillCells(index);
    }

    public void ScrollToCell(int index)
    {
        if (scrollRect != null)
            scrollRect.SrollToCell(index, tweenSpeed);
    }

    public void AddItem<T>(T t, bool toEnd = false)
    {
        if (_data == null)
            _data = new List<object>();
        scrollRect.Init();
        _data.Add(t);
        scrollRect.totalCount++;
        if (chatMode)
        {
            if (toEnd || scrollRect.IsBottom())
                scrollRect.RefillCellsFromEnd();
            else if (!scrollRect.IsFull())
                scrollRect.RefillCells();
            else
                newMsgCount++;
        }
        else
        {
            scrollRect.RefillCells();
        }
    }

    //public void AddItems<T>(List<T> adds, bool scrollToTop = false)
    //{
    //    if (_data == null)
    //        _data = new List<object>();
    //    int len = adds.Count;
    //    for (int i = 0; i < len; i++)
    //    {
    //        _data.Add(adds[i]);
    //    }
    //    scrollRect.Init();
    //    if (scrollToTop)
    //    {
    //        if (chatMode && scrollRect.IsBottom())
    //        {
    //            scrollRect.totalCount = _data.Count;
    //            scrollRect.AppendCells(len);
    //        }
    //        else
    //        {
    //            scrollRect.totalCount = _data.Count;
    //            scrollRect.RefreshCells();
    //        }
    //    }
    //    else
    //    {
    //        //静默更新数据
    //        scrollRect.totalCount = _data.Count;
    //    }
    //}

    public void Clear()
    {
        if (_data != null)
            _data.Clear();
        scrollRect.totalCount = 0;
        scrollRect.RefreshCells();
    }

    /// <summary>
    /// callback by LoopScrollRect
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tf"></param>
    private void OnRefreshCell(int index, Transform tf)
    {
        if (_data == null || _data.Count < 0)
        {
            //Debug.LogError("loopScrollView data is empty");
            return;
        }
        if (index < 0 || index > _data.Count - 1)
        {
            Debug.LogError("cell index is overflow " + index);
            return;
        }
        UICell cel = tf.GetComponent<UICell>();
        if (cel)
        {
            cel.Init(this);
            cel.SetData(index, _data[index], _curSelectIndex == index);
        }
    }


    private List<object> ConvertToList<T>(List<T> lst)
    {
		int len = lst.Count;
        List<object> ret = new List<object>(len);
        for (int i = 0; i < len; i++)
            ret.Add(lst[i]);
        return ret;
    }

    private void OnScrollChanged(Vector2 p)
    {
        if (_scrollRect.ContentOverflow)
        {
            if (_isHorizontal)
            {
                if (buttonValid)
                {
                    leftBtn.SetActive(p.x>0.1f);
                    rightBtn.SetActive(p.x < 0.9f);
                }
                if (p.x >= 1)
                    newMsgCount = 0;
            }
            else
            {
                if (buttonValid)
                {
                    leftBtn.SetActive(p.y > 0.1f);
                    rightBtn.SetActive(p.y < 0.9f);
                }
                if (p.y >= 1)
                    newMsgCount = 0;
            }
        }
        else
        {
            if (buttonValid)
            {
                leftBtn.SetActive(false);
                rightBtn.SetActive(false);
            }
        }
    }

    public void CheckBtnStatus()
    {
        if (!buttonValid) return;

        if (scrollRect.ContentOverflow)
        {
            leftBtn.SetActive(false);
            rightBtn.SetActive(true);
        }
        else
        {
            leftBtn.SetActive(false);
            rightBtn.SetActive(false);
        }
    }

    //public void SetNormalizedPosition(float value)
    //{
    //    scrollRect.SetNormalizedPosition(value);
    //}

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

  
}
