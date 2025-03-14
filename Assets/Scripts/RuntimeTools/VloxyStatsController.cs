using System.Text;

using CodeBlaze.Vloxy.Engine.World;

using Tayx.Graphy.Utils.NumString;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

namespace CodeBlaze.Vloxy.Game.RuntimeTools {

    public class VloxyStatsController : MonoBehaviour {

        [SerializeField] private Text Coords;
        [SerializeField] private Text ChunkCoords;
        [SerializeField] private Text Data;
        [SerializeField] private Text Mesh;
        [SerializeField] private Text Bake;

        private StringBuilder sb;

        private void Start() {
            sb = new StringBuilder();
        }

        private void Update() {
            var world = WorldAPI.Current.World;
            
            UpdateCoords(Vector3Int.RoundToInt(world.Focus.position));
            UpdateChunkCoords(world.FocusChunkCoord);
            UpdateData(world.Scheduler.DataQueueCount, world.Scheduler.DataAvgTiming);
            UpdateMesh(world.Scheduler.MeshQueueCount, world.Scheduler.MeshAvgTiming);
            UpdateBake(world.Scheduler.BakeQueueCount, world.Scheduler.BakeAvgTiming);
        }

        private void UpdateCoords(Vector3Int coords) {
            sb.Clear();

            sb.Append("Focus Block Coordinates : ")
              .Append("X = ").Append(coords.x.ToStringNonAlloc())
              .Append(", Y = ").Append(coords.y.ToStringNonAlloc())
              .Append(", Z = ").Append(coords.z.ToStringNonAlloc());

            Coords.text = sb.ToString();
        }
        
        private void UpdateChunkCoords(int3 chunk_coords) {
            sb.Clear();

            sb.Append("Focus Chunk Coordinates : ")
              .Append("X = ").Append(chunk_coords.x.ToStringNonAlloc())
              .Append(", Y = ").Append(chunk_coords.y.ToStringNonAlloc())
              .Append(", Z = ").Append(chunk_coords.z.ToStringNonAlloc());

            ChunkCoords.text = sb.ToString();
        }

        private void UpdateData(int queue_count, float avg) {
            sb.Clear();

            sb.Append("Chunk Queue : ").Append(queue_count.ToStringNonAlloc())
              .Append(", Average : ").Append(avg.ToString("F3")).Append("ms");

            Data.text = sb.ToString();
        }

        private void UpdateMesh(int queue_count, float avg) {
            sb.Clear();

            sb.Append("Mesh Queue : ").Append(queue_count.ToStringNonAlloc())
              .Append(", Average : ").Append(avg.ToString("F3")).Append("ms");

            Mesh.text = sb.ToString();
        }

        private void UpdateBake(int queue_count, float avg) {
            sb.Clear();

            sb.Append("Bake Queue : ").Append(queue_count.ToStringNonAlloc())
              .Append(", Average : ").Append(avg.ToString("F3")).Append("ms");

            Bake.text = sb.ToString();
        }

    }

}