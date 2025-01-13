using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;

using Runevision.LayerProcGen;
using Unity.Mathematics;
using Runevision.Common;

namespace CodeBlaze.Vloxy.Game
{
    public class DecorationChunk : LayerChunk<DecorationLayer, DecorationChunk>
    {
        public override void Create(int level, bool destroy)
        {
            if (destroy) return;

            var position = bounds.min.ToInt3XZ();

            var chunk = layer.ChunkManager.GetChunkUnsafe(position).Value;

            var height_map = RasterLayer.instance.GetHeightMapForChunk(position); 

            for (var z = 0; z < 32; z++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var y = height_map[x, z];
                        
                    if (chunk.GetBlock(x, y, z) == (int) Block.STONE) 
                    {
                        #region Surface Replacement
                        chunk.SetBlock(x, y, z, (int) Block.GRASS);

                        for (var y_iter = y - 1; y_iter >= math.max(0, y - 3); y_iter--)
                        {
                            chunk.SetBlock(x, y_iter, z, (int) Block.DIRT);
                        }
                        #endregion

                        #region Decoration Placement
                        // Block x, y, z = Grass
                        if (ShouldPlaceTree(x, z)) {
                            var tree_height = CanPlaceTree(chunk, x, y, z);

                            if (tree_height >= 3 && y + tree_height + 1 < 256) {
                                // Stump
                                for (var y_iter = 1; y_iter <= tree_height; y_iter++) {
                                    chunk.SetBlock(x, y + y_iter, z, (int) Block.WOOD);
                                }

                                chunk.SetBlock(x, y + tree_height + 1, z, (int) Block.LEAFS);
                            }
                        }

                        #endregion
                    }
                }
            }
        }

        private bool ShouldPlaceTree(int x, int z) {
            var val = ((float) layer.Rng.Range(0, 100, index.x, index.y, 2 * x, 2 * z)) / 100.0f;
            return val <= layer.WorldGenerator.GetTreeChanceValue(x, z);
        }

        private int CanPlaceTree(Chunk chunk, int x, int y, int z) {
            int height = 0;

            for (var i = 1; i <= 5; i ++) {
                if (y + i >= 256) {
                    return 0;
                }
                else if (chunk.GetBlock(x, y + i, z) == (int) Block.AIR) {
                    height = i;
                } else {
                    break;
                }
            }

            if (height < 3) {
                return 0;
            } else {
                // return 4;
                return layer.Rng.Range(3, 5, index.x, index.y, 2 * x + 1, 2 * y + 1, 2 * z + 1);
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

        public DecorationLayer() {
            WorldGenerator = WorldData.Current.Generator;
            ChunkManager = WorldAPI.Current.World.ChunkManager;

            Rng = new(WorldData.Current.TreeProfile.Seed);

            AddLayerDependency(new LayerDependency(RasterLayer.instance, 0));
        }
    }
}