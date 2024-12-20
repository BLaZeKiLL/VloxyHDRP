using System;
using System.Collections.Generic;
using CodeBlaze.Vloxy.Engine.Data;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;
using Priority_Queue;
using Runevision.Common;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Components {

    public interface IChunkManager {
        public int ChunkCount();
        public bool IsChunkLoaded(int3 position);
        public List<int3> GetChunksInBounds(GridBounds bounds);
        public ChunkAccessor GetAccessor(List<int3> positions);
    }

}