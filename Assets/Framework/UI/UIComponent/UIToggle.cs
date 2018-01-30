using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
	[DisallowMultipleComponent, AddComponentMenu("UI/UIToggle", 58)]
	public class UIToggle : Toggle {

        [SerializeField]public bool m_IsTabSpriteNativeSize = false;

		public enum TextTransition
		{
			None,
			ColorTint
		}
		
        [SerializeField] private List<GameObject> m_Activate;
        [SerializeField] private List<GameObject> m_Deactivate;
		[SerializeField] private GameObject m_TargetContent;
	    [SerializeField] private GameObject m_targetPrefab;//支持动态创建prefab并赋值给content
	    [SerializeField] private Transform m_contentParent;

        [SerializeField] private Image m_ImageTarget;
		[SerializeField] private Transition m_ImageTransition = Transition.None;
		[SerializeField] private ToggleColorBlock m_ImageColors = ToggleColorBlock.defaultColorBlock;
		[SerializeField] public ToggleSpriteState m_ImageSpriteState;
		
		[SerializeField] private Text m_TextTarget;
		[SerializeField] private TextTransition m_TextTransition = TextTransition.None;
		[SerializeField] private ToggleColorBlock m_TextColors = ToggleColorBlock.defaultColorBlock;
		
		private Selectable.SelectionState m_CurrentState = Selectable.SelectionState.Normal;

        public readonly UIToggleEvent onToggleEvent = new UIToggleEvent();
	    private bool _sendToggleEvent = true;
        //toggle position : from 0 to n
	    public int Index;

	    private bool _needRefresh = false;

	    protected override void Awake()
	    {
	        base.Awake();
            _sendToggleEvent = false;
            OnToggleStateChanged(isOn);
            _sendToggleEvent = true;
	    }

	    protected override void OnEnable()
	    {
	        base.OnEnable();
            this.onValueChanged.RemoveListener(OnToggleStateChanged);
            this.onValueChanged.AddListener(OnToggleStateChanged);
	        if (_needRefresh)
	        {
	            _needRefresh = false;
	            OnToggleStateChanged(isOn);
	        }
	    }

	    //protected override void OnEnable()
        //{
        //    base.OnEnable();

        //    transition = Transition.None;

        //    if (_needRefresh)
        //        OnToggleStateChanged(isOn);
        //    _needRefresh = false;
        //    // Hook an event listener
        //    //this.onValueChanged.RemoveListener(OnToggleStateChanged);
        //    //this.onValueChanged.AddListener(OnToggleStateChanged);
        //    //SetToggleEffect(isOn, true);
        //    // Apply initial state
        //    //this.InternalEvaluateAndTransitionState(true);
        //}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			
			this.m_ImageColors.fadeDuration = Mathf.Max(this.m_ImageColors.fadeDuration, 0f);
			this.m_TextColors.fadeDuration = Mathf.Max(this.m_TextColors.fadeDuration, 0f);
			
			if (this.isActiveAndEnabled)
			{
                SetToggleEffect(this.isOn);

                // Toggle the content
                if (this.m_TargetContent != null)
                    m_TargetContent.SetActive(this.isOn);

                //Debug.Log(gameObject.name + "   active gameobject  " + state);
                for (int i = 0; i < m_Activate.Count; ++i)
                    Set(m_Activate[i], this.isOn);
                for (int i = 0; i < m_Deactivate.Count; ++i)
                    Set(m_Deactivate[i], !this.isOn);

                this.InternalEvaluateAndTransitionState(true);
			}
		}
