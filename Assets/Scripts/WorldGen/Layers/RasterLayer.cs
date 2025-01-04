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
        public NativeReference<Chunk> Data { get; private set; }
        public bool Loaded { get; private set; }

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
    }

    public class RasterLayer : ChunkBasedDataLayer<RasterLayer, RasterChunk>, IChunkManager
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public WorldGenerator WorldGenerator { get; private set; }

        public RasterLayer()
        {
            WorldGenerator = WorldData.Current.Generator;
        }

        #region ChunkManager
        public NativeReference<Chunk> GetChunk(int3 position) 
        {
            return chunks[position.x / chunkW, position.z / chunkH].Data;
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

        public Block GetBlock(int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);
            
            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Warn<IChunkManager>($"Chunk : {chunk_pos} not loaded");
                return Block.ERROR;
            }
            
            var chunk = GetChunk(chunk_pos).Value;

            return (Block) chunk.GetBlock(block_pos);
        }

        public bool SetBlock(Block block, int3 position)
        {
            var chunk_pos = VloxyUtils.GetChunkCoords(position);
            var block_pos = VloxyUtils.GetBlockIndex(position);

            if (!IsChunkLoaded(chunk_pos)) {
                VloxyLogger.Warn<IChunkManager>($"Chunk : {chunk_pos} not loaded");
                return false;
            }

            var chunk = GetChunk(chunk_pos).Value;
            
            var result = chunk.SetBlock(block_pos, VloxyUtils.GetBlockId(block));

            // _Chunks[chunk_pos] = chunk;

            // if (remesh && result) ReMeshChunks(position.Int3());
            
            return result;
        }
        #endregion
    }
}