using CodeBlaze.Vloxy.Engine.Settings;

using UnityEngine;
using UnityEngine.Rendering;

namespace CodeBlaze.Vloxy.Engine.Behaviour {

    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ChunkBehaviour : MonoBehaviour {

        private MeshRenderer _Renderer;

        public Mesh Mesh { get; private set; }
        public MeshCollider Collider { get; private set; }

        private void Awake() {
            Mesh = GetComponent<MeshFilter>().mesh;
            _Renderer = GetComponent<MeshRenderer>();
        }

        public void Init(RendererSettings settings, MeshCollider m_collider) {
            _Renderer.sharedMaterials = settings.Materials;
            Collider = m_collider;

            if (!settings.CastShadows) _Renderer.shadowCastingMode = ShadowCastingMode.Off;
        }

        private void OnDrawGizmos() {
            Gizmos.color = new Color(1, 0, 1, 0.5f);
            Gizmos.DrawWireCube(transform.position + new Vector3(16, 16, 16), new Vector3(32, 32, 32));
        }
    }

}