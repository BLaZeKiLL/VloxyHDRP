using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "ContinentalProfile", menuName = "Vloxy/Profiles/ContinentalProfile", order = 0)]
    public class ContinentalProfile : FastNoiseLiteProfile {
        public AnimationCurve Curve;
        public int CurveResolution = 4096;
        public int Scale = 160;
        public int Shift = 32;
    }
}