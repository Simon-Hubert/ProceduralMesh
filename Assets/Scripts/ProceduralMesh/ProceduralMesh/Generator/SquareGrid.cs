using UnityEngine;
using Unity.Mathematics;

using static Unity.Mathematics.math;
using Unity.VisualScripting;

namespace ProceduralMeshes.Generators{
    public struct SquareGrid : IMeshGenerator
    {
        public int Resolution {get; set;}

        public int VertexCount => 4 * Resolution * Resolution;

        public int IndexCount => 6 * Resolution * Resolution;

        public int JobLength => Resolution;

        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams {
            int vi = 4*Resolution*z, ti = 2*Resolution*z;

            for (int x=0; x<Resolution;x++, vi +=4, ti +=2){
                var xcoordinates = float2(x, x+1f)/Resolution - .5f;
                var zcoordinates = float2(z,z+1f)/Resolution - .5f;

                var vertex = new Vertex();
                vertex.position.x = xcoordinates.x;
                vertex.position.z = zcoordinates.x;
                vertex.normal.y = 1f;
                vertex.tangent.xw = float2(1f, -1f);
                streams.SetVertex(vi, vertex);

                vertex.position.x = xcoordinates.y;
                vertex.texCoord0 = float2(1f, 0f);
                streams.SetVertex(vi+1, vertex);

                vertex.position.x = xcoordinates.x;
                vertex.position.z = zcoordinates.y;
                vertex.texCoord0 = float2(0f,1f);
                streams.SetVertex(vi+2, vertex);

                vertex.position.x = xcoordinates.y;
                vertex.texCoord0 = 1f;
                streams.SetVertex(vi+3, vertex);

                streams.SetTriangle(ti, vi+int3(0,2,1));
                streams.SetTriangle(ti+1, vi+int3(1,2,3));
            }
        }

        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f,0f,1f));
    }
}
