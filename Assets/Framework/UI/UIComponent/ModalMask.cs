using UnityEngine;
using UnityEngine.EventSystems;

namespace TheNextMoba.UI
{
    /// <summary>
    /// 放于弹出框父节点下
    /// 屏蔽点击事件
    /// 勾选clickClose，那么点击mask关闭ui
    /// </summary>
    public class ModalMask:UIBehaviour,IPointerClickHandler
    {
        [SerializeField] private bool clickClose;

        protected override void Awake()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickClose)
            {
                string parentName = transform.parent.name;
                parentName = parentName.Replace("Bind", "");
                UIManager.Instance.CloseWindow(parentName);
            }
        }

    }
}
