using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellPlunger : MonoBehaviour
{
    public Action OnRelease;
    public int level;
    public TextMeshProUGUI levelText;

    [NonSerialized]
    public DragAndDrop dragAndDrop;

    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
        dragAndDrop = GetComponent<DragAndDrop>();
    }

    private void Start()
    {
        levelText.text = level.ToString();
    }

    public void Release()
    {
        OnRelease?.Invoke();
    }

    public void Upgrade()
    {
        level++;
        levelText.text = level.ToString();
    }

    public void DisableInteraction()
    {
        icon.raycastTarget = false;
    }

    public void EnableInteraction()
    {
        icon.raycastTarget = true;
    }
}
