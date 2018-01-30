using UnityEngine;
using System;

public enum LogLevel
{    
    Info = 0,
    Warning,
    Error,
    NoLog,
}
	

[RequireComponent(typeof(Utils))]
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public bool Local = false;
    public bool IsDebug = false;


    void Awake()
    {
		Application.targetFrameRate = 40;
        Application.runInBackground = true;

        try
        {
            if (Instance)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void Start()
    {
        Init(() =>
        {
            UIManager.Instance.ShowWindow("UILogin");
            
        });
    }


#region 初始化	
   
    
    private void Init(Action cb)
    {
        Utils.Instance.Init();
        UIManager.Instance.Init();

        Bean.TryLoadConfig("BagItem");


        if (cb != null)
        {
            cb();
        }
    }

    void UnInit()
    {
    }


    public void ReLogin()
    {
        UnInit();
        Init(null);
    }

#endregion


    void OnApplicationPause(bool pause)
    {        
        
    }
    void OnApplicationQuit()
    {
    }

}
