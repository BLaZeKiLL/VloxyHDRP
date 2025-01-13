using CodeBlaze.Vloxy.Engine.Data;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    public class WorldGenerator
    {
        private readonly WorldProfile _WorldProfile;

        private readonly FastNoiseLite _ShapeNoise;
        private readonly ShapeProfile _ShapeProfile;

        private readonly FastNoiseLite _ContinentalNoise;
        private readonly BakedAnimationCurve _ContinentalCurve;
        private readonly ContinentalProfile _ContinentalProfile;

        private readonly FastNoiseLite _SquishNoise;
        private readonly BakedAnimationCurve _SquishCurve;
        private readonly SquishProfile _SquishProfile;

        private readonly FastNoiseLite _TreeNoise;
        private readonly TreeProfile _TreeProfile;

        public WorldGenerator(
            WorldProfile worldProfile,
            ShapeProfile shapeNoiseProfile,
            ContinentalProfile continentalNoiseProfile,
            SquishProfile squishNoiseProfile,
            TreeProfile treeProfile
        )
        {
            _WorldProfile = worldProfile;

            _ShapeProfile = shapeNoiseProfile;
            _ShapeNoise = FastNoiseLiteExtensions.FromProfile(shapeNoiseProfile);

            _ContinentalProfile = continentalNoiseProfile;
            _ContinentalNoise = FastNoiseLiteExtensions.FromProfile(continentalNoiseProfile);
            _ContinentalCurve = new(continentalNoiseProfile.Curve, continentalNoiseProfile.CurveResolution);

            _SquishProfile = squishNoiseProfile;
            _SquishNoise = FastNoiseLiteExtensions.FromProfile(squishNoiseProfile);
            _SquishCurve = new(squishNoiseProfile.Curve, squishNoiseProfile.CurveResolution);

            _TreeProfile = treeProfile;
            _TreeNoise = FastNoiseLiteExtensions.FromProfile(treeProfile);
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
                return y >= _WorldProfile.WaterLevel ? (int)Block.AIR : (int)Block.WATER;
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
                _ContinentalProfile.Scale, 
                _ContinentalProfile.Shift
            );
        }

        public float GetSquishValue(float x, float z)
        {
            return _SquishNoise.CurveSampleNoiseScaleShiftFloat(
                x, z, 
                _SquishCurve, 
                _SquishProfile.Scale
            );
        }

        public float GetTreeChanceValue(float x, float z) 
        {
            return _TreeNoise.NoiseScaleShiftFloat(x, z, 0.5f, 0.5f) * _TreeProfile.Scale;
        }
    }
}