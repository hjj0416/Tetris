using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(BaseScene), true)]
public class SceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BaseScene scene = (BaseScene)target;
        scene.gameObject.name = scene.GetType().ToString();
    }
}
