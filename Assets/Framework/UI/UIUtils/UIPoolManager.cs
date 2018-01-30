using System.Collections.Generic;
using UnityEngine;

public class UIPoolManager : MonoBehaviour
{
    private bool _debug = false;
    private Transform _poolRoot;
    private Dictionary<string, UISimplePool> _poolDic = new Dictionary<string, UISimplePool>();

    void Awake()
    {
        _poolRoot = transform;
    }


    public void Perload(string poolName, string prefabPath, int count = 1)
    {
        UISimplePool pool = getPool(poolName);
        if (pool != null)
        {
            pool.Perload(count);
        }
        else
        {
            GameObject prefab = createPerfab(prefabPath);
            if (prefab != null)
            {
                Perload(poolName, prefab);
            }
        }
    }

    public void Perload(string poolName, GameObject prefab, int count = 1)
    {
        UISimplePool pool = getPool(poolName);
        if (pool != null)
        {
            pool.Perload(count);
        }
        else
        {
            UISimplePool _pool = spawnPool(poolName, prefab);
            _pool.Perload(count);
        }
    }

    public Transform Spawn(string poolName, string prefabPath)
    {
        UISimplePool pool = getPool(poolName);
        if (pool != null)
        {
            return pool.Spawn();
        }
        else
        {
            GameObject prefab = createPerfab(prefabPath);
            if (prefab != null)
                return Spawn(poolName, prefab);
            return null;
        }
    }

    public Transform Spawn(string poolName, GameObject prefab = null)
    {
        UISimplePool pool = getPool(poolName);
        if (pool != null)
        {
            return pool.Spawn();
        }
        else if(prefab!=null)
        {
            pool = spawnPool(poolName, prefab);
            return pool.Spawn();
        }
        else
        {
            Debug.LogError("prefab is null");
            return null;
        }
    }

    public void UnSpawn(string poolName, Transform tf)
    {
        unSpawn(poolName, tf);
    }

    public void Clear(string poolName)
    {
        UISimplePool _pool;
        if (_poolDic.TryGetValue(poolName, out _pool))
        {
            _pool.Dispose();
            _poolDic.Remove(poolName);
        }
    }

    public void ClearAll()
    {
        foreach (var pool in _poolDic)
        {
            pool.Value.Dispose();
        }
        _poolDic.Clear();
    }


    private GameObject createPerfab(string prefabPath)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("prefab path is error : " + prefabPath);
        }
        //Debug.LogError("--- create prefab " + prefabPath);
        return prefab;
    }

    private UISimplePool getPool(string poolName)
    {
        UISimplePool _pool;
        if (_poolDic.TryGetValue(poolName, out _pool))
        {
            return _pool;
        }
        return null;
    }

    private UISimplePool spawnPool(string poolName, GameObject prefab)
    {
        UISimplePool _pool;
        if (!_poolDic.TryGetValue(poolName, out _pool))
        {
            _pool = new UISimplePool(poolName, _poolRoot, prefab);
            _poolDic.Add(poolName, _pool);
        }
        return _pool;
    }

    private void unSpawn(string poolName, Transform tf)
    {
        UISimplePool _pool;
        if (_poolDic.TryGetValue(poolName, out _pool))
        {
            _pool.Despawn(tf);
        }
        else
        {
            Debug.Log("------  pool " + poolName + " is destroyed, so destroy gameobject");
            Destroy(tf.gameObject);
        }
    }

}
