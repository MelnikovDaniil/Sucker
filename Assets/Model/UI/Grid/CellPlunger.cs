using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellPlunger : MonoBehaviour
{
    public Action OnRelease;
    public int level;
    public TextMeshProUGUI levelText;

    public List<MeshRenderer> opacityMeshRenderes;

    [NonSerialized]
    public DragAndDrop dragAndDrop;

    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
        dragAndDrop = GetComponent<DragAndDrop>();
        dragAndDrop.OnBeginDragEvent += () => SetOpacity(0.6f);
        dragAndDrop.OnEndDragEvent   += () => SetOpacity(1f);
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

    public void SetOpacity(float opacity)
    {
        foreach (var renderer in opacityMeshRenderes)
        {
            if (opacity < 1f)
            {
                renderer.material.ToFadeMode();
                var newColor = renderer.material.color;
                newColor = new Color(newColor.r, newColor.g, newColor.b, opacity);
                renderer.material.color = newColor;
            }
            else
            {
                renderer.material.ToOpaqueMode();
            }
        }
    }
}
