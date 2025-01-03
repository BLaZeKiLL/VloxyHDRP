using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "ContinentalNoiseProfile", menuName = "Vloxy/Profile/ContinentalNoiseProfile", order = 0)]
    public class ContinentalNoiseProfile : FastNoiseLiteProfile {
        public AnimationCurve Curve;
        public int CurveResolution = 4096;
        public int Scale = 160;
        public int Shift = 32;
    }
}