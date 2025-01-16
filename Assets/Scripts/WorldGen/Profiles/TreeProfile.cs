using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "TreeProfile", menuName = "Vloxy/Profiles/TreeProfile", order = 0)]
    public class TreeProfile : FastNoiseLiteProfile {

        [Range(0.0f, 1.0f)]
        public float Scale = 0.5f;

    }
}