using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

#region cb
public delegate void VoidCallback();
public delegate Ret RetCallback<Ret>();
public delegate Ret RetCallback<Ret, ArgType0>(ArgType0 obj0);
public delegate Ret RetCallback<Ret, ArgType0, ArgType1>(ArgType0 obj0, ArgType1 obj1);
#endregion

[ExecuteInEditMode]
public class Utils : MonoSingleton<Utils>
{
    public override bool Init()
    {
       bool flag = base.Init();
        if (!flag)
        {
            return false;
        }
        return true;
    }

    #region Layer Manager

    public enum LayerEnum 
    {
        Default,
    }


    public static int GetLayer(LayerEnum layer)
    {
        return LayerMask.NameToLayer(layer.ToString());
    }

    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        foreach (Transform child in t)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    static public void SetLayer(GameObject go, LayerEnum layerEnum)
    {
        int layer = GetLayer(layerEnum);
        SetLayer(go, layer);
    }

    static public void SetLayerOrder(GameObject go, int depth)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        int count = renderers == null ? 0 : renderers.Length;
        for (int i = 0; i < count; i++)
        {
            Renderer renderer = renderers[i];
            renderer.sortingOrder = depth;
        }
    }

    static public int LayerMaskToLayer(LayerMask mask)
    {
        int v = mask.value;
        int i = 0;
        while (v != 1)
        {
            v = v >> 1;
            i++;
        }
        return i;
    }

    #endregion


    #region delay call

    struct CallBack
    {
        public float delay;
        public Action fn;
    }

    private CallBack delayCall;

    public void DelayCall(float delay, Action callback)
    {
        if (callback == null) return;

        delayCall.delay = delay;
        delayCall.fn = callback;
        StartCoroutine("call");
    }

    public void ClearDelayCall()
    {
        StopCoroutine("call");
    }

    private IEnumerator call()
    {
        CallBack cb = new CallBack {delay = delayCall.delay, fn = delayCall.fn};
        if (cb.delay <= 0)
            yield return new WaitForEndOfFrame();
        else
            yield return new WaitForSeconds(cb.delay);
        try
        {
            cb.fn();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #endregion


    #region repeat call
    private Dictionary<int, bool> repeatIDs = new Dictionary<int, bool>(); //use dictionary for rapid find
    /// <summary>
    /// repeat call per interval time on the special object.
    /// compare to the InvokeRepeat, it can use call back delegate as the callback
    /// </summary>
    /// <param name="obj">the target obj</param>
    /// <param name="interval"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    public void Repeat(GameObject obj, float interval, VoidCallback cb)
    {
        if (!obj) return;
        //register ID
        int ID = obj.GetInstanceID();
        if (repeatIDs.ContainsKey(ID))
        {
            Debug.LogError("repeat: "+ ID + " can't start again while running!");
            return;
        }
        repeatIDs[ID] = true;
        StartCoroutine(repeat(ID, interval, cb));
    }

    /// <summary>
    /// cancel a repeat call on the special object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public void CancelRepeat(GameObject obj)
    {
        if (!obj) return;
        repeatIDs.Remove(obj.GetInstanceID());
    }

    private IEnumerator repeat(int ID, float interval, VoidCallback cb)
    {
        //loop repeat call per interval time until you call CancelRepeat to remove the repeat
        while (repeatIDs.ContainsKey(ID))
        {
            yield return new WaitForSeconds(interval);
            cb();
        }
    }
    #endregion



    #region 创建对象
    public static GameObject CreatePrefab(string prefabsPath, Transform parent = null)
    {
        GameObject go;
        UnityEngine.Object prefab = Resources.Load(prefabsPath);
        if (prefab != null)
        {
            go = GameObject.Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            if (parent !=null)
            {
                AddChild(parent, go);
            }
            return go;
        }
        return null;
    }
    public static GameObject CreateChild(Transform parent, string name = "GameObject")
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go;
    }

    static public Transform GetMissingChild(Transform parent, string name)
    {
        Transform trans = parent.Find(name);
        if (trans == null)
        {
            GameObject go = CreateChild(parent, name);
            return go.transform;
        }
        return trans;
    }

    static public GameObject GetMissingObject(string name, params Type[] compents)
    {
        if (string.IsNullOrEmpty(name)) return null;
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name, compents);
            go.name = name;
        }
        return go;
    }

    static public void AddChild(Transform parent, GameObject go)
    {
        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent;
            t.localPosition = Vector3.zero;
            //t.localRotation = Quaternion.identity;
            //t.localScale = Vector3.one;
            go.layer = parent.gameObject.layer;
        }
        else
        {
            Debug.LogError("Instantiate a prefab failed !");
        }
    }

    public static void RemoveChild(Transform parnt, string childName)
    {
        if (parnt == null) return;
        Transform child = parnt.Find(childName);
        if(child !=null)
        {
            Destroy(child.gameObject);
        }
    }

    public static void RemoveChildren(Transform trans, int leftCount = 0)
    {
        if (!trans) return;
        if (leftCount >= trans.childCount) return;
        Transform child = trans.GetChild(leftCount);
        while (child)
        {
            DestroyImmediate(child.gameObject);
            if (trans.childCount <= leftCount)
            {
                break;
            }
            child = trans.GetChild(leftCount);
        }
    }


    /// <summary>
    /// 递归遍历
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChild(Transform node, string name)
    {
        if (node.name == name)
        {
            return node;
        }
        foreach (Transform child in node)
        {
            Transform tNode = FindChild(child, name);
            if (tNode != null && tNode.name == name)
            {
                return tNode;
            }
        }
        return null;
    }

    /// <summary>
    /// 递归遍历
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindChild<T>(Transform node, string name)
    {
        T t = default(T);
        if (node.name == name)
        {
            t = node.GetComponent<T>();
            if(t!=null)
                return t;
        }
        foreach (Transform child in node)
        {
            Transform tNode = FindChild(child, name);
            if (tNode != null && tNode.name == name)
            {
                t = tNode.GetComponent<T>();
                if(t !=null)
                    return t;
            }
        }
        return t;
    }

    /// <summary>
    /// 设置孩子的数量，不够就补齐，多了就移除
    /// </summary>
    /// <param name="curPageTrans"></param>
    /// <param name="itemCountPerPage"></param>
    /// <returns></returns>
    public static void SetChildrenNum(Transform parent, int targetCount)
    {
        if (parent == null)
        {
            Debug.LogError("transform is null");
            return;
        }
        int totalCount = parent.childCount;
        if (totalCount <= 0)
        {
            Debug.LogError("child is empty");
            return;
        }
        Transform firstChild = parent.GetChild(0);
        int toAddCount = targetCount - totalCount;
        if (toAddCount == 0)
            return;
        if (toAddCount > 0)
        {
            //add child
            for (int i = 0; i < toAddCount; ++i)
            {
                Transform child = Instantiate(firstChild, firstChild.position, firstChild.rotation) as Transform;
                child.parent = parent;
                child.localScale = firstChild.localScale;
            }
        }
        else
        {
            ArrayList childsToRemove = new ArrayList();
            //remove child
            for (int i = targetCount; i < totalCount; i++)
            {
                Transform child = parent.GetChild(i);
                //out of itemCountPerPage, just remove it
                if (child) childsToRemove.Add(child);
            }
            //remove overflow child
            foreach (Transform trans in childsToRemove)
            {
                trans.parent = null;
                Destroy(trans.gameObject);
            }
        }
    }

    #endregion



    #region other

    /// <summary>
    /// 唯一ID生成器，使用从2000-01-01开始到目前时间的秒数单位TICK数量作为ID，
    /// 32位整形，够用136年。足够了确保唯一性。但是使用时必须注意，不同制作机器的时间
    /// 同步
    /// </summary>
    /// <returns></returns>    
    static public int GenID()
    {
        DateTime centuryBegin = new DateTime(2000, 1, 1);
        DateTime currentDate = DateTime.Now;
        long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        return (int)elapsedSpan.TotalSeconds;
    }

    public static IEnumerator DoInvoke(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    /// <summary>
    /// get the object hierarchy path of the Transform me
    /// </summary>
    /// <param name="me"></param>
    /// <returns></returns>
    public static string GetPath<T>(Transform me) where T : Component
    {
        string ret = me.name;
        T par = me.GetComponent<T>();
        //search all of the parents until the parent has a T component
        while (me)
        {
            me = me.parent;
            par = me.GetComponent<T>();
            if (!par)
                ret = string.Format("{0}/{1}", me.name, ret);
            else
                break;
        }
        return ret;
    }

    //mesh
    public static void SetVertexColor(Mesh mesh, Color color)
    {
        if (!mesh) return;
        //change vertex color
        Vector3[] vertices = mesh.vertices;//顶点
        Color[] colors = new Color[vertices.Length];
        int i = 0;
        while (i < vertices.Length)
        {
            colors[i] = color;
            i++;
        }
        mesh.colors = colors;
    }

    //comparor
    public static int IntTransformNameComparor(Transform x, Transform y)
    {
        if (x == null && y == null) return 0;
        else if (x == null) return -1;
        else if (y == null) return 1;
        else
        {
            int nX;
            if (!int.TryParse(x.name, out nX))
            {
                return -1;
            }
            int nY;
            if (!int.TryParse(y.name, out nY))
            {
                return -1;
            }
            return nX.CompareTo(nY);
        }
    }


    #endregion


    #region format time



    //解析并格式化时间 时:分:秒//
    public static string GetFormatTime(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
    }

    /// <summary>
    /// 将剩余秒数转化为时间
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string FormatTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        return (d > 0 ? (d + "D ") : "") + (h > 0 ? (h + "H ") : "") + (m > 0 ? (m + "M ") : "") + (s > 0 ? (s + "S") : "");
    }

    public static string FormatShortTime(int seconds)
    {
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;
        int s = timeSpan.Seconds;
        if(d > 0)
            return d + "D " + (h > 0 ? (h + "H ") : "");

        if (h > 0)
            return h + "H " + (m > 0 ? (m + "M ") : "");

        if (m > 0)
            return m + "M " + (s > 0 ? (s + "S") : "");

        return s + "S";
    }

    public static string FormatNum(int num)
    {
        if (num > 1000000)
        {
            return string.Format("{0}M", (((float)num)/1000000).ToString("N1"));
        }

        if (num > 1000)
        {
            return string.Format("{0}K", (((float)num)/1000).ToString("N1"));
        }

       return num.ToString("N0");
    }

    #endregion


    public static Color GetColor(int r, int g, int b)
    {
        return new Color((float)r / 256, (float)g / 256, (float)b / 256);
    }

}



