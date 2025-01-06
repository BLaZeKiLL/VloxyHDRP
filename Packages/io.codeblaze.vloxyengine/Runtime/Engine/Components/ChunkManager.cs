﻿using System;
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
        private readonly Dictionary<int3, NativeReference<Chunk>> Chunks;
        private readonly object ChunksLock;

        private readonly HashSet<int3> ReadyChunks;
        private readonly object ReadyChunksLock;

        private int3 ChunkSize;


        public ChunkManager(VloxySettings settings) {
            ChunkSize = settings.Chunk.ChunkSize;

            var size = settings.Chunk.DrawDistance.XZSize();

            Chunks = new Dictionary<int3, NativeReference<Chunk>>(size);
            ChunksLock = new();

            ReadyChunks = new HashSet<int3>(size);
            ReadyChunksLock = new object();
        }

        public int ChunkCount() => Chunks.Count;

        public void Dispose()
        {
            foreach (var (_, chunk) in Chunks) {
                chunk.Dispose();
            }
        }

        public void AddChunk(int3 position, NativeReference<Chunk> chunk) {
            lock (ChunksLock) {
                Chunks.Add(position, chunk);
            }
        }

        public void MarkChunkReady(int3 postion) {
            lock (ReadyChunksLock) {
                ReadyChunks.Add(postion);
            }
        }

        public Block GetBlock(int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);
            
            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Warn<ChunkManager>($"Chunk : {chunk_pos} not loaded");
                return Block.ERROR;
            }
            
            var chunk = GetChunkUnsafe(chunk_pos).Value;

            return (Block) chunk.GetBlock(block_pos);
        }

        public bool SetBlock(Block block, int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);

            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Warn<ChunkManager>($"Chunk : {chunk_pos} not loaded");
                return false;
            }

            var chunk = GetChunkUnsafe(chunk_pos).Value;
            
            var result = chunk.SetBlock(block_pos, VloxyUtils.GetBlockId(block));

            // _Chunks[chunk_pos] = chunk;

            // if (remesh && result) ReMeshChunks(position.Int3());
            
            return result;
        }

        public NativeReference<Chunk> GetChunkUnsafe(int3 position) => Chunks[position];

        public bool IsChunkLoaded(int3 position) => ReadyChunks.Contains(position);

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

        internal void RemoveChunks(List<int3> positions) {
            lock (ChunksLock) {
                foreach (var position in positions) {
                    RemoveChunk(position);
                }
            }
        }

        private void RemoveChunk(int3 position) {
            Chunks[position].Dispose();
            Chunks.Remove(position);
            ReadyChunks.Remove(position);
        }
    }

}