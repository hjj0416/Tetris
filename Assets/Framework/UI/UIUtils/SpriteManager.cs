using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载及存储一些零散sprite资源
/// 可以设定过期时间，自动移除
/// </summary>
public class SpriteManager : MonoSingleton<SpriteManager>
{
    public static readonly short LOAD_SPRITE_EVENT = 1;
    public static readonly short LOAD_ALL_SPRITE_EVENT = 2;

    //设定过期时间（秒）
    private float SPRITE_EXPIRE_TIME = 60;
    //检查时间间隔
    private float CHECK_EXPIRE_TIME_DELTA = 25;

    //sprite 仓库
    public List<SpriteData> _list = new List<SpriteData>(30);
    Dictionary<string, SpriteData> _dic = new Dictionary<string, SpriteData>();
    //loader
    SpritesLoader _spritesLoader;
    List<SpriteLoader> _loadingLoader = new List<SpriteLoader>(30); 
    //event
    public static EventDispatcher GlobalEventDispatcher = new EventDispatcher();

    private bool _isInArena = false;//是否进入单局

    /// <summary>
    /// 获取sprite，为null时加载
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Sprite GetSprite(string path, bool cacheSprite = true)
    {
        SpriteData data = null;
        if (_dic.TryGetValue(path, out data))
        {
            data.usedTime = Time.time;
            return data.sprite;
        }
        else
        {
            LoadSprite(path, cacheSprite);
            return null;
        }
    }

    public void LoadSprite(string path, bool cacheSprite = true)
    {
        _pastTime = 0;//加载请求时候，延迟检查图片过期
        int count = _loadingLoader.Count;
        for (int i = 0; i < count; i++)
        {
            if (_loadingLoader[i].Path.Equals(path))
            {
                //Debug.Log(path + " is loading ,  loading total count " + _loadingLoader.Count);
                return;
            }
        }
        Debug.Log("--- start load  " + path + "  loading count "+ _loadingLoader.Count);
        SpriteLoader sl = new SpriteLoader();
        sl.SetManager(this);
        sl.Run(path, cacheSprite);
        _loadingLoader.Add(sl);
    }

    public void LoadSprites(List<string> paths, bool cacheSprite = true)
    {
        _pastTime = 0;//加载请求时候，延迟检查图片过期
        if (_spritesLoader != null && _spritesLoader.IsRun)
        {
            Debug.LogError("前一次加载sprites请求没有完成");
            _spritesLoader.OnReturnToPool();
        }
        //int len = paths.Count;
        //for (int i = 0; i < len; i++)
        //{
        //    Debug.Log(string.Format("load sprites {0}  index {1}", paths[i], i));
        //}
        //Debug.LogError("--- start load sprites count " + paths.Count);
        if(_spritesLoader == null)
            _spritesLoader = new SpritesLoader();
        _spritesLoader.SetManager(this);
        _spritesLoader.Run(paths, cacheSprite);
        _pastTime = 0;//加载请求时候，延迟检查图片过期
    }

    public SpriteData TryGetSprite(string path)
    {
        SpriteData ret;
        if (_dic.TryGetValue(path, out ret))
            return ret;
        return null;
    }

    public void RemoveSprite(List<string> paths)
    {
        SpriteData sd;
        int lastIndex = _list.Count - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            sd = _list[i];
            if (paths.Contains(sd.path))
            {
                paths.Remove(sd.path);
                _list[i] = _list[lastIndex];
                _list.RemoveAt(lastIndex);
                _dic.Remove(sd.path);
                lastIndex--;
                //Debug.LogError("-------------  remove sprite " + sd.path);
                if (paths.Count <= 0)
                {
                    break;
                }
            }
        }
    }

    #region loader

    public void _RecycleLoader(SpriteLoader sl)
    {
        _loadingLoader.Remove(sl);
    }

    /// <summary>
    /// 缓存加载的sprite
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sp"></param>
    public void _CacheSprite(string path, Sprite sp)
    {
        if (_dic.ContainsKey(path))
        {
            return;
        }
        SpriteData data =new SpriteData();
        data.SetData(path, sp, Time.time);
        _list.Add(data);
        _dic.Add(path, data);
    }

    public void _OnLoadSprite(string path, Sprite sp)
    {
        //如果加载失败，此时sprite是null
        //Debug.LogError("--- complete load " + path);
        SpriteEvent evt = new SpriteEvent();
        evt.SetData(path, sp);
        GlobalEventDispatcher.SendEvent<SpriteEvent>(LOAD_SPRITE_EVENT, evt);
    }

    public void _OnLoadAllSprites()
    {
        GlobalEventDispatcher.SendEvent(LOAD_ALL_SPRITE_EVENT);
    }

    #endregion

    #region auto clear

    private float _pastTime;

    void LateUpdate()
    {
        if (_isInArena) return;
        _pastTime += Time.deltaTime;
        if (_pastTime > CHECK_EXPIRE_TIME_DELTA)
        {
            _pastTime = 0;
            ClearExpiredSprite();
        }

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Debug.LogError("-------  unload asset");
        //    Resources.UnloadUnusedAssets();
        //}
    }

    private void ClearExpiredSprite(bool forceClear = false)
    {
        int startCount = _list.Count;
        SpriteData sd;
        int lastIndex = _list.Count - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            sd = _list[i];
            if (forceClear || (Time.time - sd.usedTime > SPRITE_EXPIRE_TIME))
            {
                _list[i] = _list[lastIndex];
                _list.RemoveAt(lastIndex);
                _dic.Remove(sd.path);
                lastIndex--;
                //Debug.LogError("-------------  remove sprite " + sd.path);
            }
        }
        if (startCount - _list.Count > 0)
        {
            if(!_isInArena)
                Resources.UnloadUnusedAssets();
            //Debug.Log("------------  remove sprite : " + (startCount - _list.Count) + "      remain : " + _list.Count);
        }
    }

    #endregion

    public void EnterArena()
    {
        if (_isInArena) return;
        _isInArena = true;
        ClearExpiredSprite(true);
        Resources.UnloadUnusedAssets();
    }

    public void ExitArena()
    {
        if (!_isInArena) return;
        _isInArena = false;
        ClearExpiredSprite(true);
        //Resources.UnloadUnusedAssets();
        UIManager.Instance.UnloadUnused();
    }
}

