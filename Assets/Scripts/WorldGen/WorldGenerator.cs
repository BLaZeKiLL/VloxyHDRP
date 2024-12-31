using CodeBlaze.Vloxy.Engine.Data;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    public class WorldGenerator {

        private readonly FastNoiseLite ShapeNoise;

        private readonly FastNoiseLite ContinentalNoise;
        private readonly BakedAnimationCurve ContinentalCurve;

        private readonly FastNoiseLite SquishNoise;
        private readonly BakedAnimationCurve SquishCurve;

        public WorldGenerator(
            FastNoiseLiteProfile shapeNoiseProfile,
            FastNoiseLiteProfile continentalNoiseProfile,
            AnimationCurve continentalCurve,
            FastNoiseLiteProfile squishNoiseProfile,
            AnimationCurve squishCurve
        ) {
            ShapeNoise = FastNoiseLiteExtensions.FromProfile(shapeNoiseProfile);

            ContinentalNoise = FastNoiseLiteExtensions.FromProfile(continentalNoiseProfile);
            ContinentalCurve = new(continentalCurve, 4096);

            SquishNoise = FastNoiseLiteExtensions.FromProfile(squishNoiseProfile);
            SquishCurve = new(squishCurve, 4096);
        }

        public int GetBlock(int x, int y, int z) {
            var continental_value = GetContinentalValue(x, z);
            var squish_factor = GetSquishValue(x, z);
            
            var mod = (continental_value - y) * squish_factor;
            var density = GetShapeValue(x, y, z);

            if (density + mod > 0f) {
                return (int) Block.STONE;
            } else {
                return (int) Block.AIR;
            }
        }

        public float GetShapeValue(float x, float y, float z) {
            return ShapeNoise.GetNoise(x, y, z);
        }

        public int GetContinentalValue(float x, float z) {
            return ContinentalNoise.CurveSampleNoiseScaleShiftInt(x, z, ContinentalCurve, 160, 32);
        }

        public float GetSquishValue(float x, float z) {
            return SquishNoise.CurveSampleNoiseScaleShiftFloat(x, z, SquishCurve, 0.1f);
        }
    }
}