#endif

		/// <summary>
		/// Raises the toggle state changed event.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		protected void OnToggleStateChanged(bool state)
		{
            if (!this.IsActive() || !Application.isPlaying)
            {
                //Debug.LogError(gameObject.name + "  is deactive");
                _needRefresh = true;
                return;
            }
		    SetToggleEffect(state);

		    if (m_TargetContent == null && isOn && m_targetPrefab != null && m_contentParent != null)
		    {
		        m_TargetContent = Instantiate(m_targetPrefab);
		        RectTransform tf = m_TargetContent.transform as RectTransform;
		        tf.parent = m_contentParent;
                if (tf.anchorMin == tf.anchorMax)
                {
                    tf.anchoredPosition = Vector2.zero;
                }
                else
                {
                    tf.offsetMin = Vector2.zero;
                    tf.offsetMax = Vector2.zero;
                }
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;
		    }
            // Toggle the content
            if (this.m_TargetContent != null)
                m_TargetContent.SetActive(this.isOn);
			
			this.InternalEvaluateAndTransitionState(false);
            //Debug.Log(gameObject.name + "   active gameobject  " + state);
            for (int i = 0; i < m_Activate.Count; ++i)
                Set(m_Activate[i], state);
            for (int i = 0; i < m_Deactivate.Count; ++i)
                Set(m_Deactivate[i], !state);

            if (_sendToggleEvent)
            {
                //Debug.Log(gameObject.name + "   onToggleEvent  " + state);
                onToggleEvent.Invoke(this, state);
            }
        }


        void Set(GameObject go, bool state)
        {
            if (go != null)
            {
                go.SetActive(state);
            }
        }

		/// <summary>
		/// Internaly evaluates and transitions to the current state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionState(bool instant)
		{
			// Transition the active graphic children
			if (this.graphic != null && this.graphic.transform.childCount > 0)
			{
				// Loop through the children
				foreach (Transform child in this.graphic.transform)
				{
					// Try getting a graphic component
					Graphic g = child.GetComponent<Graphic>();
					
					if (g != null)
					{
						if (instant) g.canvasRenderer.SetAlpha((!this.isOn) ? 0f : 1f);
						else g.CrossFadeAlpha((!this.isOn) ? 0f : 1f, 0.1f, true);
					}
				}
			}
			
			// Do a state transition
			this.DoStateTransition(this.m_CurrentState, instant);
		}

	    private void SetToggleEffect(bool flag, bool imme = false)
	    {
            Color newImageColor = flag ? m_ImageColors.onColor : m_ImageColors.offColor;
            Color newTextColor = flag ? m_TextColors.onColor : m_TextColors.offColor;
            Sprite newSprite = flag ? m_ImageSpriteState.onSprite : m_ImageSpriteState.offSprite;

            // Check if the tab is active in the scene
            if (gameObject.activeInHierarchy)
            {
                // Do the image transition
                switch (m_ImageTransition)
                {
                    case Selectable.Transition.ColorTint:
                        StartColorTween(m_ImageTarget, newImageColor, imme ? 0 : m_ImageColors.fadeDuration);
                        break;
                    case Selectable.Transition.SpriteSwap:
                        DoSpriteSwap(this.m_ImageTarget, newSprite);
                        break;
                }

                // Do the text transition
                switch (m_TextTransition)
                {
                    case TextTransition.ColorTint:
                        StartColorTween(m_TextTarget, newTextColor, imme ? 0 : m_TextColors.fadeDuration);
                        break;
                }
            }
	    }

		/// <summary>
		/// Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Graphic target, Color targetColor, float duration)
		{
			if (target == null)
				return;
			
			if (!Application.isPlaying || duration == 0f)
			{
				target.canvasRenderer.SetColor(targetColor);
			}
			else
			{
                //target.CrossFadeColor(targetColor, duration, true, true);
			    target.color = targetColor;
			}
		}
		
		/// <summary>
		/// Does a sprite swap.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Image target, Sprite newSprite)
		{
			if (target == null)
				return;

			target.overrideSprite = newSprite;
            if(m_IsTabSpriteNativeSize)
                target.SetNativeSize();
		}
		
		/// <summary>
		/// Activate the tab.
		/// </summary>
		public void Activate()
		{
			if (!this.isOn)
				this.isOn = true;
		}

	    public void SetStateWithoutEvent(bool state)
	    {
	        _sendToggleEvent = false;
            isOn = state;
	        _sendToggleEvent = true;
	    }

	}


}


[Serializable]
public struct ToggleColorBlock
{
    //
    // Static Properties
    //
    public static ToggleColorBlock defaultColorBlock
    {
        get
        {
            return new ToggleColorBlock
            {
                m_OnColor = new Color32(255, 255, 255, 255),
                m_OffColor = new Color32(245, 245, 245, 255),
                m_DisabledColor = new Color32(200, 200, 200, 128),
                m_FadeDuration = 0.1f
            };
        }
    }

    //
    // Properties
    //
    [SerializeField]
    private Color m_OnColor;
    [SerializeField]
    private Color m_OffColor;
    [SerializeField]
    private Color m_DisabledColor;
    [SerializeField]
    private float m_FadeDuration;

    public Color onColor
    {
        get
        {
            return this.m_OnColor;
        }
        set
        {
            this.m_OnColor = value;
        }
    }

    public Color offColor
    {
        get
        {
            return this.m_OffColor;
        }
        set
        {
            this.m_OffColor = value;
        }
    }


    public Color disabledColor
    {
        get
        {
            return this.m_DisabledColor;
        }
        set
        {
            this.m_DisabledColor = value;
        }
    }

    public float fadeDuration
    {
        get
        {
            return this.m_FadeDuration;
        }
        set
        {
            this.m_FadeDuration = value;
        }
    }
}


[Serializable]
public struct ToggleSpriteState
{
    //
    // Properties
    //
    [SerializeField]
    private Sprite m_OnSprite;
    [SerializeField]
    private Sprite m_OffSprite;
    [SerializeField]
    private Sprite m_DisabledSprite;

    public Sprite onSprite
    {
        get
        {
            return this.m_OnSprite;
        }
        set
        {
            this.m_OnSprite = value;
        }
    }

    public Sprite offSprite
    {
        get
        {
            return this.m_OffSprite;
        }
        set
        {
            this.m_OffSprite = value;
        }
    }

    public Sprite disabledSprite
    {
        get
        {
            return this.m_DisabledSprite;
        }
        set
        {
            this.m_DisabledSprite = value;
        }
    }
}
