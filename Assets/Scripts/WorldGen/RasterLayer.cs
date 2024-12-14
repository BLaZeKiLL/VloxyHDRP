using CodeBlaze.Vloxy.Engine.Data;
using Runevision.LayerProcGen;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBlaze.Vloxy.Demo {
    public class RasterChunk : LayerChunk<RasterLayer, RasterChunk> 
    {
        private Chunk chunk;

        public override void Create(int level, bool destroy)
        {
            if (destroy) {
                chunk.Dispose(); // Clear instead of dispose
            } else {
                chunk = new(new int3(bounds.min.x, 0, bounds.min.y), new int3(32, 32, 32));

                var noise = GetNoise(bounds.min.x, bounds.min.y);
                int current_block = GetBlock(0, noise);
                
                int count = 0;
            
                // Loop order should be same as flatten order for AddBlocks to work properly
                for (var y = 0; y < 32; y++) {
                    for (var z = bounds.min.y; z < bounds.max.y; z++) {
                        for (var x = bounds.min.x; x < bounds.max.x; x++) {
                            noise = GetNoise(x, z);
                            var block = GetBlock(y, noise);
            
                            if (block == current_block) {
                                count++;
                            } else {
                                chunk.AddBlocks(current_block, count);
                                current_block = block;
                                count = 1;
                            }
                        }
                    }
                }
                
                chunk.AddBlocks(current_block, count); // Finale interval

                Debug.Log(chunk);
            }
        }

        private int GetNoise(float x, float z) {
            var height = layer.fnl.GetNoise(x, z);
            return math.clamp((int) math.round(height * 128), -128, 128);
        }

        private static int GetBlock(int Y, int height) {
            if (Y > height) return Y > 96 ? (int) Block.AIR : (int) Block.WATER;
            if (Y == height) return (int) Block.GRASS;
            if (Y <= height - 1 && Y >= height - 3) return (int) Block.DIRT;

            return (int) Block.STONE;
        }
    }

    public class RasterLayer : ChunkBasedDataLayer<RasterLayer, RasterChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public FastNoiseLite fnl;

        public RasterLayer() {
            fnl = new FastNoiseLite();

            fnl.SetSeed(1337);
            fnl.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fnl.SetFrequency(0.01f);
            fnl.SetFractalType(FastNoiseLite.FractalType.FBm);
            fnl.SetFractalOctaves(3);
            fnl.SetFractalLacunarity(2.0f);
            fnl.SetFractalGain(0.5f);
        }
    }
}