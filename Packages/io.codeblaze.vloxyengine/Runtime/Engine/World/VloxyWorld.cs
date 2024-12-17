using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Jobs;
using CodeBlaze.Vloxy.Engine.Jobs.Chunk;
using CodeBlaze.Vloxy.Engine.Jobs.Collider;
using CodeBlaze.Vloxy.Engine.Jobs.Mesh;
using CodeBlaze.Vloxy.Engine.Noise;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;

using Runevision.Common;
using Runevision.LayerProcGen;
using Unity.Mathematics;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.World {

    public class VloxyWorld : MonoBehaviour {

        [SerializeField] private Transform _Focus;
        [SerializeField] private GenerationSource _GenerationSource;
        [SerializeField] private VloxySettings _Settings;
        
        #region API
        public Transform Focus => _Focus;
        public VloxySettings Settings => _Settings;
        public int3 FocusChunkCoord { get; private set; }
        public GridBounds LastUpdateBound { get; private set; }
        public GridBounds DiffBounds { get; private set; } // this will be used to query chunks for meshing
        
        
        public VloxyScheduler Scheduler { get; private set; }
        public NoiseProfile NoiseProfile { get; private set; }
        public ChunkManager ChunkManager { get; private set; }
        public IChunkManager TopLevelChunkManager { get; private set; }

        #endregion
        
        private ChunkPool _ChunkPool;
        private MeshBuildScheduler _MeshBuildScheduler;
        private ChunkScheduler _ChunkScheduler;
        private ColliderBuildScheduler _ColliderBuildScheduler;

        private bool _IsFocused;
        private byte _UpdateFrame = 1;
        private int _BoundSize = 0;
        private int _BoundOffset = 0;

        #region Virtual

        protected virtual VloxyProvider Provider() => new();
        protected virtual void WorldConfigure() { }
        protected virtual void WorldInitialize() { }
        protected virtual void WorldAwake() { }
        protected virtual void WorldStart() { }
        protected virtual void WorldUpdate() { }
        protected virtual void WorldFocusUpdate() { }
        protected virtual void WorldSchedulerUpdate() { }
        protected virtual void WorldLateUpdate() {}

        #endregion

        #region Unity

        private void Awake() {
            VloxyProvider.Initialize(Provider(), provider => {
                ConfigureSettings();
                
                provider.Settings = Settings;
#if VLOXY_LOGGING
                VloxyLogger.Info<VloxyWorld>("Provider Initialized");
#endif
                WorldInitialize();
            });

            ConstructVloxyComponents();
            
            FocusChunkCoord = new int3(1,1,1) * int.MinValue;
            LastUpdateBound = new(int.MinValue, int.MinValue, _BoundSize, _BoundSize); // size 128 * 128
            DiffBounds = LastUpdateBound;

            WorldAwake();
        }

        private void Start() {
            _IsFocused = _Focus != null;

            WorldStart();
        }

        private void Update() {
            #region Wait
            // Wait till initial chunks are generated
            if ((FocusChunkCoord == new int3(1,1,1) * int.MinValue).AndReduce() && TopLevelChunkManager.ChunkCount() == 0) {
                Debug.Log("Waiting");
                return;
            }

            if (LayerManager.instance.building) {
                Debug.Log("Waiting");
                return;
            }
            #endregion

            var NewFocusChunkCoord = _IsFocused ? VloxyUtils.GetChunkCoords(_Focus.position) : int3.zero;

            if (!(NewFocusChunkCoord == FocusChunkCoord).AndReduce()) {
                FocusChunkCoord = NewFocusChunkCoord;
                GridBounds NewUpdateBound = new(_BoundOffset + FocusChunkCoord.x, _BoundOffset + FocusChunkCoord.z, _BoundSize, _BoundSize);
                DiffBounds = NewUpdateBound.DiffBounds(LastUpdateBound);
                LastUpdateBound = NewUpdateBound;

                Scheduler.SchedulerUpdate2(DiffBounds);

                Scheduler.FocusChunkUpdate(FocusChunkCoord);
                WorldFocusUpdate();
            }
            
            // There are "should" and "could" checks that need to happen every frame as chunks may be ready
            // Scheduler.SchedulerUpdate(FocusChunkCoord);

            // Schedule every 'x' frames (throttling)
            if (_UpdateFrame % Settings.Scheduler.TickRate == 0) {
                _UpdateFrame = 1;

                Scheduler.JobUpdate();

                WorldSchedulerUpdate();
            } else {
                _UpdateFrame++;
            }

            WorldUpdate();

#if VLOXY_LOGGING
            DebugUtils.DrawBounds(DiffBounds, Color.green);
            DebugUtils.DrawBounds(LastUpdateBound, Color.cyan);
#endif
        }

        private void LateUpdate() {
            Scheduler.SchedulerLateUpdate();

            WorldLateUpdate();
        }

        private void OnDestroy() {
            Scheduler.Dispose();
            ChunkManager.Dispose();
        }
        
        #endregion

        private void ConfigureSettings() {
            Settings.Chunk.LoadDistance = Settings.Chunk.DrawDistance + 1;
            Settings.Chunk.ColliderDistance = math.min(Settings.Chunk.DrawDistance - 2, 2);

            _BoundOffset = -1 * Settings.Chunk.DrawDistance * 32;
            _BoundSize = (-2 * _BoundOffset) + 32;
            _GenerationSource.size = new Point(_BoundSize + 32, _BoundSize + 32);

            // TODO : ideally these should be dynamic based on device
            // Settings.Scheduler.MeshingBatchSize = 4;
            // Settings.Scheduler.StreamingBatchSize = 8;
            // Settings.Scheduler.ColliderBatchSize = 4;

            WorldConfigure();
        }
        
        private void ConstructVloxyComponents() {
            NoiseProfile = VloxyProvider.Current.NoiseProfile();
            ChunkManager = VloxyProvider.Current.ChunkManager();
            TopLevelChunkManager = VloxyProvider.Current.TopLevelChunkManager();

            _ChunkPool = VloxyProvider.Current.ChunkPool(transform);

            _MeshBuildScheduler = VloxyProvider.Current.MeshBuildScheduler(
                ChunkManager, 
                _ChunkPool
            );
            
            _ChunkScheduler = VloxyProvider.Current.ChunkDataScheduler(
                ChunkManager,
                NoiseProfile
            );

            _ColliderBuildScheduler = VloxyProvider.Current.ColliderBuildScheduler(
                ChunkManager,
                _ChunkPool
            );

            Scheduler = VloxyProvider.Current.VloxyScheduler(
                _MeshBuildScheduler, 
                _ChunkScheduler,
                _ColliderBuildScheduler,
                ChunkManager,
                _ChunkPool,
                TopLevelChunkManager
            );

#if VLOXY_LOGGING
            VloxyLogger.Info<VloxyWorld>("Vloxy Components Constructed");
#endif
        }

    }

}