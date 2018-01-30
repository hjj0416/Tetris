using System.Collections.Generic;
using UnityEngine;

public class UISimplePool
{
    private string _poolName;
    private Stack<GameObject> _pool = new Stack<GameObject>();
    private Transform _root;
    private GameObject _prefab;

    public UISimplePool(string poolName, Transform poolRoot, GameObject prefab)
    {
        _poolName = poolName;
        _root = poolRoot;
        _prefab = prefab;
        if (IsInValid()) return;
        _prefab.SetActive(false);
    }

    public void Perload(int count)
    {
        if (count <= 0) return;
        if (IsInValid()) return;
        if (_pool.Count > 0) return;
        GameObject go = GameObject.Instantiate(_prefab);
        go.SetActive(false);
        Transform tf = go.transform;
        tf.SetParent(_root);
        _pool.Push(go);
    }

    /// <summary>
    /// 创建实例
    /// </summary>
    /// <returns></returns>
    public Transform Spawn()
    {
        if (IsInValid()) return null;
        GameObject go;
        if (_pool.Count <= 0)
        {
            go = GameObject.Instantiate(_prefab);
        }
        else
        {
            go = _pool.Pop();
        }
        go.SetActive(true);
        Transform tf = go.transform;
        return tf;
    }

    public void Despawn(Transform tf)
    {
        if (IsInValid()) return;
        tf.SetParent(_root);
        tf.gameObject.SetActive(false);
        _pool.Push(tf.gameObject);
    }

    public void DespawnAll(Transform parent)
    {
        if (IsInValid()) return;
        while (parent.childCount>0)
        {
            Despawn(parent.GetChild(0));
        }
    }

    public void Dispose()
    {
        if (!IsInValid())
        {
            while (_pool.Count > 0)
            {
                Object.Destroy(_pool.Pop());
            }
        }
        _root = null;
        _prefab = null;
    }

    private bool IsInValid()
    {
        if (_root == null)
        {
            Debug.LogWarning(string.Format("uipool {0}'s root is null", _poolName));
            return true;
        }
        if (_prefab == null)
        {
            Debug.LogWarning(string.Format("uipool {0}'s  prefab is null", _poolName));
            return true;
        }
        return false;
    }
}
