using TheNextMoba.UI;
using UnityEngine;
using UnityEngine.UI;

public class OpenWindow : MonoBehaviour
{
    [SerializeField] private string winName;
    [SerializeField] private string url;
    [SerializeField] private Button button;

    void Start()
    {
        button.onClick.AddListener(OnClickBtn);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickBtn);
    }

    void OnClickBtn()
    {
        if (!string.IsNullOrEmpty(winName))
        {
            UIManager.Instance.ShowWindow(winName);
        }
    }
}
