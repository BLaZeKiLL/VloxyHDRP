using CodeBlaze.Vloxy.Game.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    
    // [DefaultExecutionOrder(-1000)]
    public class WorldData : SingletonBehaviour<WorldData> 
    {
        [SerializeField] private FastNoiseLiteProfile _ShapeNoiseProfile;
        [SerializeField] private FastNoiseLiteProfile _ContinentalNoiseProfile;
        [SerializeField] private AnimationCurve _ContinentalCurve;
        [SerializeField] private FastNoiseLiteProfile _SquishNoiseProfile;
        [SerializeField] private AnimationCurve _SquishCurve;

        private WorldGenerator _Generator;

        // public FastNoiseLiteProfile HeightNoiseProfile => _ShapeNoiseProfile;
        // public FastNoiseLiteProfile ContinentNoiseProfile => _ContinentalNoiseProfile;
        // public BakedAnimationCurve ContinentRemapCurve => new(_ContinentalCurve, 4096);

        // TODO: Cache this ?
        public WorldGenerator Generator => _Generator;

        protected override void Initialize()
        {
            _Generator = new(
                _ShapeNoiseProfile,
                _ContinentalNoiseProfile,
                _ContinentalCurve,
                _ShapeNoiseProfile,
                _SquishCurve
            );
        }
    }
}