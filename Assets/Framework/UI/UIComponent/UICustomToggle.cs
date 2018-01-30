using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Next项目使用的toggle
/// 两个状态，对应两个sprite
/// </summary>
[RequireComponent(typeof(Toggle))]
public class UICustomToggle : MonoBehaviour
{
    public Toggle toggle;
    public Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private bool useNativeSize;
    private Sprite originSprite;

    void Awake()
    {
        if (toggle != null && image != null)
        {
            originSprite = image.sprite;
            toggle.onValueChanged.AddListener(OnToggleChange);
        }
    }

    void OnDestroy()
    {
        if(toggle!=null)
            toggle.onValueChanged.RemoveListener(OnToggleChange);
    }

    void OnToggleChange(bool flag)
    {
        if (image != null)
            image.sprite = flag ? onSprite : originSprite;
        if (useNativeSize)
            image.SetNativeSize();
    }


}