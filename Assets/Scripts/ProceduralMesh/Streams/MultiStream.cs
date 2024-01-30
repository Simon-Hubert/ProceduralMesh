using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace ProceduralMeshes.Streams {
    public struct MultiStream : IMeshStreams
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float3> stream0, stream1;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float4> stream2;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float2> stream3;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<TriangleUInt16> triangles;

        public void Setup(Mesh.MeshData data, Bounds bounds, int vertexCount, int indexCount)
        {
            var descriptor = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );

            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
            descriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, stream: 2);
            descriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 3);
    
            data.SetVertexBufferParams(vertexCount, descriptor);
            descriptor.Dispose();

            data.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

            data.subMeshCount = 1;
            data.SetSubMesh(0, new SubMeshDescriptor(0, indexCount){
                bounds = bounds,
                vertexCount = vertexCount
            },
                MeshUpdateFlags.DontRecalculateBounds |
                MeshUpdateFlags.DontValidateIndices
            );

            stream0 = data.GetVertexData<float3>();
            stream1 = data.GetVertexData<float3>(1);
            stream2 = data.GetVertexData<float4>(2);
            stream3 = data.GetVertexData<float2>(3);
            triangles = data.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
        }

        public void SetTriangle(int index, int3 triangle) => triangles[index] = triangle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex data){
            stream0[index] = data.position;
            stream1[index] = data.normal;
            stream2[index] = data.tangent;
            stream3[index] = data.texCoord0;
        }
    }
}