using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
//using DG.Tweening;

public class CameraMgr : MonoSingleton<CameraMgr>
{
    private Dictionary<CameraCtrl.CameraType, CameraCtrl> cameras = new Dictionary<CameraCtrl.CameraType,CameraCtrl>();

    public void ShakeScreen()
    {
        if (cameras.ContainsKey(CameraCtrl.CameraType.MainCamera))
        {
            Transform camTransform = cameras[CameraCtrl.CameraType.MainCamera].transform;
            camTransform.DOShakePosition(0.5f, 1, 30);
        }
    }

    public void SetCamera(CameraCtrl cameraCtrl, CameraCtrl.CameraType type)
    {
        cameras[type] = cameraCtrl;
    }

    public CameraCtrl GetCamera(CameraCtrl.CameraType type = CameraCtrl.CameraType.MainCamera)
    {
        return cameras[type];
    }

    public Vector3 World2UIPos(Vector3 worldPos)
    {
        //CameraCtrl mainCam = Instance.GetCamera(CameraCtrl.CameraType.TopCamera);
        //if (mainCam == null || mainCam.Camera == null) return Vector3.zero;
        //CameraCtrl uiCam = Instance.GetCamera(CameraCtrl.CameraType.UICamera);
        //if(uiCam == null || uiCam.Camera == null)return Vector3.zero;
        //Vector3 screenPos = mainCam.Camera.WorldToScreenPoint(worldPos);
        //screenPos.z = 0;
        //return uiCam.Camera.ScreenToWorldPoint(screenPos);
        return Vector3.zero;
    }

    public Vector3 Ui2WorldPos(Vector3 worldPos)
    {
        CameraCtrl mainCam = Instance.GetCamera();
        if (mainCam == null || mainCam.Camera == null) return Vector3.zero;
        CameraCtrl uiCam = Instance.GetCamera(CameraCtrl.CameraType.UICamera);
        if (uiCam == null || uiCam.Camera == null) return Vector3.zero;
        Vector3 screenPos = uiCam.Camera.WorldToScreenPoint(worldPos);
        screenPos.z = Mathf.Abs(mainCam.transform.position.z);
        return mainCam.Camera.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// 左下角是 0，0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 View2UIPos(float x, float y)
    {
        CameraCtrl uiCam = Instance.GetCamera(CameraCtrl.CameraType.UICamera);
        if (uiCam == null || uiCam.Camera == null) return Vector3.zero;
        return uiCam.Camera.ViewportToScreenPoint(new Vector3(x, y, 0));
    }

    public Vector3 View2WorldPos(float x, float y)
    {
        CameraCtrl uiCam = Instance.GetCamera(CameraCtrl.CameraType.UICamera);
        if (uiCam == null || uiCam.Camera == null) return Vector3.zero;
        return uiCam.Camera.ViewportToWorldPoint(new Vector3(x, y, 0));
    }

    #region focus

    public void FocusOn(GameObject go, float duration = 1, Action callback = null)
    {
        CameraCtrl main = GetCamera();
        Vector3 targetPos = go.transform.position;
        Vector3 currentCameraPos = main.transform.position;
        Tweener tw = main.transform.DOMove(new Vector3(targetPos.x, currentCameraPos.y, currentCameraPos.z), duration);
        tw.onComplete = () =>
        {
            if (callback != null)
                callback();
        };
    }


    public void UnFocus(float duration = 1, Action callback = null)
    {
        CameraCtrl main = GetCamera();
        Tweener tw = main.transform.DOMove(main.OriginalPos, duration);
        tw.onComplete = () =>
        {
            if (callback != null)
                callback();
        };
    }

    #endregion

    #region screen size


    public Vector3 Border(Vector3 worldPos, bool isLeftSide = true)
    {
        Vector3 viewPos = Instance.GetCamera().Camera.WorldToViewportPoint(worldPos);
        viewPos.x = isLeftSide ? 0 : 1;
        return Instance.GetCamera().Camera.ViewportToWorldPoint(viewPos);

    }

    public Vector3 GetScaleXPos(Vector3 scalexPos, bool isLeftSide = true)
    {
        Vector3 position = Border(scalexPos, isLeftSide);
        position.x = position.x * scalexPos.x;
        return position;
    }


    public static Vector3 GetWorldPosByViewPos(Vector2 pos)
    {
        return Instance.GetCamera().Camera.ViewportToWorldPoint(pos);
    }

    #endregion


}
