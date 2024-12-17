﻿using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Jobs;
using CodeBlaze.Vloxy.Engine.Jobs.Chunk;
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

        protected internal virtual ChunkManager ChunkManager() => new(Settings);

        protected internal virtual ChunkPool ChunkPool(Transform transform) => new (transform, Settings);

        protected internal virtual VloxyScheduler VloxyScheduler(
            MeshBuildScheduler meshBuildScheduler,
            ChunkScheduler ChunkScheduler,
            ColliderBuildScheduler colliderBuildScheduler,
            ChunkManager ChunkManager,
            ChunkPool chunkPool,
            IChunkManager topChunk
        ) => new(Settings, meshBuildScheduler, ChunkScheduler, colliderBuildScheduler, ChunkManager, chunkPool, topChunk);

        protected internal virtual ChunkScheduler ChunkDataScheduler(
            ChunkManager ChunkManager,
            NoiseProfile noiseProfile
        ) => new(Settings, ChunkManager, noiseProfile);

        protected internal virtual MeshBuildScheduler MeshBuildScheduler(
            ChunkManager ChunkManager,
            ChunkPool chunkPool
        ) => new(Settings, ChunkManager, chunkPool);

        protected internal virtual ColliderBuildScheduler ColliderBuildScheduler(
            ChunkManager chunkManager,
            ChunkPool chunkPool
        ) => new(chunkManager, chunkPool);

    }

}