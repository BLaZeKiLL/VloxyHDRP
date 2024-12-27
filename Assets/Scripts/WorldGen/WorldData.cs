using CodeBlaze.Vloxy.Demo.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Demo
{
    
    public class WorldData : SingletonBehaviour<WorldData> 
    {
        [SerializeField] private FastNoiseLiteProfile _HeightNoiseProfile;
        [SerializeField] private FastNoiseLiteProfile _ContinentNoiseProfile;
        [SerializeField] private AnimationCurve _ContinentRemapCurve;

        public FastNoiseLiteProfile HeightNoiseProfile => _HeightNoiseProfile;
        public FastNoiseLiteProfile ContinentNoiseProfile => _ContinentNoiseProfile;
        public BakedAnimationCurve ContinentRemapCurve => new(_ContinentRemapCurve, 4096);
    }
}