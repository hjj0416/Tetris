using System;
using UnityEngine.SceneManagement;
using UnityEngine;


    public class SceneID
    {
        public static readonly string Start = "Start";

    }

    /// <summary>
    /// 每个场景只能存在唯一一个main scene和一个additive scene
    /// </summary>
    public class SceneMgr
    {
        #region 单例
        private static SceneMgr mInstance;
        /// <summary>
        /// 获取资源加载实例
        /// </summary>
        /// <returns></returns>
        public static SceneMgr Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new SceneMgr();
                    mInstance.Init();
                }
                return mInstance;
            }
        }

        #endregion

        public string prevSceneName { get; private set; }
        public string curSceneName
        {
            get
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (scene!=null)
                {
                    return scene.name;
                }
                return string.Empty;
            }
        }

        private string _loadingSceneName;
        private Action _onLoaded;

        private bool _hasInit = false;
        public void Init()
        {
            if (_hasInit) return;
            _hasInit = true;
            prevSceneName = "";
        }

        public bool IsOpenScene(string sceneName)
        {
            if (string.IsNullOrEmpty(curSceneName)) return false;
            return curSceneName.Equals(sceneName);
        }

        public void EnterScene(string sceneName, Action cb = null)
        {
            //Debug.LogError("---------  change scene " + sceneName);
            Init();
            if (!string.IsNullOrEmpty(_loadingSceneName))
            {
                //UIManager.Instance.Alert("error", string.Format("正在切换场景{0}, 不能同时切换{1}", _loadingSceneName, sceneName), AlertMode.Ok);
                Debug.LogError(string.Format("正在切换场景{0}, 不能同时切换{1}", _loadingSceneName, sceneName));
                if (cb != null) cb();
                return;
            }
            if (curSceneName.Equals(sceneName))
            {
                Debug.LogWarning("EnterScene failed, cant enter same scene " + sceneName);
                if (cb != null) cb();
                return;
            }
            _loadingSceneName = sceneName;
            _onLoaded = cb;

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void ExitScene(string sceneName)
        {
            Init();
            if (string.IsNullOrEmpty(curSceneName)) return;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(curSceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(_loadingSceneName))
            {
                prevSceneName = curSceneName;
                _loadingSceneName = "";
                if (_onLoaded != null)
                {
                    _onLoaded();
                    _onLoaded = null;
                }
            }
            Debug.Log(string.Format("场景加载完成：{0}", scene.name));
        }

        public bool ExistScene(string sceneName)
        {
            return curSceneName.Equals(sceneName);
        }

    }
