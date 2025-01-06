using CodeBlaze.Vloxy.Engine;
using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.World;
using Runevision.LayerProcGen;
using Unity.Mathematics;

using UnityEngine;

namespace CodeBlaze.Vloxy.Game {

    public class WorldEngine : VloxyWorld {

        // protected override void WorldInitialize() {
        //     RenderSettings.fogMode = FogMode.Linear;
        //     RenderSettings.fogEndDistance = Settings.Chunk.DrawDistance * 32 - 16;
        // }

        public Vector3 GetSpawnPoint() {
            // TODO: Fix this !!!
            return new Vector3(16f, 128f, 16f);
        }

    }

}