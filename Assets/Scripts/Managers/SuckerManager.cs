using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerManager : MonoBehaviour
{
    public static SuckerManager Instance;

    public List<Mesh> meshes;
    public List<Material> materials;

    private void Awake()
    {
        Instance = this;
    }

    public (Mesh mesh, Material material) GetSucker(int level)
    {
        var mesh = meshes[level % meshes.Count];
        var material = materials[level % materials.Count];
        return (mesh, material);
    }
}
