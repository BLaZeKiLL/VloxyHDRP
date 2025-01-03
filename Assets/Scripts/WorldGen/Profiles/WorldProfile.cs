using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    [CreateAssetMenu(fileName = "WorldProfile", menuName = "Vloxy/Profiles/WorldProfile", order = 0)]
    public class WorldProfile : ScriptableObject {
        public int WaterLevel = 80;
    }
}