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

namespace CodeBlaze.Vloxy.Engine.Jobs {
    
    public class VloxyScheduler {
        
        private readonly MeshBuildScheduler _MeshBuildScheduler;
        private readonly ColliderBuildScheduler _ColliderBuildScheduler;

        private readonly IChunkManager _TopLayer;
        private readonly ChunkPool _ChunkPool;

        private readonly SimpleFastPriorityQueue<int3, int> _ViewQueue;
        private readonly SimpleFastPriorityQueue<int3, int> _DataQueue;
        private readonly SimpleFastPriorityQueue<int3, int> _ColliderQueue;

        private readonly HashSet<int3> _ViewSet;
        private readonly HashSet<int3> _DataSet;
        private readonly HashSet<int3> _ColliderSet;

        private readonly VloxySettings _Settings;

        internal VloxyScheduler(
            VloxySettings settings, 
            MeshBuildScheduler meshBuildScheduler,
            ColliderBuildScheduler colliderBuildScheduler,
            ChunkPool chunkPool,
            IChunkManager topLayer
        ) {
            _MeshBuildScheduler = meshBuildScheduler;
            _ColliderBuildScheduler = colliderBuildScheduler;

            _ChunkPool = chunkPool;
            _TopLayer = topLayer;

            _ViewQueue = new SimpleFastPriorityQueue<int3, int>();
            _DataQueue = new SimpleFastPriorityQueue<int3, int>();
            _ColliderQueue = new SimpleFastPriorityQueue<int3, int>();

            _ViewSet = new HashSet<int3>();
            _DataSet = new HashSet<int3>();
            _ColliderSet = new HashSet<int3>();

            _Settings = settings;
        }

        // Priority Updates for Reclaim
        // At max 2 Queues are updated in total (ViewReclaimQueue, DataReclaimQueue)
        internal void FocusChunkUpdate(int3 focus_chunk_coords) {
            // _ChunkManager.FocusChunkUpdate(focus_chunk_coords);
            _ChunkPool.FocusChunkUpdate(focus_chunk_coords);
        }

        internal void SchedulerUpdate2(GridBounds diff, int3 focus) {
            var chunk_positions = _TopLayer.GetChunksInBounds(diff);

            foreach (var chunk_pos in chunk_positions) {
                if (_ViewQueue.Contains(chunk_pos)) {
                    _ViewQueue.UpdatePriority(chunk_pos, (chunk_pos - focus).SqrMagnitude());
                } else if (ShouldScheduleForMeshing2(chunk_pos) && CanGenerateMeshForChunk2(chunk_pos)) {
                    _ViewQueue.Enqueue(chunk_pos, (chunk_pos - focus).SqrMagnitude());
                }
            }
        }

        // TODO : This thing takes 4ms every frame need to make a reactive system and maybe try the fast queue
        // At max 3 Queues are updated in total (ViewQueue, DataQueue, ColliderQueue)
        // internal void SchedulerUpdate(int3 focus) {
        //     var load = _Settings.Chunk.LoadDistance;
        //     var draw = _Settings.Chunk.DrawDistance;
        //     var update = _Settings.Chunk.ColliderDistance;

        //     // TODO : This is a quick little optimization for out of generation bounds pruning, +1 for meshing
        //     var y_load = math.min(load, ((_Settings.Noise.Height / 2) / _Settings.Chunk.ChunkSize.y) + 1);

        //     for (var x = -load; x <= load; x++) {
        //         for (var z = -load; z <= load; z++) {
        //             // Y can be contained between heigh limits instead of load limits for faster 2D world
        //             for (var y = -y_load; y <= y_load; y++) {
        //                 var pos = focus + _Settings.Chunk.ChunkSize.MemberMultiply(x, y, z);

        //                 if (
        //                     (x >= -draw && x <= draw) &&
        //                     (y >= -draw && y <= draw) &&
        //                     (z >= -draw && z <= draw)
        //                 ) {
        //                     if (_ViewQueue.Contains(pos)) {
        //                         _ViewQueue.UpdatePriority(pos, (pos - focus).SqrMagnitude());
        //                     } else if (ShouldScheduleForMeshing(pos) && CanGenerateMeshForChunk(pos)) {
        //                         _ViewQueue.Enqueue(pos, (pos - focus).SqrMagnitude());
        //                     }
        //                 }
                        
