using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IDropHandler
{
    public CellPlunger cellPlunger;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null
            && eventData.pointerDrag.TryGetComponent(out CellPlunger plunger))
        {
            PutPlunger(plunger);
        }
    }

    public void PutPlunger(CellPlunger plunger)
    {
        if (cellPlunger == null || cellPlunger.level == plunger.level)
        {
            if (cellPlunger != null)
            {
                plunger.Upgrade();
                Destroy(cellPlunger.gameObject);
            }

            plunger.Release();
            cellPlunger = plunger;
            cellPlunger.transform.parent = _rectTransform.transform;
            cellPlunger.transform.localPosition = Vector3.zero;
            cellPlunger.OnRelease = ReleaseCell;
        }
    }

    public void ReleaseCell()
    {
        cellPlunger = null;
    }
}
