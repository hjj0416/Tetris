using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICreateTool 
{
    #region code from MenuOptions.cs

    private const string kUILayerName = "UI";


    private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
    {
        // Find the best scene view
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null && SceneView.sceneViews.Count > 0)
            sceneView = SceneView.sceneViews[0] as SceneView;

        // Couldn't find a SceneView. Don't set position.
        if (sceneView == null || sceneView.camera == null)
            return;

        // Create world space Plane from canvas position.
        Vector2 localPlanePosition;
        Camera camera = sceneView.camera;
        Vector3 position = Vector3.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
        {
            // Adjust for canvas pivot
            localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
            localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

            localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
            localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

            // Adjust for anchoring
            position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
            position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

            Vector3 minLocalPosition;
            minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
            minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

            Vector3 maxLocalPosition;
            maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
            maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

            position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
            position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
        }

        itemTransform.anchoredPosition = position;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localScale = Vector3.one;
    }

    public static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            parent = GetOrCreateCanvasGameObject();
        }

        string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
        element.name = uniqueName;
        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
        Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
        GameObjectUtility.SetParentAndAlign(element, parent);
        if (parent != menuCommand.context) // not a context click, so center in sceneview
            SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

        Selection.activeGameObject = element;
    }

    public static GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        root.layer = LayerMask.NameToLayer(kUILayerName);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

        // if there is no event system add one...
        // CreateEventSystem(false);
        return root;
    }

    public static GameObject CreateUIElementRoot(string name, Vector2 size)
    {
        GameObject child = new GameObject(name);
        RectTransform rectTransform = child.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        return child;
    }

    public static GameObject CreateUIObject(string name, GameObject parent)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<RectTransform>();
        SetParentAndAlign(go, parent);
        return go;
    }

    public static void SetParentAndAlign(GameObject child, GameObject parent)
    {
        if (parent == null)
            return;

        child.transform.SetParent(parent.transform, false);
        SetLayerRecursively(child, parent.layer);
    }

    public static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i).gameObject, layer);
    }

    // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
    public static GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in selection or its parents? Then use just any canvas..
        canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in the scene at all? Then create a new one.
        return CreateNewUI();
    }
    #endregion

    #region loop scrollView

    public static GameObject CreateLoopHorizontalScrollRect()
    {
        GameObject root = CreateUIElementRoot("Loop Horizontal Scroll Rect", new Vector2(200, 200));

        GameObject viewpoint = CreateUIObject("ViewPoint", root);

        GameObject content = CreateUIObject("Content", viewpoint);

        RectTransform contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 0.5f);
        contentRT.anchorMax = new Vector2(0, 0.5f);
        contentRT.sizeDelta = new Vector2(0, 200);
        contentRT.pivot = new Vector2(0, 0.5f);

        // Setup UI components.

        LoopHorizontalScrollRect scrollRect = root.AddComponent<LoopHorizontalScrollRect>();
        scrollRect.content = contentRT;
        scrollRect.viewport = null;
        scrollRect.horizontalScrollbar = null;
        scrollRect.verticalScrollbar = null;
        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        scrollRect.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.verticalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.horizontalScrollbarSpacing = 0;
        scrollRect.verticalScrollbarSpacing = 0;
        root.AddComponent<UILoopScrollView>();

        RectTransform viewpointRt = viewpoint.GetComponent<RectTransform>();
        viewpointRt.anchorMin = new Vector2(0, 0);
        viewpointRt.anchorMax = new Vector2(1, 1);
        viewpointRt.sizeDelta = new Vector2(0, 0);
        viewpointRt.pivot = new Vector2(0.5f, 0.5f);
        viewpoint.AddComponent<Mask>();
        Image image = viewpoint.AddComponent<Image>();
        Color c = image.color;
        c.a = 1.0f / 255;
        image.color = c;

        HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = true;

        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        return root;
    }

    public static GameObject CreateLoopVerticalScrollRect()
    {
        GameObject root = CreateUIElementRoot("Loop Vertical Scroll Rect", new Vector2(200, 200));

        GameObject viewpoint = CreateUIObject("ViewPoint", root);
        GameObject content = CreateUIObject("Content", viewpoint);

        RectTransform contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0.5f, 1);
        contentRT.anchorMax = new Vector2(0.5f, 1);
        contentRT.sizeDelta = new Vector2(200, 0);
        contentRT.pivot = new Vector2(0.5f, 1);

        // Setup UI components.

        LoopVerticalScrollRect scrollRect = root.AddComponent<LoopVerticalScrollRect>();
        scrollRect.content = contentRT;
        scrollRect.viewport = null;
        scrollRect.horizontalScrollbar = null;
        scrollRect.verticalScrollbar = null;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.verticalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.horizontalScrollbarSpacing = 0;
        scrollRect.verticalScrollbarSpacing = 0;
        root.AddComponent<UILoopScrollView>();

        RectTransform viewpointRt = viewpoint.GetComponent<RectTransform>();
        viewpointRt.anchorMin = new Vector2(0, 0);
        viewpointRt.anchorMax = new Vector2(1, 1);
        viewpointRt.sizeDelta = new Vector2(0, 0);
        viewpointRt.pivot = new Vector2(0.5f, 0.5f);
        viewpoint.AddComponent<Mask>();
        Image image = viewpoint.AddComponent<Image>();
        Color c = image.color;
        c.a = 1.0f / 255;
        image.color = c;

        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return root;
    }

    #endregion


}
