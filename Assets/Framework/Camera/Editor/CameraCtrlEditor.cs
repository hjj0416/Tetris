using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(CameraCtrl), true)]
public class CameraCtrlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CameraCtrl _target = (CameraCtrl) target;
        _target.gameObject.name = _target.cameraType.ToString();
    }
}
