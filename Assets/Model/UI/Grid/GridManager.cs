using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public Canvas gridCanvas;
    public Transform grid;
    public GridCell cellPrefab;
    public CellPlunger cellPlungerPrefab;
    public Plunger plungerPrefab;
    public int cellCount;

    private List<GridCell> cells = new List<GridCell>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ClearCells();
        GenerageCells();
        GeneratePlungers(8);
        gridCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        gridCanvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnSchemaPlungers();
            ClearCells();
        }
    }

    public void SpawnSchemaPlungers()
    {
        foreach (var cell in cells)
        {
            if (cell.cellPlunger != null)
            {
                var plunger = Instantiate(plungerPrefab, cell.cellPlunger.transform.position, Quaternion.identity);
                plunger.plungerStrength = cell.cellPlunger.level;
                var (mesh, material) = SuckerManager.Instance.GetSucker(cell.cellPlunger.level);

                plunger.sucker1.meshFilter.mesh = mesh;
                plunger.sucker1.meshRenderer.material = material;
                plunger.sucker2.meshFilter.mesh = mesh;
                plunger.sucker2.meshRenderer.material = material;
            }
        }
    }

    private void DisableRaycasting()
    {
        var plungers = cells.Where(x => x.cellPlunger).Select(x => x.cellPlunger);

        foreach (var plunger in plungers)
        {
            plunger.DisableInteraction();
        }
    }

    private void EnableRaycasting()
    {
        var plungers = cells.Where(x => x.cellPlunger).Select(x => x.cellPlunger);

        foreach (var plunger in plungers)
        {
            plunger.EnableInteraction();
        }
    }

    private void ClearCells()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }

    private void GeneratePlungers(int plungersCount)
    {
        for (int i = 0; i < plungersCount; i++)
        {
            var plunger = Instantiate(cellPlungerPrefab);
            cells.Where(x => x.cellPlunger == null).GetRandom().PutPlunger(plunger);
            plunger.dragAndDrop.OnBeginDragEvent += DisableRaycasting;
            plunger.dragAndDrop.OnEndDragEvent += EnableRaycasting;
        }
    }

    private void GenerageCells()
    {
        for (int i = 0; i < cellCount; i++)
        {
            cells.Add(Instantiate(cellPrefab, grid));
        }
    }
}
