using System.Collections.Generic;

using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Jobs.Core;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine.Rendering;

namespace CodeBlaze.Vloxy.Engine.Jobs.Mesh
{

    public class MeshBuildScheduler : JobScheduler
    {

        private readonly ChunkManager _ChunkManager;
        private readonly ChunkPool _ChunkPool;

        private int3 _ChunkSize;
        private JobHandle _Handle;

        private NativeList<int3> _Jobs;
        private ChunkAccessor _ChunkAccessor;
        private NativeParallelHashMap<int3, int2> _Results;
        private UnityEngine.Mesh.MeshDataArray _MeshDataArray;
        private NativeArray<VertexAttributeDescriptor> _VertexParams;
        private NativeParallelHashMap<int3, Chunk> _AccessorMap;

        public MeshBuildScheduler(
            VloxySettings settings,
            ChunkPool chunkPool,
            ChunkManager chunkManager
        )
        {
            _ChunkManager = chunkManager;
            _ChunkPool = chunkPool;

            _ChunkSize = settings.Chunk.ChunkSize;

            // TODO : Make Configurable (Source Generators)
            _VertexParams = new NativeArray<VertexAttributeDescriptor>(6, Allocator.Persistent);

            // Int interpolation cause issues
            _VertexParams[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
            _VertexParams[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);
            _VertexParams[2] = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4);
            _VertexParams[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 3);
            _VertexParams[4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 2);
            _VertexParams[5] = new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4);

            _AccessorMap = new NativeParallelHashMap<int3, Chunk>(
                (settings.Scheduler.MeshingBatchSize + 1) * (settings.Scheduler.MeshingBatchSize + 1),
                Allocator.Persistent
            );

            _Results = new NativeParallelHashMap<int3, int2>(settings.Scheduler.MeshingBatchSize, Allocator.Persistent);
            _Jobs = new NativeList<int3>(Allocator.Persistent);
        }

        internal bool IsReady = true;
        internal bool IsComplete => _Handle.IsCompleted;

        internal void Start(List<int3> jobs)
        {
            StartRecord();

            IsReady = false;

            _AccessorMap.Clear();

            _ChunkManager.PopulateChunkAccessor(jobs, _AccessorMap);

            _ChunkAccessor = new ChunkAccessor(_AccessorMap.AsReadOnly(), _ChunkSize);

            foreach (var j in jobs)
            {
                _Jobs.Add(j);
            }

            _MeshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(_Jobs.Length);

            var job = new MeshBuildJob
            {
                Accessor = _ChunkAccessor,
                ChunkSize = _ChunkSize,
                Jobs = _Jobs,
                VertexParams = _VertexParams,
                MeshDataArray = _MeshDataArray,
                Results = _Results.AsParallelWriter()
            };

            _Handle = job.Schedule(_Jobs.Length, 1);
        }

        internal void Complete()
        {
            _Handle.Complete();

            var meshes = new UnityEngine.Mesh[_Jobs.Length];

            for (var index = 0; index < _Jobs.Length; index++)
            {
                var position = _Jobs[index];

                // TODO More state handling is required for this optimization as the main schedular keeps trying to build empty meshes forever, since they are valid positions and don't become active
                // if (_Results[position].y == 0) // Empty mesh
                // {
                //     // Let's hope these are cleaned fast, or we can have a single empty mesh reference
                //     meshes[_Results[position].x] = new UnityEngine.Mesh();

                //     // Chunk pool maintains active empty meshing to satisfy valid positions, need to handle re-mesh case
                //     _ChunkPool.MarkEmptyMesh();
                //     continue;
                // }

                // TODO ReMesh
                // if (_ChunkManager.ReMeshedChunk(position))
                // {
                //     meshes[_Results[position].x] = _ChunkPool.Get(position).Mesh;
                // }
                // else
                // {
                meshes[_Results[position].x] = _ChunkPool.Claim(position).Mesh;
                // }
            }

            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(
                _MeshDataArray,
                meshes,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices
            );

            for (var index = 0; index < meshes.Length; index++)
            {
                if (meshes[index].vertexCount > 0)
                {
                    meshes[index].RecalculateBounds();
                }
                // else
                // {
                //     VloxyLogger.Info<MeshBuildScheduler>("Empty Mesh");
                // }
            }

            _Results.Clear();
            _Jobs.Clear();

            IsReady = true;

            StopRecord();
        }

        internal void Dispose()
        {
            _Handle.Complete();

            _VertexParams.Dispose();
            _AccessorMap.Dispose();
            _Results.Dispose();
            _Jobs.Dispose();
        }

    }

}