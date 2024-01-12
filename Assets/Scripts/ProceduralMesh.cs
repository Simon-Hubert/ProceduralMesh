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
    Mesh mesh;

    private void Awake() {
        mesh = new Mesh{
            name = "Procedural Mesh"
        };
        GenerateMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateMesh()
    {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        MeshJob<SquareGrid, SingleStream>.ScheduleParallel(meshData, default).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}
