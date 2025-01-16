using System;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Jobs;
using CodeBlaze.Vloxy.Engine.Jobs.Collider;
using CodeBlaze.Vloxy.Engine.Jobs.Mesh;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Provider;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine {

    public class VloxyProvider : Provider<VloxyProvider> {

        public VloxySettings Settings { get; set; }

        protected internal ChunkManager ChunkManager() => new(Settings);

        protected internal virtual ChunkPool ChunkPool(Transform transform) => new (transform, Settings);

        protected internal virtual VloxyScheduler VloxyScheduler(
            MeshBuildScheduler meshBuildScheduler,
            ColliderBuildScheduler colliderBuildScheduler,
            ChunkPool chunkPool,
            ChunkManager chunkManager
        ) => new(Settings, meshBuildScheduler, colliderBuildScheduler, chunkPool, chunkManager);

        protected internal virtual MeshBuildScheduler MeshBuildScheduler(
            ChunkPool chunkPool,
            ChunkManager chunkManager
        ) => new(Settings, chunkPool, chunkManager);

        protected internal virtual ColliderBuildScheduler ColliderBuildScheduler(
            ChunkPool chunkPool
        ) => new(chunkPool);
    }

}