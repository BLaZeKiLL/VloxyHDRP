using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Runevision.LayerProcGen;
using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Game
{
    public class DecorationChunk : LayerChunk<DecorationLayer, DecorationChunk>
    {
        public override void Create(int level, bool destroy)
        {
            if (destroy) return;

            var position = new int3(bounds.min.x, 0, bounds.min.y);

            if (RasterLayer.instance.IsChunkLoaded(position)) {
                VloxyLogger.Warn<DecorationLayer>($"Decoration run for {position} while chunk not loaded");
                return;
            }

            var chunk = RasterLayer.instance.GetChunk(position).Value;

            // Surface Replacement
            for (var z = 0; z < 32; z++)
            {
                for (var x = 0; x < 32; x++)
                {
                    for (var y = 0; y < 256; y++)
                    {
                        var y_invert = 256 - y;

                        if (chunk.GetBlock(x, y_invert, z) == (int) Block.STONE) 
                        {
                            chunk.SetBlock(x, y_invert, z, (int) Block.GRASS);

                            for (var y_iter = y_invert - 1; y_iter >= math.max(0, y_iter - 3); y_iter--)
                            {
                                chunk.SetBlock(x, y_invert, z, (int) Block.DIRT);
                            }

                            break;
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

        public DecorationLayer() {
            AddLayerDependency(new LayerDependency(RasterLayer.instance, 0));
        }
    }
}