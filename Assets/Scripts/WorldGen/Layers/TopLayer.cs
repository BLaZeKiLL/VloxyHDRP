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

namespace CodeBlaze.Vloxy.Game {
    public class TopChunk : LayerChunk<TopLayer, TopChunk> {}

    public class TopLayer : ChunkBasedDataLayer<TopLayer, TopChunk>
    {
        public override int chunkW => 32;

        public override int chunkH => 32;

        public TopLayer() {
            AddLayerDependency(new LayerDependency(DecorationLayer.instance, 0));
        }
    }
}