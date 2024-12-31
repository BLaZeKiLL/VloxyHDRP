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

namespace CodeBlaze.Vloxy.Game
{
    public class RasterChunk : LayerChunk<RasterLayer, RasterChunk>
    {
        // TODO : Can we avoid this native reference
        public NativeReference<Chunk> Data { get; private set; }
        public bool Loaded { get; private set; }

        public const float SquishFactor = 0.1f;

        public override void Create(int level, bool destroy)
        {
            if (destroy)
            {
                Data.Dispose(); // Clear instead of dispose
                Loaded = false;
            }
            else
            {
                var data = new Chunk(new int3(bounds.min.x, 0, bounds.min.y), new int3(32, 256, 32));

                var posX = bounds.min.x;
                var posZ = bounds.min.y;

                int current_block = GetBlock(posX, 0, posZ);

                int count = 0;

                // Loop order should be same as flatten order for AddBlocks to work properly
                for (var y = 0; y < 256; y++)
                {
                    for (var z = 0; z < 32; z++)
                    {
                        for (var x = 0; x < 32; x++)
                        {
                            var block = GetBlock(posX + x, y, posZ + z);

                            if (block == current_block)
                            {
                                count++;
                            }
                            else
                            {
                                data.AddBlocks(current_block, count);
                                current_block = block;
                                count = 1;
                            }
                        }
                    }
                }

                data.AddBlocks(current_block, count); // Finale interval

                Data = new NativeReference<Chunk>(data, Allocator.Persistent);
                Loaded = true;
            }
        }

        private int GetBlock(int x, int y, int z) {
            var continentalness = layer.GetContinentalValue(x, z);
            
            var mod = (continentalness - y) * SquishFactor;
            var density = layer.GetNoise3D(x, y, z);

            if (density + mod > 0f) {
                return (int)Block.STONE;
            } else {
                return (int)Block.AIR;
            }
        }

        private static int GetBlock(int Y, int height)
        {
            if (Y > height) return Y > 96 ? (int)Block.AIR : (int)Block.WATER;
            if (Y == height) return (int)Block.GRASS;
            if (Y <= height - 1 && Y >= height - 3) return (int)Block.DIRT;

            return (int)Block.STONE;
        }
    }

    public class RasterLayer : ChunkBasedDataLayer<RasterLayer, RasterChunk>, IChunkManager
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        private readonly FastNoiseLite fnl_height;
        private readonly FastNoiseLite fnl_continent;
        private readonly BakedAnimationCurve continent_curve;

        public RasterLayer()
        {
            fnl_height = FastNoiseLiteExtensions.FromProfile(WorldData.Current.HeightNoiseProfile);
            fnl_continent = FastNoiseLiteExtensions.FromProfile(WorldData.Current.ContinentNoiseProfile);

            continent_curve = WorldData.Current.ContinentRemapCurve;
        }

        public int GetContinentalValue(float x, float z) {
            return NoiseRemapScaleShift(fnl_continent.GetNoise(x, z), continent_curve, 160, 32);
        }

        public float GetNoise3D(float x, float y, float z) {
            return fnl_height.GetNoise(x, y, z);
        }

        public int GetNoise(float x, float z)
        {
            var height = NoiseScaleShift(fnl_height.GetNoise(x, z), 16); // [-1, 1] -> [-16, 16]
            var continent = NoiseRemapScaleShift(fnl_continent.GetNoise(x, z), continent_curve, 160, 32); // [-1, 1] -> [32, 192]
            return math.clamp(continent + height, 0, 256); // [64, 192] + [-16, 16] -> [0, 256]
        }

        /// <summary>
        /// noise = [-1, 1] -scale-> [-scale, scale] -shift-> [-scale+shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private int NoiseScaleShift(float noise, int scale, int shift = 0)
        {
            return math.clamp((int)math.round(noise * scale), -scale, scale) + shift;
        }

        /// <summary>
        /// noise = [-1, 1] -curve-> [0, 1] -scale-> [0, scale] -shift-> [shift, scale+shift]
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="curve"></param>
        /// <param name="scale"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private int NoiseRemapScaleShift(float noise, BakedAnimationCurve curve, int scale, int shift = 0) {
            var remap_val = curve.Evaluate(noise, -1.0f, 1.0f);
            return math.clamp((int)math.round(remap_val * scale), 0, scale) + shift;
        }

        public bool IsChunkLoaded(int3 position)
        {
            var chunk = chunks[position.x / chunkW, position.z / chunkH];

            if (chunk == null) return false;

            return chunk.Loaded;
        }

        public List<int3> GetChunkPositionsInBounds(GridBounds bounds)
        {
            List<int3> chunks = new();

            Point point = new(Crd.Div(bounds.min.x, chunkW), Crd.Div(bounds.min.y, chunkH));
            Point point2 = new(Crd.DivUp(bounds.max.x, chunkW), Crd.DivUp(bounds.max.y, chunkH));

            for (int x = point.x; x < point2.x; x++)
            {
                for (int z = point.y; z < point2.y; z++)
                {
                    chunks.Add(new int3(x * chunkW, 0, z * chunkH));
                }
            }

            return chunks;
        }

        public void PopulateChunkAccessor(List<int3> positions, NativeParallelHashMap<int3, Chunk> chunk_map)
        {
            foreach (var position in positions)
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        var pos = position + new int3(chunkW, 0, chunkH).MemberMultiply(x, 0, z);

                        if (!IsChunkLoaded(pos))
                        {
                            // Anytime this exception is thrown, mesh building completely stops
                            throw new InvalidOperationException($"Chunk {pos} has not been generated");
                        }

                        var raster_chunk = chunks[pos.x / chunkW, pos.z / chunkH];

                        if (!chunk_map.ContainsKey(pos))
                        {
                            chunk_map.Add(pos, raster_chunk.Data.Value);
                        }
                    }
                }
            }
        }

        public int ChunkCount() => chunks.Count();

        public void Dispose()
        {
            foreach (var chunk in chunks)
            {
                chunk.Data.Dispose();
            }
        }
    }
}