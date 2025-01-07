using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;

using Runevision.LayerProcGen;
using Unity.Mathematics;

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

            // Surface Replacement
            for (var z = 0; z < 32; z++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var y = height_map[x, z];
                        
                    if (chunk.GetBlock(x, y, z) == (int) Block.STONE) 
                    {
                        chunk.SetBlock(x, y, z, (int) Block.GRASS);

                        for (var y_iter = y - 1; y_iter >= math.max(0, y - 3); y_iter--)
                        {
                            chunk.SetBlock(x, y_iter, z, (int) Block.DIRT);
                        }
                    }
                }
            }
        }
    }

    public class DecorationLayer : ChunkBasedDataLayer<DecorationLayer, DecorationChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public ChunkManager ChunkManager { get; private set; }

        public DecorationLayer() {
            ChunkManager = WorldAPI.Current.World.ChunkManager;

            AddLayerDependency(new LayerDependency(RasterLayer.instance, 0));
        }
    }
}