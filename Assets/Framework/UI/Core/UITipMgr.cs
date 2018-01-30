//using System;
//using TheNextMoba.UI;
//using TheNextMoba.Utils;
//using UnityEngine;

//public class UITipMgr : MonoBehaviour
//{

//    #region 单例
//    private static UITipMgr mInstance;
//    /// <summary>
//    /// 获取资源加载实例
//    /// </summary>
//    /// <returns></returns>
//    public static UITipMgr Instance
//    {
//        get
//        {
//            if (mInstance == null)
//            {
//                Transform tf = UIManager.Instance.TipLayer;
//                mInstance = tf.gameObject.AddMissingComponent<UITipMgr>();
//            }
//            return mInstance;
//        }
//    }

//    #endregion

//    private const string _poolName = "UITip";

//    private Transform m_cacheTransform;
//    void Awake()
//    {
//        m_cacheTransform = transform;
//        UIManager.Instance.Perload(_poolName, PathDefine.UI_TIP);
//    }

//    public void Init()
//    {
//    }

//    public void ShowTip(string text, float duration)
//    {
//        Load((tip) =>
//        {
//            tip.SetDate(text, duration);
//            m_refresh = true;
//        });
//        Debug.Log(text);
//    }

//    void Load(Action<UITip> cb)
//    {
//        Transform tf = UIManager.Instance.Spawn(_poolName, PathDefine.UI_TIP);
//        tf.gameObject.SetActive(true);
//        RectTransform rect = tf as RectTransform;
//        rect.SetParent(m_cacheTransform);
//        rect.anchoredPosition = Vector2.zero;
//        rect.offsetMin = Vector2.zero;
//        rect.offsetMax = Vector2.zero;
//        rect.localScale = Vector3.one;
//        rect.SetAsLastSibling();
//        UITip tip = tf.gameObject.AddMissingComponent<UITip>();
//        cb(tip);
//    }

//    private bool m_refresh = false;
//    void Update()
//    {
//        if (!m_refresh) return;
//        m_refresh = false;
//        OnRefresh();
//    }

//    void OnRefresh()
//    {
//        int childCount = m_cacheTransform.childCount;
//        int index = 0;
//        for (int i = childCount - 1; i >= 0; i--)
//        {
//            Transform child = m_cacheTransform.GetChild(i);
//            Vector3 pos = new Vector3(0, index * 50 + 150, 0);
//            child.localPosition = pos;
//            index++;
//        }
//    }

//    public void UnSpawn(Transform tf)
//    {
//        UIManager.Instance.UnSpawn(_poolName, tf);
//    }

//}
