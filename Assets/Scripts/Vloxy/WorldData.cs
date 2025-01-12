using CodeBlaze.Vloxy.Game.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game
{
    
    public class WorldData : SingletonBehaviour<WorldData> 
    {
        [SerializeField] private WorldProfile _WorldProfile;
        [SerializeField] private ShapeProfile _ShapeProfile;
        [SerializeField] private ContinentalProfile _ContinentalProfile;
        [SerializeField] private SquishProfile _SquishProfile;
        [SerializeField] private TreeProfile _TreeProfile;

        private WorldGenerator _Generator;

        public WorldGenerator Generator => _Generator;

        protected override void Initialize()
        {
            _Generator = new(
                _WorldProfile,
                _ShapeProfile,
                _ContinentalProfile,
                _SquishProfile,
                _TreeProfile
            );
        }

        public WorldProfile WorldProfile
        {
            get => _WorldProfile;
            #if UNITY_EDITOR
            set => _WorldProfile = value;
            #endif
        }

        public ShapeProfile ShapeProfile
        {
            get => _ShapeProfile;
            #if UNITY_EDITOR
            set => _ShapeProfile = value;
            #endif
        }

        public ContinentalProfile ContinentalProfile
        {
            get => _ContinentalProfile;
            #if UNITY_EDITOR
            set => _ContinentalProfile = value;
            #endif
        }

        public SquishProfile SquishProfile
        {
            get => _SquishProfile;
            #if UNITY_EDITOR
            set => _SquishProfile = value;
            #endif
        }

        public TreeProfile TreeProfile
        {
            get => _TreeProfile;
            #if UNITY_EDITOR
            set => _TreeProfile = value;
            #endif
        }

    }
}