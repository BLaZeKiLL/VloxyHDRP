using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Mesher;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine.Rendering;

namespace CodeBlaze.Vloxy.Engine.Jobs.Mesh {

    [BurstCompile]
    internal struct MeshBuildJob : IJobParallelFor {

        [ReadOnly] public int3 ChunkSize;
        [ReadOnly] public NativeArray<VertexAttributeDescriptor> VertexParams;

        [ReadOnly] public ChunkAccessor Accessor;
        [ReadOnly] public NativeList<int3> Jobs;

        [WriteOnly] public NativeParallelHashMap<int3, int2>.ParallelWriter Results;

        public UnityEngine.Mesh.MeshDataArray MeshDataArray;

        public void Execute(int index) {
            var mesh = MeshDataArray[index];
            var position = Jobs[index];

            var meshBuffer = GreedyMesher.GenerateMesh(Accessor, position, ChunkSize);
            
            // Vertex Buffer
            var vertexCount = meshBuffer.VertexBuffer.Length;

            mesh.SetVertexBufferParams(vertexCount, VertexParams);
            mesh.GetVertexData<Vertex>().CopyFrom(meshBuffer.VertexBuffer.AsArray());

            // Index Buffer
            var index0Count = meshBuffer.IndexBuffer0.Length;
            var index1Count = meshBuffer.IndexBuffer1.Length;
            var index2Count = meshBuffer.IndexBuffer2.Length;
            var index3Count = meshBuffer.IndexBuffer3.Length;
            
            mesh.SetIndexBufferParams(index0Count + index1Count + index2Count + index3Count, IndexFormat.UInt32);

            // Sub Mesh
            mesh.subMeshCount = 4;

            var indexBuffer = mesh.GetIndexData<int>();
            
            NativeArray<int>.Copy(meshBuffer.IndexBuffer0.AsArray(), 0, indexBuffer, 0, index0Count);
            var descriptor0 = new SubMeshDescriptor(0, index0Count);
            mesh.SetSubMesh(0, descriptor0, MeshUpdateFlags.DontRecalculateBounds);

            if (index1Count > 1) {
                NativeArray<int>.Copy(meshBuffer.IndexBuffer1.AsArray(), 0, indexBuffer, index0Count, index1Count);
                var descriptor = new SubMeshDescriptor(index0Count, index1Count);
                mesh.SetSubMesh(1, descriptor, MeshUpdateFlags.DontRecalculateBounds);
            }

            if (index2Count > 1) {
                NativeArray<int>.Copy(meshBuffer.IndexBuffer2.AsArray(), 0, indexBuffer, index0Count + index1Count, index2Count);
                var descriptor = new SubMeshDescriptor(index0Count + index1Count, index2Count);
                mesh.SetSubMesh(2, descriptor, MeshUpdateFlags.DontRecalculateBounds);
            }

            // if (index3Count > 1) {
            //     NativeArray<int>.Copy(meshBuffer.IndexBuffer3.AsArray(), 0, indexBuffer, index0Count + index1Count + index2Count, index3Count);
            //     var descriptor = new SubMeshDescriptor(index0Count + index1Count + index2Count, index3Count);
            //     mesh.SetSubMesh(3, descriptor, MeshUpdateFlags.DontRecalculateBounds);
            // }
            
            Results.TryAdd(position, new int2(index, vertexCount));

            meshBuffer.Dispose();
        }

    }

}