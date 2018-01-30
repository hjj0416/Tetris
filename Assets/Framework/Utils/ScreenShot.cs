using System;
using UnityEngine;

//GetComponent<Renderer>().material.mainTexture = tex;
public class ScreenShot 
{
    /// <summary>
    /// 移动端调用此函数进行截图操作
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="cb"></param>
    public void ShotScreen(Rect rect, Action<ScreenShotData> cb = null)
    {
        Utils.Instance.DelayCall(0.02f, () =>
        {
            DoShotScreen(rect, cb);
        });
    }

    private void DoShotScreen(Rect rect, Action<ScreenShotData> cb)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.persistentDataPath + "/screenshot/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        if (cb != null)
        {
            cb(new ScreenShotData(screenShot, filename));
        }
    }

	void Update()
    {
	    if(Input.GetKeyDown(KeyCode.S))
        {
            ShotScreen(new Rect(0, 0, Screen.width, Screen.height));
        }
	}
}


public class ScreenShotData
{
    public Texture2D m_Texture { get; private set; }
    public string TexturePath { get; private set; }

    public ScreenShotData(Texture2D tex, string path)
    {
        m_Texture = tex;
        TexturePath = path;
    }
}