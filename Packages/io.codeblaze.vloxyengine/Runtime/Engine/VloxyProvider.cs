using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Jobs;
using CodeBlaze.Vloxy.Engine.Jobs.Collider;
using CodeBlaze.Vloxy.Engine.Jobs.Mesh;
using CodeBlaze.Vloxy.Engine.Noise;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Provider;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine {

    public class VloxyProvider : Provider<VloxyProvider> {

        public VloxySettings Settings { get; set; }

        internal virtual NoiseProfile NoiseProfile() => new (new NoiseProfile.Settings {
            Height = Settings.Noise.Height,
            WaterLevel = Settings.Noise.WaterLevel,
            Seed = Settings.Noise.Seed,
            Scale = Settings.Noise.Scale,
            Lacunarity = Settings.Noise.Lacunarity,
            Persistance = Settings.Noise.Persistance,
            Octaves = Settings.Noise.Octaves,
        });

        protected internal virtual IChunkManager TopLevelChunkManager() => null;

        protected internal virtual ChunkPool ChunkPool(Transform transform) => new (transform, Settings);

        protected internal virtual VloxyScheduler VloxyScheduler(
            MeshBuildScheduler meshBuildScheduler,
            ColliderBuildScheduler colliderBuildScheduler,
            ChunkPool chunkPool,
            IChunkManager topChunk
        ) => new(Settings, meshBuildScheduler, colliderBuildScheduler, chunkPool, topChunk);

        protected internal virtual MeshBuildScheduler MeshBuildScheduler(
            ChunkPool chunkPool,
            IChunkManager topLevel
        ) => new(Settings, chunkPool, topLevel);

        protected internal virtual ColliderBuildScheduler ColliderBuildScheduler(
            ChunkPool chunkPool
        ) => new(chunkPool);

    }

}