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
        public int[,] HeightMap { get; private set; } = new int[32, 32];

        public override void Create(int level, bool destroy)
        {
            var position = bounds.min.ToInt3XZ();

            if (destroy) {
                HeightMap.Clear();
                layer.ChunkManager.DisposeChunk(position);
                return;
            }

            var chunk = new Chunk(position, new int3(32, 256, 32));

            var posX = position.x;
            var posZ = position.z;

            int current_block = layer.WorldGenerator.GetBlock(posX, 0, posZ);

            # if VLOXY_COMPRESS
            int count = 0;
            #endif

            // Loop order should be same as flatten order for AddBlocks to work properly
            for (var y = 0; y < 256; y++)
            {
                for (var z = 0; z < 32; z++)
                {
                    for (var x = 0; x < 32; x++)
                    {
                        var block = layer.WorldGenerator.GetBlock(posX + x, y, posZ + z);

                        if (block == (int) Block.STONE) {
                            HeightMap[x, z] = math.max(HeightMap[x, z], y);
                        }

                        #if VLOXY_COMPRESS
                        if (block == current_block)
                        {
                            count++;
                        }
                        else
                        {
                            chunk.AddBlocks(current_block, count);
                            current_block = block;
                            count = 1;
                        }
                        #else
                        chunk.SetBlock(x, y, z, block);
                        #endif
                    }
                }
            }

            #if VLOXY_COMPRESS
            chunk.AddBlocks(current_block, count); // Finale interval
            #endif

            var chunk_ref = new NativeReference<Chunk>(chunk, Allocator.Persistent);

            layer.ChunkManager.AddChunk(position, chunk_ref);
        }
    }

    public class RasterLayer : ChunkBasedDataLayer<RasterLayer, RasterChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public WorldGenerator WorldGenerator { get; private set; }
        public ChunkManager ChunkManager { get; private set; }

        public RasterLayer()
        {
            WorldGenerator = WorldData.Current.Generator;
            ChunkManager = WorldAPI.Current.World.ChunkManager;

            // AddLayerDependency(new LayerDependency(StructureLayer.instance, 32));
        }

        public int[,] GetHeightMapForChunk(int3 position) {
            var chunk = chunks[
                Crd.Div(position.x, chunkW), 
                Crd.Div(position.z, chunkH)
            ];

            return chunk.HeightMap;
        }

    }
}