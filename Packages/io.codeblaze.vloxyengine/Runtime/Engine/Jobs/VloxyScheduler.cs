using System.Collections.Generic;
using System.Linq;

using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Jobs.Collider;
using CodeBlaze.Vloxy.Engine.Jobs.Mesh;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Priority_Queue;
using Runevision.Common;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Jobs
{

    public class VloxyScheduler
    {

        private readonly MeshBuildScheduler _MeshBuildScheduler;
        private readonly ColliderBuildScheduler _ColliderBuildScheduler;

        private readonly IChunkManager _ChunkManager;
        private readonly ChunkPool _ChunkPool;

        private readonly SimpleFastPriorityQueue<int3, int> _ViewQueue;
        private readonly SimpleFastPriorityQueue<int3, int> _ColliderQueue;

        private readonly HashSet<int3> _ViewSet;
        private readonly HashSet<int3> _ColliderSet;

        private readonly VloxySettings _Settings;

        internal VloxyScheduler(
            VloxySettings settings,
            MeshBuildScheduler meshBuildScheduler,
            ColliderBuildScheduler colliderBuildScheduler,
            ChunkPool chunkPool,
            IChunkManager chunkManager
        )
        {
            _MeshBuildScheduler = meshBuildScheduler;
            _ColliderBuildScheduler = colliderBuildScheduler;

            _ChunkPool = chunkPool;
            _ChunkManager = chunkManager;

            _ViewQueue = new SimpleFastPriorityQueue<int3, int>();
            _ColliderQueue = new SimpleFastPriorityQueue<int3, int>();

            _ViewSet = new HashSet<int3>();
            _ColliderSet = new HashSet<int3>();

            _Settings = settings;
        }

        internal void FocusUpdate(int3 focus, GridBounds new_diff, GridBounds old_diff)
        {
            if (old_diff.min != Point.one * int.MinValue)
            {
                var old_chunk_positions = _ChunkManager.GetChunkPositionsInBounds(old_diff);
                _ChunkPool.ReclaimChunks(old_chunk_positions);
            }

            var new_chunk_positions = _ChunkManager.GetChunkPositionsInBounds(new_diff);

            foreach (var chunk_pos in new_chunk_positions)
            {
                if (_ViewQueue.Contains(chunk_pos))
                {
                    _ViewQueue.UpdatePriority(chunk_pos, (chunk_pos - focus).SqrMagnitude());
                }
                else if (ShouldScheduleForMeshing(chunk_pos) /*&& CanGenerateMeshForChunk(chunk_pos)*/)
                {
                    _ViewQueue.Enqueue(chunk_pos, (chunk_pos - focus).SqrMagnitude());
                }
            }

        }

        internal void JobUpdate(GridBounds current_update_bound)
        {
            if (_ViewQueue.Count > 0 && _MeshBuildScheduler.IsReady)
            {
                var count = math.min(_Settings.Scheduler.MeshingBatchSize, _ViewQueue.Count);
                for (var i = 0; i < count; i++)
                {
                    var chunk = _ViewQueue.First;

                    // Remove chunks no longer in current bound
                    if (!current_update_bound.Contains(chunk.Point()))
                    {
                        _ViewQueue.Dequeue();
                        continue;
                    }

                    // The chunk may be removed from memory by the time we schedule,
                    // Should we check this only here ?
                    if (CanGenerateMeshForChunk(chunk))
                    {
                        _ViewQueue.Dequeue();
                        _ViewSet.Add(chunk);
                    }
                    // else
                    // {
                    //     VloxyLogger.Warn<VloxyScheduler>($"Can't Mesh Chunk {chunk}");
                    // }
                }

                _MeshBuildScheduler.Start(_ViewSet.ToList());
            }

            if (_ColliderQueue.Count > 0 && _ColliderBuildScheduler.IsReady)
            {
                var count = math.min(_Settings.Scheduler.ColliderBatchSize, _ColliderQueue.Count);

                for (var i = 0; i < count; i++)
                {
                    var position = _ColliderQueue.Dequeue();

                    if (CanBakeColliderForChunk(position)) _ColliderSet.Add(position);
                }

                _ColliderBuildScheduler.Start(_ColliderSet.ToList());
            }
        }

        internal void LateUpdate()
        {
            if (_MeshBuildScheduler.IsComplete && !_MeshBuildScheduler.IsReady)
            {
                _MeshBuildScheduler.Complete();
                _ViewSet.Clear();
            }

            if (_ColliderBuildScheduler.IsComplete && !_ColliderBuildScheduler.IsReady)
            {
                _ColliderBuildScheduler.Complete();
                _ColliderSet.Clear();
            }
        }

        internal void Dispose()
        {
            _ChunkManager.Dispose();
            _MeshBuildScheduler.Dispose();
            _ColliderBuildScheduler.Dispose();
        }

        private bool ShouldScheduleForMeshing(int3 position)
        {
            return
            // Should not be active or should be marked for re-build
            (!_ChunkPool.IsActive(position) /* || _ChunkManager.ShouldReMesh(position), need to handle this somewhere */)
            // Should not be scheduled in a job 
            && !_ViewSet.Contains(position);
        }

        // private bool ShouldScheduleForBaking(int3 position)
        // {
        //     return (!_ChunkPool.IsCollidable(position) || _ChunkManager.ShouldReCollide(position)) && !_ColliderSet.Contains(position);
        // }

        /// <summary>
        /// Checks if the specified chunks and it's neighbors are generated
        /// </summary>
        /// <param name="position">Position of chunk to check</param>
        /// <returns>Is it ready to be meshed</returns>
        private bool CanGenerateMeshForChunk(int3 position)
        {
            var result = true;

            for (var x = -1; x <= 1; x++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    var pos = position + _Settings.Chunk.ChunkSize.MemberMultiply(x, 0, z);
                    result &= _ChunkManager.IsChunkLoaded(pos);
                }
            }

            return result;
        }

        private bool CanBakeColliderForChunk(int3 position) => _ChunkPool.IsActive(position);

        #region RuntimeStatsAPI

        public float DataAvgTiming => 0f;
        public float MeshAvgTiming => _MeshBuildScheduler.AvgTime;
        public float BakeAvgTiming => _ColliderBuildScheduler.AvgTime;

        public int DataQueueCount => 0;
        public int MeshQueueCount => _ViewQueue.Count;
        public int BakeQueueCount => _ColliderQueue.Count;

        #endregion

    }

}