using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler,
    IDropHandler
{
    public event Action OnBeginDragEvent;
    public event Action OnEndDragEvent;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private Vector2 previousPosition;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        OnBeginDragEvent?.Invoke();
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
        previousPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        _rectTransform.anchoredPosition += 
            eventData.delta / GridManager.Instance.gridCanvas.scaleFactor; 
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null
            || !eventData.pointerEnter.TryGetComponent(out GridCell cell)
            || cell.cellPlunger.level != GetComponent<CellPlunger>().level)
        {
            _rectTransform.anchoredPosition = previousPosition;
        }

        OnEndDragEvent?.Invoke();
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}
