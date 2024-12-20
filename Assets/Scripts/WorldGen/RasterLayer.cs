using System;
using System.Collections.Generic;
using System.Linq;

using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;

using Runevision.Common;
using Runevision.LayerProcGen;

using Unity.Collections;
using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Demo {
    public class RasterChunk : LayerChunk<RasterLayer, RasterChunk> 
    {
        // TODO : Can we avoid this native reference
        public NativeReference<Chunk> Chunk { get; private set; }
        public bool Loaded { get; private set; }

        public override void Create(int level, bool destroy)
        {
            if (destroy) {
                Chunk.Dispose(); // Clear instead of dispose
                Loaded = false;
            } else {
                var data = new Chunk(new int3(bounds.min.x, 0, bounds.min.y), new int3(32, 256, 32));

                var noise = layer.GetNoise(bounds.min.x, bounds.min.y);
                int current_block = GetBlock(0, noise);

                int count = 0;

                // Loop order should be same as flatten order for AddBlocks to work properly
                for (var y = 0; y < 256; y++) {
                    for (var z = 0; z < 32; z++) {
                        for (var x = 0; x < 32; x++) {
                            noise = layer.GetNoise(bounds.min.x + x, bounds.min.y + z);
                            var block = GetBlock(y, noise);
            
                            if (block == current_block) {
                                count++;
                            } else {
                                data.AddBlocks(current_block, count);
                                current_block = block;
                                count = 1;
                            }
                        }
                    }
                }

                data.AddBlocks(current_block, count); // Finale interval

                Chunk = new NativeReference<Chunk>(data, Allocator.Persistent);
                Loaded = true; 
            }
        }

        private static int GetBlock(int Y, int height) {
            if (Y > height) return Y > 96 ? (int) Block.AIR : (int) Block.WATER;
            if (Y == height) return (int) Block.GRASS;
            if (Y <= height - 1 && Y >= height - 3) return (int) Block.DIRT;

            return (int) Block.STONE;
        }
    }

    public class RasterLayer : ChunkBasedDataLayer<RasterLayer, RasterChunk>, IChunkManager
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        private NativeParallelHashMap<int3, Chunk> _AccessorMap;
        private readonly FastNoiseLite fnl;
        private readonly int3 _ChunkSize;

        // public GridBounds Bounds => this.Bounds;

        public RasterLayer() {
            fnl = new FastNoiseLite();

            fnl.SetSeed(1337);
            fnl.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fnl.SetFrequency(0.01f);
            fnl.SetFractalType(FastNoiseLite.FractalType.FBm);
            fnl.SetFractalOctaves(3);
            fnl.SetFractalLacunarity(2.0f);
            fnl.SetFractalGain(0.5f);

            var meshing_batch_size = 6; // TODO : Fix Hardcode

            _AccessorMap = new NativeParallelHashMap<int3, Chunk>(
                (meshing_batch_size + 1) * (meshing_batch_size + 1), 
                Allocator.Persistent
            );

            _ChunkSize = new int3(32, 256, 32); // TODO : Fix Hardcode
        }

        public int GetNoise(float x, float z) {
            var height = fnl.GetNoise(x, z);
            // [-1,1] -> [0,256]
            return math.clamp((int) math.round(height * 128), -128, 128) + 128;
        }

        public bool IsChunkLoaded(int3 position)
        {
            var chunk = chunks[position.x / chunkW, position.z / chunkH];

            if (chunk == null) return false;

            return chunk.Loaded;
        }

        public List<int3> GetChunksInBounds(GridBounds bounds)
        {
            List<int3> chunks = new();

            HandleChunksInBounds(null, bounds, 0, (chunk) => {
                chunks.Add(new int3(chunk.bounds.min.x, 0 , chunk.bounds.min.y));
            });

            return chunks;
        }

        public ChunkAccessor GetAccessor(List<int3> positions)
        {
            _AccessorMap.Clear();

            foreach (var position in positions) {
                for (var x = -1; x <= 1; x++) {
                    for (var z = -1; z <= 1; z++) {
                        var pos = position + _ChunkSize.MemberMultiply(x,0,z);

                        if (!IsChunkLoaded(pos)) {
                            // Anytime this exception is thrown, mesh building completely stops
                            throw new InvalidOperationException($"Chunk {pos} has not been generated");
                        }

                        var raster_chunk = chunks[pos.x / chunkW, pos.z / chunkH];

                        if (!_AccessorMap.ContainsKey(pos))
                        {
                            UnityEngine.Debug.Log(raster_chunk.Chunk.IsCreated);
                            _AccessorMap.Add(pos, raster_chunk.Chunk.Value);
                        }
                    }
                }
            }

            return new ChunkAccessor(_AccessorMap.AsReadOnly(), _ChunkSize);
        }

        public int ChunkCount() => chunks.Count();

        public void Dispose()
        {
            foreach (var chunk in chunks) {
                chunk.Chunk.Dispose();
            }
        }
    }
}