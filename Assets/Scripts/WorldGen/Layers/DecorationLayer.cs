using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;

using Runevision.LayerProcGen;
using Unity.Mathematics;
using Runevision.Common;
using CodeBlaze.Vloxy.Engine.Utils.Logger;

namespace CodeBlaze.Vloxy.Game
{
    public class DecorationChunk : LayerChunk<DecorationLayer, DecorationChunk>
    {
        private static int3 _ChunkSize = new(32, 256, 32);

        public override void Create(int level, bool destroy)
        {
            if (destroy) return;

            var chunk_pos = bounds.min.ToInt3XZ();

            var chunk = layer.ChunkManager.GetChunkUnsafe(chunk_pos).Value;

            var height_map = RasterLayer.instance.GetHeightMapForChunk(chunk_pos);

            for (var z = 0; z < 32; z++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var y = height_map[x, z];

                    if (chunk.GetBlock(x, y, z) == (int)Block.STONE)
                    {
                        #region Surface Replacement
                        chunk.SetBlock(x, y, z, (int)Block.GRASS);

                        for (var y_iter = y - 1; y_iter >= math.max(0, y - 3); y_iter--)
                        {
                            chunk.SetBlock(x, y_iter, z, (int)Block.DIRT);
                        }
                        #endregion

                        #region Decoration Placement
                        // Block x, y, z = Grass
                        PlaceTree(chunk_pos, z, x, y);
                        #endregion
                    }
                }
            }
        }

        private void PlaceTree(int3 chunk_pos, int z, int x, int y)
        {
            if (ShouldPlaceTree(x, z))
            {
                var tree_height = CanPlaceTree(chunk_pos, x, y, z);

                if (tree_height >= 5 && y + tree_height + 1 < 256)
                {
                    Tree.Generate(layer.ChunkManager, chunk_pos, tree_height, x, y + 1, z);
                }
            }
        }

        private bool ShouldPlaceTree(int x, int z)
        {
            var val = ((float)layer.Rng.Range(0, 100, index.x, index.y, 2 * x, 2 * z)) / 100.0f;
            return val <= layer.WorldGenerator.GetTreeChanceValue(x, z);
        }

        private int CanPlaceTree(int3 chunk_pos, int x, int y, int z)
        {
            int height = 0;

            for (var w = -4; w <= 4; w++)
            {
                for (var b = -4; b <= 4; b++)
                {
                    for (var h = 1; h <= 7; h++)
                    {
                        if (y + h >= 256)
                        {
                            return 0;
                        }
                        else if (layer.ChunkManager.GetBlockLocal(chunk_pos, new int3(x + w, y + h, z + b)) == (int)Block.AIR)
                        {
                            height = h;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }

            if (height < 5)
            {
                return 0;
            }
            else
            {
                return layer.Rng.Range(5, 7, index.x, index.y, 2 * x + 1, 2 * y + 1, 2 * z + 1);
            }
        }
    }

    public class DecorationLayer : ChunkBasedDataLayer<DecorationLayer, DecorationChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public WorldGenerator WorldGenerator { get; private set; }
        public ChunkManager ChunkManager { get; private set; }

        public RandomHash Rng { get; private set; }

        public DecorationLayer()
        {
            WorldGenerator = WorldData.Current.Generator;
            ChunkManager = WorldAPI.Current.World.ChunkManager;

            Rng = new(WorldData.Current.TreeProfile.Seed);

            AddLayerDependency(new LayerDependency(RasterLayer.instance, 32));
        }
    }
}