using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICreateMenu  
{

    [MenuItem("GameObject/UI/Custom Toggle", false, 2101)]
    static public void AddCustomToggle(MenuCommand menuCommand)
    {
        GameObject go = UICreateTool.CreateUIElementRoot("toggle", new Vector2(100,100));
        Image image = go.AddComponent<Image>();
        UIToggle toggle = go.AddComponent<UIToggle>();

        UICreateTool.PlaceUIElementRoot(go, menuCommand);
        Selection.activeGameObject = go;
    }


    [MenuItem("GameObject/UI/Loop Horizontal Scroll Rect", false, 2151)]
    static public void AddLoopHorizontalScrollRect(MenuCommand menuCommand)
    {
        GameObject go = UICreateTool.CreateLoopHorizontalScrollRect();
        UICreateTool.PlaceUIElementRoot(go, menuCommand);
    }

    [MenuItem("GameObject/UI/Loop Vertical Scroll Rect", false, 2152)]
    static public void AddLoopVerticalScrollRect(MenuCommand menuCommand)
    {
        GameObject go = UICreateTool.CreateLoopVerticalScrollRect();
        UICreateTool.PlaceUIElementRoot(go, menuCommand);
    }
}
