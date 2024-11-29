using System;
using System.Collections.Generic;

using CodeBlaze.Vloxy.Engine.Behaviour;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;

#if VLOXY_LOGGING
using CodeBlaze.Vloxy.Engine.Utils.Logger;
#endif

using Priority_Queue;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Pool;

namespace CodeBlaze.Vloxy.Engine.Components {

    /// <summary>
    /// Chunks are created on demand
    /// </summary>
    public class ChunkPool {

        private readonly ObjectPool<ChunkBehaviour> _Pool;
        private readonly Dictionary<int3, ChunkBehaviour> _MeshMap;
        private readonly HashSet<int3> _ColliderSet;
        private readonly SimpleFastPriorityQueue<int3, int> _Queue;
        private readonly List<int3> _FocusUpdateReclaimList;

        private readonly int _ChunkPoolSize;
        private readonly int3 _DrawDistanceVec;

        private int3 _Focus;
        
        internal ChunkPool(Transform transform, VloxySettings settings) {
            _ChunkPoolSize = settings.Chunk.DrawDistance.CubedSize();
            _DrawDistanceVec = new int3(1, 1, 1) * settings.Chunk.DrawDistance;

            _MeshMap = new Dictionary<int3, ChunkBehaviour>(_ChunkPoolSize);
            _ColliderSet = new HashSet<int3>(settings.Chunk.ColliderDistance.CubedSize());
            _Queue = new SimpleFastPriorityQueue<int3, int>();

            _FocusUpdateReclaimList = new List<int3>();

            _Pool = new ObjectPool<ChunkBehaviour>( // pool size = x^2 + 1
                () => {
                    var go = new GameObject("Chunk", typeof(ChunkBehaviour)) {
                        transform = {
                            parent = transform
                        },
                    };

                    var collider = new GameObject("Collider", typeof(MeshCollider)) {
                        transform = {
                            parent = go.transform
                        },
                        tag = "Chunk"
                    };

                    go.SetActive(false);

                    var chunkBehaviour = go.GetComponent<ChunkBehaviour>();

                    chunkBehaviour.Init(settings.Renderer, collider.GetComponent<MeshCollider>());

                    return chunkBehaviour;
                },
                chunkBehaviour => chunkBehaviour.gameObject.SetActive(true),
                chunkBehaviour => chunkBehaviour.gameObject.SetActive(false),
                null, false, _ChunkPoolSize, _ChunkPoolSize
            );
            
#if VLOXY_LOGGING
            VloxyLogger.Info<ChunkPool>("Chunk Pool Size : " + _ChunkPoolSize);
#endif
        }

        internal bool IsActive(int3 pos) => _MeshMap.ContainsKey(pos);
        internal bool IsCollidable(int3 pos) => _ColliderSet.Contains(pos);

        internal void FocusUpdate(int3 focus) {
            _Focus = focus;

            foreach (var position in _Queue) {
                var should_reclaim = (_Focus - position > _DrawDistanceVec).OrReduce();

                // Reclaim far chunks, imp to do here due to air chunk skip
                if (should_reclaim) { 
                    _FocusUpdateReclaimList.Add(position);
                } else {
                    _Queue.UpdatePriority(position, - (position - _Focus).SqrMagnitude());
                }
            }

            if (_FocusUpdateReclaimList.Count > 0) {
                foreach (var position in _FocusUpdateReclaimList) {
                    _Queue.Remove(position);
                    ReclaimChunk(position);
                }

                _FocusUpdateReclaimList.Clear();
            }
        }
        
        internal ChunkBehaviour Claim(int3 position) {
            if (_MeshMap.ContainsKey(position)) {
                throw new InvalidOperationException($"Chunk ({position}) already active");
            }

            // Reclaim
            if (_Queue.Count >= _ChunkPoolSize)
            {
                var reclaim_position = _Queue.Dequeue();
                ReclaimChunk(reclaim_position);
            }

            // Claim
            var behaviour = _Pool.Get();
                
            behaviour.transform.position = position.GetVector3();
            behaviour.name = $"Chunk({position})";
                
            _MeshMap.Add(position, behaviour);
            _Queue.Enqueue(position, -(position - _Focus).SqrMagnitude());

            return behaviour;
        }

        private void ReclaimChunk(int3 reclaim_position)
        {
            var reclaim_behaviour = _MeshMap[reclaim_position];

            reclaim_behaviour.Collider.sharedMesh = null;

            _Pool.Release(reclaim_behaviour);
            _MeshMap.Remove(reclaim_position);
            _ColliderSet.Remove(reclaim_position);
        }

        internal Dictionary<int3, ChunkBehaviour> GetActiveMeshes(List<int3> positions) {
            var map = new Dictionary<int3, ChunkBehaviour>();
            
            for (int i = 0; i < positions.Count; i++) {
                var position = positions[i];
                
                if (IsActive(position)) map.Add(position, _MeshMap[position]);
            }

            return map;
        }

        internal void ColliderBaked(int3 position) {
            _ColliderSet.Add(position);
        }

        internal ChunkBehaviour Get(int3 position) {
            if (!_MeshMap.ContainsKey(position)) {
                throw new InvalidOperationException($"Chunk ({position}) isn't active");
            }

            return _MeshMap[position];
        }

    }

}