public class BaseSpriteLoader
{
    private Action<string, object> _sucessCallback;
    private Action<string> _failCallback;

    protected SpriteManager _mgr;
    protected bool _cacheSprite;//是否缓存sprite

    public BaseSpriteLoader()
    {
        _failCallback = OnIconLoadFaild;
        _sucessCallback = OnIconLoadSuccess;
    }

    public void SetManager(SpriteManager mgr)
    {
        _mgr = mgr;
    }
    public void LoadSprite(string path)
    {
        SpriteData sd = _mgr.TryGetSprite(path);
        if (sd == null)
        {
            object obj = Resources.Load(path);
            if (obj != null)
                OnIconLoadSuccess(path, obj);
            else
                OnIconLoadFaild(path);
        }
        else
        {
            sd.usedTime = Time.time;
            OnComplete(sd.sprite);
        }
    }

    protected void OnIconLoadSuccess(string path, object content)
    {
        Sprite loadSprite = null;
        if (content is Texture2D)
        {
            Texture2D tex2D = (Texture2D)content;
            loadSprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.zero);
        }
        else if (content is Sprite)
        {
            loadSprite = (Sprite)content;
        }
        if (loadSprite != null)
        {
            if(_cacheSprite)
                _mgr._CacheSprite(path, loadSprite);
        }
        else
        {
            Debug.LogErrorFormat("地址为{0}的资源无法转换为Sprite类型", path);
        }
        OnComplete(loadSprite);
    }

    protected void OnIconLoadFaild(string path)
    {
        Debug.LogErrorFormat("地址为{0}的资源加载失败", path);
        OnComplete(null);
    }

    protected virtual void OnComplete(Sprite sp)
    {
    }

    #region pool

    protected virtual void Clear()
    {
        _mgr = null;
    }

    public void OnAwakenFromPool()
    {
        Clear();
    }

    public void OnReturnToPool()
    {
        Clear();
    }
    #endregion
}

public class SpriteLoader:BaseSpriteLoader
{
    private string _path;

    public string Path
    {
        get { return _path; }
    }
    public void Run(string path, bool cacheSprite)
    {
        _path = path;
        _cacheSprite = cacheSprite;
        LoadSprite(path);
    }

    protected override void OnComplete(Sprite sp)
    {
        base.OnComplete(sp);
        _mgr._OnLoadSprite(_path, sp);
        _mgr._RecycleLoader(this);
    }

    protected override void Clear()
    {
        base.Clear();
        _path = string.Empty;
    }
}

public class SpritesLoader : BaseSpriteLoader
{
    private int _loadingCount = 0;
    public bool IsRun { get; protected set; }

    public void Run(List<string> paths, bool cacheSprite)
    {
        _cacheSprite = cacheSprite;
        IsRun = true;
        int count = paths.Count;
        _loadingCount = count;
        for (int i = 0; i < count; i++)
        {
            LoadSprite(paths[i]);
        }
    }

    protected override void OnComplete(Sprite sp)
    {
        base.OnComplete(sp);
        _loadingCount--;
        //if(sp!=null)
        //    Debug.Log(string.Format("加载完成一个 {0}  还剩 {1}", sp.name, _loadingCount));
        //else
        //    Debug.Log(string.Format("加载失败 还剩 {0}", _loadingCount));
        if (_loadingCount <= 0)
        {
            IsRun = false;
            _mgr._OnLoadAllSprites();
        }
    }

    protected override void Clear()
    {
        base.Clear();
        _loadingCount = 0;
        IsRun = false;
    }
}


public class SpriteData
{
    public string path;
    public Sprite sprite;
    public float usedTime;

    public void SetData(string path, Sprite sp, float time)
    {
        this.path = path;
        sprite = sp;
        usedTime = time;
    }

    void Clear()
    {
        path = String.Empty;
        sprite = null;
        usedTime = 0;
    }

    public void OnAwakenFromPool()
    {
        Clear();
    }

    public void OnReturnToPool()
    {
        Clear();
    }
}

public class SpriteEvent
{
    public string spriteName;
    public Sprite sprite;

    public void SetData(string name, Sprite sp)
    {
        spriteName = name;
        sprite = sp;
    }

    void Clear()
    {
        spriteName = String.Empty;
        sprite = null;
    }

    public void OnAwakenFromPool()
    {
        Clear();
    }

    public void OnReturnToPool()
    {
        Clear();
    }
}