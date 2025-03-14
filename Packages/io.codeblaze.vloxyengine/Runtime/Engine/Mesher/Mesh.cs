﻿using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Engine.Mesher {

    [BurstCompile]
    public struct Vertex {

        public float3 Position;
        public float3 Normal;
        public float4 Color;
        public float3 UV0;
        public float2 UV1;
        public float4 UV2;

    }
    
    [BurstCompile]
    internal struct MeshBuffer {

        public NativeList<Vertex> VertexBuffer;
        public NativeList<int> IndexBuffer0; // Solid
        public NativeList<int> IndexBuffer1; // Water
        public NativeList<int> IndexBuffer2; // Transparent
        public NativeList<int> IndexBuffer3; // Folliage (X Mesh)

        internal void Dispose() {
            VertexBuffer.Dispose();
            IndexBuffer0.Dispose();
            IndexBuffer1.Dispose();
            IndexBuffer2.Dispose();
            IndexBuffer3.Dispose();
        }

    }

}