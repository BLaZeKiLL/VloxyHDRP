using System;
using System.Collections.Generic;
using System.Linq;

using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Runevision.Common;
using Runevision.LayerProcGen;

using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    public class RasterChunk : LayerChunk<RasterLayer, RasterChunk>
    {
        public override void Create(int level, bool destroy)
        {
            if (destroy)
            {
            }
            else
            {
                var position = new int3(bounds.min.x, 0, bounds.min.y);
                var chunk = new Chunk(position, new int3(32, 256, 32));

                var posX = position.x;
                var posZ = position.z;

                int current_block = layer.WorldGenerator.GetBlock(posX, 0, posZ);

                int count = 0;

                // Loop order should be same as flatten order for AddBlocks to work properly
                for (var y = 0; y < 256; y++)
                {
                    for (var z = 0; z < 32; z++)
                    {
                        for (var x = 0; x < 32; x++)
                        {
                            var block = layer.WorldGenerator.GetBlock(posX + x, y, posZ + z);

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
                        }
                    }
                }

                chunk.AddBlocks(current_block, count); // Finale interval

                var chunk_ref = new NativeReference<Chunk>(chunk, Allocator.Persistent);

                layer.ChunkManager.AddChunk(position, chunk_ref);
            }
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
        }
    }
}