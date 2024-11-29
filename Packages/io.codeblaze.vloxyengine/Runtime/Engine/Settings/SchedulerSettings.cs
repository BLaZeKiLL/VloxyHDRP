using System;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Settings {

    [Serializable]
    public class SchedulerSettings {

        public int MeshingBatchSize = 4;
        
        public int StreamingBatchSize = 8;

        public int ColliderBatchSize = 4;

        [Tooltip("Framerate at which the scheduler updates")]
        public int TickRate = 4;

    }

}