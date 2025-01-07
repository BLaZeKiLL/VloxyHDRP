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

namespace CodeBlaze.Vloxy.Engine.Components
{
    /// <summary>
    /// Chunks are created on demand
    /// </summary>
    public class ChunkPool
    {
        private readonly ObjectPool<ChunkBehaviour> _Pool;
        private readonly Dictionary<int3, ChunkBehaviour> _MeshMap;
        private readonly HashSet<int3> _ColliderSet;
        private readonly HashSet<int3> _ReclaimSet;

        private readonly int _ChunkPoolSize;

        internal ChunkPool(Transform transform, VloxySettings settings)
        {
            _ChunkPoolSize = settings.Chunk.DrawDistance.XZSize();

            _ReclaimSet = new HashSet<int3>();
            _MeshMap = new Dictionary<int3, ChunkBehaviour>(_ChunkPoolSize);
            _ColliderSet = new HashSet<int3>(settings.Chunk.ColliderDistance.XZSize());

            _Pool = new ObjectPool<ChunkBehaviour>( // pool size = x^2 + 1
                () =>
                {
                    var go = new GameObject("Chunk", typeof(ChunkBehaviour))
                    {
                        transform = {
                            parent = transform
                        },
                    };

                    var collider = new GameObject("Collider", typeof(MeshCollider))
                    {
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

        internal void ReclaimChunks(List<int3> positions)
        {
            ProcessReclaimSet(); // Pending Reclaims

            foreach (var chunk_position in positions)
            {
                if (_MeshMap.ContainsKey(chunk_position))
                    ReclaimChunk(chunk_position);
                else
                    _ReclaimSet.Add(chunk_position);
            }
        }

        internal ChunkBehaviour Claim(int3 position)
        {
            if (_MeshMap.ContainsKey(position))
            {
                throw new InvalidOperationException($"Chunk ({position}) already active");
            }

            // Claim
            var behaviour = _Pool.Get();

            behaviour.transform.position = position.GetVector3();
            behaviour.name = $"Chunk({position})";

            _MeshMap.Add(position, behaviour);

            return behaviour;
        }

        internal Dictionary<int3, ChunkBehaviour> GetActiveMeshes(List<int3> positions)
        {
            var map = new Dictionary<int3, ChunkBehaviour>();

            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];

                if (IsActive(position)) map.Add(position, _MeshMap[position]);
            }

            return map;
        }

        internal void ColliderBaked(int3 position)
        {
            _ColliderSet.Add(position);
        }

        internal ChunkBehaviour Get(int3 position)
        {
            if (!_MeshMap.ContainsKey(position))
            {
                throw new InvalidOperationException($"Chunk ({position}) isn't active");
            }

            return _MeshMap[position];
        }

        private void ProcessReclaimSet() {
            var remove_list = new List<int3>(_ReclaimSet.Count);

            foreach (var positon in _ReclaimSet) {
                if (_MeshMap.ContainsKey(positon)) {
                    ReclaimChunk(positon);
                    remove_list.Add(positon);
                }
            }

            foreach (var position in remove_list) {
                _ReclaimSet.Remove(position);
            }
        }

        private void ReclaimChunk(int3 reclaim_chunk_position)
        {
            var reclaim_behaviour = _MeshMap[reclaim_chunk_position];

            reclaim_behaviour.Collider.sharedMesh = null;

            _Pool.Release(reclaim_behaviour);
            _MeshMap.Remove(reclaim_chunk_position);
            _ColliderSet.Remove(reclaim_chunk_position);
        }

    }

}