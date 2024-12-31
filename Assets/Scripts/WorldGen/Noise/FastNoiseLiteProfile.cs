using UnityEngine;

namespace CodeBlaze.Vloxy.Game {

    [CreateAssetMenu(fileName = "FastNoiseLiteProfile", menuName = "Vloxy/FastNoiseLiteProfile", order = 0)]
    public class FastNoiseLiteProfile : ScriptableObject {

        public int Seed = 777;
        public float Frequency = 0.01f;
        public FastNoiseLite.NoiseType NoiseType = FastNoiseLite.NoiseType.OpenSimplex2S;
        public FastNoiseLite.FractalType FractalType = FastNoiseLite.FractalType.FBm;
        public float FractalGain = 0.5f;
        public float FractalLacunarity = 2f;
        public int FractalOctaves = 4;

    }

}