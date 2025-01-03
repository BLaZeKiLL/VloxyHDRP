using CodeBlaze.Vloxy.Game.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Game {
    
    public class WorldAPI : SingletonBehaviour<WorldAPI> {

        public WorldEngine World { get; private set; }

        protected override void Initialize() {
            World = GetComponent<WorldEngine>();
        }

    }

}