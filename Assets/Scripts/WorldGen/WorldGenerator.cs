using CodeBlaze.Vloxy.Engine.Data;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    public class WorldGenerator
    {

        private readonly FastNoiseLite _ShapeNoise;
        private readonly ShapeNoiseProfile _ShapeNoiseProfile;

        private readonly FastNoiseLite _ContinentalNoise;
        private readonly BakedAnimationCurve _ContinentalCurve;
        private readonly ContinentalNoiseProfile _ContinentalNoiseProfile;

        private readonly FastNoiseLite _SquishNoise;
        private readonly BakedAnimationCurve _SquishCurve;
        private readonly SquishNoiseProfile _SquishNoiseProfile;

        public WorldGenerator(
            ShapeNoiseProfile shapeNoiseProfile,
            ContinentalNoiseProfile continentalNoiseProfile,
            SquishNoiseProfile squishNoiseProfile
        )
        {
            _ShapeNoiseProfile = shapeNoiseProfile;
            _ShapeNoise = FastNoiseLiteExtensions.FromProfile(shapeNoiseProfile);

            _ContinentalNoiseProfile = continentalNoiseProfile;
            _ContinentalNoise = FastNoiseLiteExtensions.FromProfile(continentalNoiseProfile);
            _ContinentalCurve = new(continentalNoiseProfile.Curve, continentalNoiseProfile.CurveResolution);

            _SquishNoiseProfile = squishNoiseProfile;
            _SquishNoise = FastNoiseLiteExtensions.FromProfile(squishNoiseProfile);
            _SquishCurve = new(squishNoiseProfile.Curve, squishNoiseProfile.CurveResolution);
        }

        public int GetBlock(int x, int y, int z)
        {
            var continental_value = GetContinentalValue(x, z);
            var squish_factor = GetSquishValue(x, z);

            var mod = (continental_value - y) * squish_factor;
            var density = GetShapeValue(x, y, z);

            if (density + mod > 0f)
            {
                return (int)Block.STONE;
            }
            else
            {
                return (int)Block.AIR;
            }
        }

        public float GetShapeValue(float x, float y, float z)
        {
            return _ShapeNoise.GetNoise(x, y, z);
        }

        public int GetContinentalValue(float x, float z)
        {
            return _ContinentalNoise.CurveSampleNoiseScaleShiftInt(
                x, z, 
                _ContinentalCurve, 
                _ContinentalNoiseProfile.Scale, 
                _ContinentalNoiseProfile.Shift
            );
        }

        public float GetSquishValue(float x, float z)
        {
            return _SquishNoise.CurveSampleNoiseScaleShiftFloat(
                x, z, 
                _SquishCurve, 
                _SquishNoiseProfile.Scale
            );
        }
    }
}