using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "SquishNoiseProfile", menuName = "Vloxy/Profile/SquishNoiseProfile", order = 0)]
    public class SquishNoiseProfile : FastNoiseLiteProfile {
        public AnimationCurve Curve;
        public int CurveResolution = 4096;
        public float Scale = 0.1f;
    }
}