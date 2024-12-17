using CodeBlaze.Vloxy.Engine;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.World;
using Runevision.LayerProcGen;
using Unity.Mathematics;

using UnityEngine;

namespace CodeBlaze.Vloxy.Demo {

    public class WorldProvider : VloxyProvider {
        protected override IChunkManager TopLevelChunkManager() {
            return RasterLayer.instance;
        }
    }

    public class World : VloxyWorld {
        protected override VloxyProvider Provider() => new WorldProvider();
        
        // protected override void WorldInitialize() {
        //     RenderSettings.fogMode = FogMode.Linear;
        //     RenderSettings.fogEndDistance = Settings.Chunk.DrawDistance * 32 - 16;
        // }

        public Vector3 GetSpawnPoint() {
            return new Vector3(16f, NoiseProfile.GetNoise(int3.zero).Height + 16, 16f);
        }

    }

}