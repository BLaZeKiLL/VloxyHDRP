using CodeBlaze.Vloxy.Game.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    
    // [DefaultExecutionOrder(-1000)]
    public class WorldData : SingletonBehaviour<WorldData> 
    {
        [SerializeField] private ShapeNoiseProfile _ShapeNoiseProfile;
        [SerializeField] private ContinentalNoiseProfile _ContinentalNoiseProfile;
        [SerializeField] private SquishNoiseProfile _SquishNoiseProfile;

        private WorldGenerator _Generator;

        public WorldGenerator Generator => _Generator;

        protected override void Initialize()
        {
            _Generator = new(
                _ShapeNoiseProfile,
                _ContinentalNoiseProfile,
                _SquishNoiseProfile
            );
        }

        public ShapeNoiseProfile ShapeNoiseProfile
        {
            get => _ShapeNoiseProfile;
            #if UNITY_EDITOR
            set => _ShapeNoiseProfile = value;
            #endif
        }

        public ContinentalNoiseProfile ContinentalNoiseProfile
        {
            get => _ContinentalNoiseProfile;
            #if UNITY_EDITOR
            set => _ContinentalNoiseProfile = value;
            #endif
        }

        public SquishNoiseProfile SquishNoiseProfile
        {
            get => _SquishNoiseProfile;
            #if UNITY_EDITOR
            set => _SquishNoiseProfile = value;
            #endif
        }

    }
}