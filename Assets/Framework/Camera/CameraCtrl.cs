using UnityEngine;

[ExecuteInEditMode]
public class CameraCtrl : MonoBehaviour 
{
    public enum CameraType
    {
        MainCamera,
        UICamera,
    }

    private Vector3 orgPos = Vector3.one;
    public Vector3 OriginalPos
    {
        get
        {
            return orgPos;
        }
    }

    public CameraType cameraType = CameraType.MainCamera;
    public Camera Camera{get; private set; }
	// Use this for initialization
	void Awake ()
	{
	    if (!Application.isPlaying) return;
	    orgPos = transform.position;
	    Camera = GetComponent<Camera>();
        CameraMgr.Instance.SetCamera(this, cameraType);
	}

    public void EnableCamera(bool enable)
    {
        if (Camera != null)
        {
            Camera.enabled = enable; 
        }         
    }

}
