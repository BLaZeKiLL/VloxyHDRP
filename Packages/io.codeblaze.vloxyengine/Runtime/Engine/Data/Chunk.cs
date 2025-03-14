﻿using System;
using CodeBlaze.Vloxy.Engine.Utils.Collections;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Data {

#if VLOXY_COMPRESS
    [BurstCompile]
    public struct Chunk {

        public int3 Position { get; }
        public bool Dirty { get; private set; }
        
        private int3 ChunkSize;
        private UnsafeIntervalList Data;

        public Chunk(int3 position, int3 chunkSize) {
            Dirty = false;
            Position = position;
            ChunkSize = chunkSize;
            Data = new UnsafeIntervalList(128, Allocator.Persistent);
        }

        public readonly bool IsAirChunk => Data.CompressedLength == 1 && Data.Get(0) == (int) Block.AIR;

        public void AddBlocks(int block, int count) {
            Data.AddInterval(block, count);
        }

        public bool SetBlock(int x, int y, int z, int block) {
            var result = Data.Set(ChunkSize.Flatten(x,y,z), block);
            if (result) Dirty = true;
            return result;
        }
        
        public bool SetBlock(int3 pos, int block) {
            var result= Data.Set(ChunkSize.Flatten(pos), block);
            if (result) Dirty = true;
            return result;
        }

        public int GetBlock(int x, int y, int z) {
            return Data.Get(ChunkSize.Flatten(x, y, z));
        }

        public int GetBlock(int3 pos) {
            return Data.Get(ChunkSize.Flatten(pos.x, pos.y, pos.z));
        }

        public void Dispose() {
            Data.Dispose();
        }

        public override string ToString() {
            return $"Pos : {Position}, Dirty : {Dirty}, Data : {Data.ToString()}";
        }

    }
#else
    [BurstCompile]
    public struct Chunk {

        public int3 Position { get; }
        public bool Dirty { get; private set; }
        
        private int3 ChunkSize;
        private UnsafeList<int> Data;

        public Chunk(int3 position, int3 chunkSize) {
            Dirty = false;
            Position = position;
            ChunkSize = chunkSize;
            Data = new UnsafeList<int>(32 * 256 * 32, Allocator.Persistent);
            Data.Resize(32 * 256 * 32);
        }

        public readonly bool IsAirChunk => false;

        public bool SetBlock(int x, int y, int z, int block) {
            #if UNITY_EDITOR
            if (ChunkSize.Flatten(x, y, z) >= 32 * 256 * 32) {
                throw new IndexOutOfRangeException($"SET: Index ({x}, {y}, {z}) is out of range");
            }
            #endif
            Data[ChunkSize.Flatten(x,y,z)] = block;
            Dirty = true;
            return true;
        }
        
        public bool SetBlock(int3 pos, int block) {
            return SetBlock(pos.x, pos.y, pos.z, block);
        }

        public int GetBlock(int x, int y, int z) {
            #if UNITY_EDITOR
            if (ChunkSize.Flatten(x, y, z) >= 32 * 256 * 32) {
                throw new IndexOutOfRangeException($"GET: Index ({x}, {y}, {z}) is out of range");
            }
            #endif
            return Data[ChunkSize.Flatten(x, y, z)];
        }

        public int GetBlock(int3 pos) {
            return GetBlock(pos.x, pos.y, pos.z);
        }

        public void Dispose() {
            Data.Dispose();
        }

        public override string ToString() {
            return $"Pos : {Position}, Dirty : {Dirty}, Data : {Data}";
        }
    }
#endif
}