using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Runevision.Common;
using Runevision.LayerProcGen;
using Unity.Collections;
using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Game {
    public class TopChunk : LayerChunk<TopLayer, TopChunk> {
        public override void Create(int level, bool destroy)
        {
            var position = bounds.min.ToInt3XZ();

            if (destroy) {
                layer.ChunkManager.MarkUnready(position);
                return;
            }

            layer.ChunkManager.MarkChunkReady(position);
        }
    }

    public class TopLayer : ChunkBasedDataLayer<TopLayer, TopChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public ChunkManager ChunkManager { get; private set; }

        public TopLayer() {
            ChunkManager = WorldAPI.Current.World.ChunkManager;

            AddLayerDependency(new LayerDependency(DecorationLayer.instance, 32));
        }
    }
}