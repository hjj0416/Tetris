using UnityEngine;
using UnityEngine.EventSystems;

namespace TheNextMoba.UI
{
    /// <summary>
    /// ���ڵ����򸸽ڵ���
    /// ���ε���¼�
    /// ��ѡclickClose����ô���mask�ر�ui
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
