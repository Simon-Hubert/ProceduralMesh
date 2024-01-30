using System;
using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    static MeshJobScheduleDelegate[] jobs = {
        MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel
    };

    public enum MeshType {
        SquareGrid, SharedSquareGrid
    };

    [SerializeField, Range(1,50)]
    int resolution = 1;

    [SerializeField]
    MeshType meshType;

    Mesh mesh;

    private void Awake() {
        mesh = new Mesh{
            name = "Procedural Mesh"
        };
        //GenerateMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateMesh()
    {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];
        
        jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }

    private void OnValidate() => enabled = true;

    void Update() {
        GenerateMesh();
        enabled = false;
    }
}
