using CodeBlaze.Vloxy.Demo.Utils;
using UnityEngine;

namespace CodeBlaze.Vloxy.Demo {
    
    public class WorldAPI : SingletonBehaviour<WorldAPI> {

        public WorldEngine World { get; private set; }

        protected override void Initialize() {
            World = GetComponent<WorldEngine>();
        }

    }

}