using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Priority_Queue;
using Runevision.Common;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Components {

    public class ChunkManager
    {
        private readonly ConcurrentDictionary<int3, NativeReference<Chunk>> Chunks;

        private readonly HashSet<int3> ReadyChunks; // TODO : Thread safety required ?

        private int3 ChunkSize;

        internal ChunkManager(VloxySettings settings) {
            ChunkSize = settings.Chunk.ChunkSize;

            var size = settings.Chunk.DrawDistance.XZSize();

            Chunks = new ConcurrentDictionary<int3, NativeReference<Chunk>>(Environment.ProcessorCount * 2, size);

            ReadyChunks = new HashSet<int3>(size);
        }

        #region API

        public int ChunkCount() => Chunks.Count;

        public void AddChunk(int3 position, NativeReference<Chunk> chunk) {
            Chunks[position] = chunk;
        }

        public void MarkChunkReady(int3 postion) {
            ReadyChunks.Add(postion);
        }

        public NativeReference<Chunk> GetChunkUnsafe(int3 position) => Chunks[position];

        public bool IsChunkLoaded(int3 position) => ReadyChunks.Contains(position);

        public void MarkUnready(int3 position) {
            ReadyChunks.Remove(position);
        }

        public void DisposeChunk(int3 position) {
            Chunks.Remove(position, out var chunk);
            chunk.Dispose(); // TODO : Check dispose
        }

        #endregion

        #region Internal

        internal void Dispose()
        {
            foreach (var (_, chunk) in Chunks) {
                chunk.Dispose();
            }
        }

        internal List<int3> GetChunkPositionsInBounds(GridBounds bounds)
        {
            List<int3> chunks = new();

            Point point = new(Crd.Div(bounds.min.x, ChunkSize.x), Crd.Div(bounds.min.y, ChunkSize.z));
            Point point2 = new(Crd.DivUp(bounds.max.x, ChunkSize.x), Crd.DivUp(bounds.max.y, ChunkSize.z));

            for (int x = point.x; x < point2.x; x++)
            {
                for (int z = point.y; z < point2.y; z++)
                {
                    chunks.Add(ChunkSize.MemberMultiply(x, 0, z));
                }
            }

            return chunks;
        }

        internal void PopulateChunkAccessor(List<int3> positions, NativeParallelHashMap<int3, Chunk> chunk_map)
        {
            foreach (var position in positions)
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        var pos = position + ChunkSize.MemberMultiply(x, 0, z);

                        if (!IsChunkLoaded(pos))
                        {
                            // Anytime this exception is thrown, mesh building completely stops
                            throw new InvalidOperationException($"Chunk {pos} has not been generated");
                        }

                        var raster_chunk = Chunks[pos];

                        if (!chunk_map.ContainsKey(pos))
                        {
                            chunk_map.Add(pos, raster_chunk.Value);
                        }
                    }
                }
            }
        }

        #endregion

        # region Block API

        public int3 ResolveChunkPos(ref int3 block_pos)
        {
            var key = int3.zero;

            for (var index = 0; index < 3; index++)
            {
                if (block_pos[index] >= 0 && block_pos[index] < ChunkSize[index]) continue;

                key[index] += block_pos[index] < 0 ? -1 : 1;
                block_pos[index] -= key[index] * ChunkSize[index];
            }

            key *= ChunkSize;

            return key;
        }

        public int GetBlockLocal(int3 chunk_pos, int3 block_pos)
        {
            int3 key = ResolveChunkPos(ref block_pos);

            try
            {
                return GetChunkUnsafe(chunk_pos + key).Value.GetBlock(block_pos);
            }
            catch
            {
                VloxyLogger.Error<ChunkManager>($"Error getting ref ({chunk_pos + key}) for chunk ({chunk_pos})");
                return (int)Block.ERROR;
            }
        }

        public void SetBlockLocal(int3 chunk_pos, int3 block_pos, int block)
        {
            int3 key = ResolveChunkPos(ref block_pos);

            try
            {
                GetChunkUnsafe(chunk_pos + key).Value.SetBlock(block_pos, block);
            }
            catch
            {
                VloxyLogger.Error<ChunkManager>($"Error setting ref ({chunk_pos + key}) for chunk ({chunk_pos})");
            }
        }

        public Block GetBlockGlobal(int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);
            
            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Error<ChunkManager>($"Chunk : {chunk_pos} not loaded");
                return Block.ERROR;
            }
            
            var chunk = GetChunkUnsafe(chunk_pos).Value;

            return (Block) chunk.GetBlock(block_pos);
        }

        public bool SetBlockGlobal(Block block, int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);

            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Error<ChunkManager>($"Chunk : {chunk_pos} not loaded");
                return false;
            }

            var chunk = GetChunkUnsafe(chunk_pos).Value;
            
            var result = chunk.SetBlock(block_pos, VloxyUtils.GetBlockId(block));

            // _Chunks[chunk_pos] = chunk;

            // if (remesh && result) ReMeshChunks(position.Int3());
            
            return result;
        }

        #endregion
    }

}