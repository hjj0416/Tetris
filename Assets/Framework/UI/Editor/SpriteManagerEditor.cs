using UnityEditor;
[CustomEditor(typeof(SpriteManager), true)]
public class SpriteManagerEditor : Editor
{
    private SpriteManager mTarget;


    protected virtual void OnEnable()
    {
        mTarget = target as SpriteManager;
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        EditorGUILayout.LabelField("队列长度：", mTarget._list.Count.ToString());
        EditorGUILayout.Space();
        for (int i = 0; i < mTarget._list.Count; i++)
        {
            var s = mTarget._list[i];
            EditorGUILayout.LabelField("path : "+s.path);
        }
    }
}
