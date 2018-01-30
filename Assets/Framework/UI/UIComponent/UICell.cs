using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheNextMoba.UI
{

	public class UICell : UIBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool immeRefresh;
//立即刷新
		[SerializeField] private float longPressDuration = 0.9f;
//长按响应时间
		[SerializeField] protected bool enableLongPress = false;
		//是否开启长按
		/// <summary>
		/// 消息：点击cell
		/// </summary>
		public const string ClickCellEvent = "OnClickCell";
		public const string LongClickCellEvent = "OnLongClickCell";

		/// <summary>
		/// 消息派发器
		/// </summary>
		protected ICellNotifier _notifier;

		/// <summary>
		/// cell的数据
		/// </summary>
		protected object _context { get; private set; }

		/// <summary>
		/// 是否需要刷新
		/// </summary>
		protected bool _needRefresh;

		public int index { get; private set; }

		public Transform m_cacheTransform { get; private set; }

		/// <summary>
		/// Button相关
		/// </summary>
		private bool _handled;
		private bool _pressed;
		private float _pressedTime;

		protected  override void Awake ()
		{
			m_cacheTransform = transform;
		}

		protected virtual void Update ()
		{
			// 长按
			if (!_pressed)
				return;

			if (Time.realtimeSinceStartup - _pressedTime >= longPressDuration) {
				_pressed = false;
				_handled = true;
				OnPointerLongClick ();
			}
		}

		protected virtual void LateUpdate ()
		{
			if (!_needRefresh)
				return;
			_needRefresh = false;
			OnRefresh ();
			SetSelectState (_isSelected);
		}

		private bool _isInit = false;

		public void Init (ICellNotifier notifier)
		{
			_notifier = notifier;
			if (_isInit)
				return;
			_isInit = true;
			OnInit ();
		}

		public bool HasInit ()
		{
			return _isInit;
		}

		public virtual void SetData (int index, object context, bool selected = false)
		{
			this.index = index;
			_isSelected = selected;
			_context = context;
			Refresh ();
			OnSpawn ();
			if (immeRefresh)
				LateUpdate ();
		}

		public void Refresh ()
		{
			if (_context != null)
				_needRefresh = true;
		}

		public void Despawn ()
		{
			OnDespawn ();
			//_isInit = false;
			_notifier = null;
			_context = null;
		    _isSelected = false;
		}

		/// <summary>
		/// 用于cell内容刷新，最新数据存储于_context
		/// </summary>
		protected virtual void OnRefresh ()
		{
		}

		/// <summary>
		/// 初始化，生命周期中只调用一次
		/// </summary>
		protected virtual void OnInit ()
		{
		}

		/// <summary>
		/// 从池中激活
		/// </summary>
		protected virtual void OnSpawn ()
		{
            //Debug.Log(index + " OnSpawn");
		}

		/// <summary>
		/// 回池
		/// </summary>
		protected virtual void OnDespawn ()
        {
            //Debug.Log(index + " OnDespawn");
		}



		#region select state

		private bool _isSelected;

		public bool IsSelected ()
		{
			return _isSelected;
		}

		void OnBroadcastSelectIndex (int index)
		{
			SetSelectState (index == this.index);
		}

		void SetSelectState (bool flag)
		{
			_isSelected = flag;
			OnSelected (_isSelected);
		}

		protected virtual void OnSelected (bool flag)
		{
            
		}

		#endregion


		#region cell event

		/// <summary>
		/// 注册cell中的按钮
		/// </summary>
		protected void RegisterButton (Button btn, string evnetName)
		{
			if (btn == null) {
				Debug.LogError ("button is null : " + evnetName);
				return;
			}
			//Debug.Log("register "+ btn.name + " " + gameObject.name);
			btn.onClick.RemoveAllListeners ();
			btn.onClick.AddListener (delegate {
				PostNotification (evnetName);
			});
		}

		protected void RegisterToggle (Toggle toggle, string evnetName)
		{
			if (toggle == null) {
				Debug.LogError ("toggle is null : " + evnetName);
				return;
			}
			toggle.onValueChanged.AddListener (delegate {
				PostNotification (evnetName, toggle.isOn);
			});
		}

		protected void PostNotification (string str, bool flag = true)
		{
			if (_notifier != null) {
				_notifier.NotifyEvent (str, this, flag);
			}
		}

		/// <summary>
		/// drivened by ugui
		/// </summary>
		/// <param name="eventData"></param>
		public virtual void OnPointerClick (PointerEventData eventData)
		{
			//Debug.Log("on click cell " + name);
			if (!_handled) {
				PostNotification (ClickCellEvent);
			}
		}

		public virtual void OnPointerExit (PointerEventData eventData)
		{
			if (enableLongPress) {
				_pressed = false;
			}
		}

		public virtual void OnPointerDown (PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (enableLongPress) {
				_pressed = true;
				_handled = false;
				_pressedTime = Time.realtimeSinceStartup;
			}
		}

		public virtual void OnPointerUp (PointerEventData eventData)
        {
			if (enableLongPress) {
				_pressed = false;
			}
		}

		public virtual void OnPointerLongClick ()
		{
			PostNotification (LongClickCellEvent);
		}

		#endregion
	}
}
