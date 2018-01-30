using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragView : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _cacheTransform;
    private Vector3 _offset;
    void Awake()
    {
        _cacheTransform = gameObject.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mouseWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_cacheTransform, eventData.position, eventData.pressEventCamera,
            out mouseWorldPos))
        {
            _offset = _cacheTransform.position - mouseWorldPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPos(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPos(eventData);
    }


    private void SetDraggedPos(PointerEventData eventData)
    {
        Vector3 mouseWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_cacheTransform, eventData.position, eventData.pressEventCamera,
            out mouseWorldPos))
        {
            _cacheTransform.position = mouseWorldPos + _offset;
        }
    }

}