/// <summary>
/// 对原生代码进行扩展
/// </summary>
public static class NativeExtend
{
    /// <summary>
    /// 添加子节点
    /// </summary>
    public static void AddChild(this Transform target, Transform child)
    {
        if (target == null || child == null)
        {
            Debug.LogError("parent or child is null");
        }
        child.parent = target;
        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = Vector3.zero;
        ChangeChildLayer(child, target.gameObject.layer);
    }

    /// <summary>
    /// 修改子节点Layer
    /// </summary>
    private static void ChangeChildLayer(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            ChangeChildLayer(child, layer);
        }
    }

    /// <summary>
    /// 实例化预设体并添加到子节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    static public GameObject AddPrefabChild(this Transform parent, GameObject prefab)
    {
        if (parent == null || prefab == null)
        {
            Debug.LogError("parent or prefab is null");
            return null;
        }
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        if (go != null)
        {
            parent.AddChild(go.transform);
            go.name = prefab.name;
        }
        else
        {
            Debug.LogError("Instantiate a prefab failed : " + prefab.name);
        }
        return go;
    }

    /// <summary>
    /// 添加子节点并绑定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T AddChildComponent<T>(this Transform parent)where T : MonoBehaviour
    {
        //已经存在，就直接返回
        T t = parent.GetComponentInChildren<T>();
        if (t != null) return t;
        //不存在，重新创建
        GameObject go = new GameObject();
        t = go.AddComponent<T>();
        parent.AddChild(go.transform);
        go.name = typeof (T).ToString();
        return t;
    }

    static public T GetMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    static public T GetMissingComponent<T>(this Transform tf) where T : Component
    {
        T comp = tf.GetComponent<T>();
        if (comp == null)
        {
            comp = tf.gameObject.AddComponent<T>();
        }
        return comp;
    }

    public static T AddChildComponent<T> (this GameObject obj) where T : MonoBehaviour
	{
		GameObject child = new GameObject( typeof(T).Name );
		child.transform.SetParent(obj.transform);
		return child.AddComponent<T>();
	}

}
