using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CanEditMultipleObjects, CustomEditor(typeof(UIMultImage), true)]
public class UIMultImageEditor : ImageEditor
{
    private SerializedProperty dataProperty;
    private SerializedProperty bNativeSize;

    protected virtual void OnEnable()
	{
        base.OnEnable();
        this.dataProperty = this.serializedObject.FindProperty("sprites");
        bNativeSize = serializedObject.FindProperty("nativeSize");
    }
	
	public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();
		this.serializedObject.Update();
		EditorGUILayout.Space();

        EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
        EditorGUILayout.PropertyField(bNativeSize, new GUIContent("Use Native Size"));
        EditorGUILayout.PropertyField(this.dataProperty, new GUIContent("Sprite"), true);
        EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

        this.serializedObject.ApplyModifiedProperties();
	}

}
