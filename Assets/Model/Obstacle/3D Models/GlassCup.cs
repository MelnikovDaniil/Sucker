using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassCup : MonoBehaviour
{
    public GameObject toothBrushPrefab;
    public Vector2 spawnArea;
    public Vector2 minMaxBrushes;

    // Start is called before the first frame update
    void Start()
    {
        var brushAmount = Random.Range(minMaxBrushes.x, minMaxBrushes.y);
        for (int i = 0; i < brushAmount; i++)
        {
            var x = Random.Range(-spawnArea.x, spawnArea.x) / 2f;
            var y = Random.Range(-spawnArea.y, spawnArea.y) / 2f;
            var rotation = Quaternion.Euler(0, Random.Range(-180, 0), Random.Range(-5, 5));
            var brush = Instantiate(toothBrushPrefab, transform.rotation * new Vector3(x, y) + transform.position, Quaternion.identity);
            brush.transform.rotation = transform.rotation;
            //brush.transform.GetChild(0).rotation = rotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.rotation * spawnArea);
    }
}
