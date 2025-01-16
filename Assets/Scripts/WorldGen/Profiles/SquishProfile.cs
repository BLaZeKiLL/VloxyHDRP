using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "SquishProfile", menuName = "Vloxy/Profiles/SquishProfile", order = 0)]
    public class SquishProfile : FastNoiseLiteProfile {
        public AnimationCurve Curve;
        public int CurveResolution = 4096;
        public float Scale = 0.1f;
    }
}