        //                 if (
        //                     (x >= -update && x <= update) &&
        //                     (y >= -update && y <= update) &&
        //                     (z >= -update && z <= update)
        //                 ) {
        //                     if (_ColliderQueue.Contains(pos)) {
        //                         _ColliderQueue.UpdatePriority(pos, (pos - focus).SqrMagnitude());
        //                     } else if (ShouldScheduleForBaking(pos) && CanBakeColliderForChunk(pos)) {
        //                         _ColliderQueue.Enqueue(pos, (pos - focus).SqrMagnitude());
        //                     }
        //                 }

        //                 if (_DataQueue.Contains(pos)) {
        //                     _DataQueue.UpdatePriority(pos, (pos - focus).SqrMagnitude());
        //                 } else if (ShouldScheduleForGenerating(pos)) {
        //                     _DataQueue.Enqueue(pos, (pos - focus).SqrMagnitude());
        //                 }
        //             }
        //         }
        //     }
        // }

        internal void JobUpdate() {
            // if (_DataQueue.Count > 0 && _ChunkScheduler.IsReady) {
            //     var count = math.min(_Settings.Scheduler.StreamingBatchSize, _DataQueue.Count);
                
            //     for (var i = 0; i < count; i++) {
            //         _DataSet.Add(_DataQueue.Dequeue());
            //     }
                
            //     _ChunkScheduler.Start(_DataSet.ToList());
            // }  
            
            if (_ViewQueue.Count > 0 && _MeshBuildScheduler.IsReady) {
                var count = math.min(_Settings.Scheduler.MeshingBatchSize, _ViewQueue.Count);
                
                for (var i = 0; i < count; i++) {
                    var chunk = _ViewQueue.Dequeue();
                    
                    // The chunk may be removed from memory by the time we schedule,
                    // Should we check this only here ?
                    if (CanGenerateMeshForChunk2(chunk)) _ViewSet.Add(chunk);
                }

                _MeshBuildScheduler.Start(_ViewSet.ToList());
            }

            if (_ColliderQueue.Count > 0 && _ColliderBuildScheduler.IsReady) {
                var count = math.min(_Settings.Scheduler.ColliderBatchSize, _ColliderQueue.Count);

                for (var i = 0; i < count; i++) {
                    var position = _ColliderQueue.Dequeue();

                    if (CanBakeColliderForChunk(position)) _ColliderSet.Add(position);
                }
                
                _ColliderBuildScheduler.Start(_ColliderSet.ToList());
            }
        }

        internal void SchedulerLateUpdate() {
            // if (_ChunkScheduler.IsComplete && !_ChunkScheduler.IsReady) {
            //     _ChunkScheduler.Complete();
            //     _DataSet.Clear();
            // }
            
            if (_MeshBuildScheduler.IsComplete && !_MeshBuildScheduler.IsReady) {
                _MeshBuildScheduler.Complete();
                _ViewSet.Clear();
            }

            if (_ColliderBuildScheduler.IsComplete && !_ColliderBuildScheduler.IsReady) {
                _ColliderBuildScheduler.Complete();
                _ColliderSet.Clear();
            }
        }

        internal void Dispose() {
            _MeshBuildScheduler.Dispose();
            _ColliderBuildScheduler.Dispose();
        }

        private bool ShouldScheduleForMeshing2(int3 position)
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
        private bool CanGenerateMeshForChunk2(int3 position) {
            var result = true;
            
            for (var x = -1; x <= 1; x++) {
                for (var z = -1; z <= 1; z++) {
                    var pos = position + _Settings.Chunk.ChunkSize.MemberMultiply(x, 0, z);
                    result &= _TopLayer.IsChunkLoaded(pos);
                }
            }

            return result;
        }

        private bool CanBakeColliderForChunk(int3 position) => _ChunkPool.IsActive(position);

        #region RuntimeStatsAPI

        public float DataAvgTiming => 0f;
        public float MeshAvgTiming => _MeshBuildScheduler.AvgTime;
        public float BakeAvgTiming => _ColliderBuildScheduler.AvgTime;

        public int DataQueueCount => _DataQueue.Count;
        public int MeshQueueCount => _ViewQueue.Count;
        public int BakeQueueCount => _ColliderQueue.Count;

        #endregion

    }

}