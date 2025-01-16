using CodeBlaze.Vloxy.Engine.Components;
using CodeBlaze.Vloxy.Engine.Data;
using Unity.Mathematics;

namespace CodeBlaze.Vloxy.Game
{
    public static class Tree
    {
        public static void Generate(ChunkManager chunk_manager, int3 chunk_pos, int height, int x, int y, int z)
        {
            // Generate Bark
            for (int i = 0; i < height; i++)
            {
                chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + i, z), (int) Block.WOOD);
            }

            // Generate Leafs

            // Layer 1
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 2, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 2, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 2, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 2, z - 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 2, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 2, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 2, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 2, z - 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 2, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 2, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 2, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 2, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 2, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 2, z + 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 2, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 2, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 2, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 2, z - 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 2, z - 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 2, z - 2), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 2, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 2, z - 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 2, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 2, z - 2), (int) Block.LEAFS);

            // Layer 2
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 1, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 1, z - 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 1, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 1, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 1, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 1, z - 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 1, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 2, y + height - 1, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 1, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 2, y + height - 1, z + 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 1, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 1, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 1, z + 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height - 1, z - 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height - 1, z - 2), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height - 1, z - 2), (int) Block.LEAFS);

            // Layer 3
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height, z), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height, z - 1), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height, z - 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height, z - 1), (int) Block.LEAFS);

            // Layer 4
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height + 1, z), (int) Block.LEAFS);

            chunk_manager.SetBlockLocal(chunk_pos, new int3(x + 1, y + height + 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x - 1, y + height + 1, z), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height + 1, z + 1), (int) Block.LEAFS);
            chunk_manager.SetBlockLocal(chunk_pos, new int3(x, y + height + 1, z - 1), (int) Block.LEAFS);
        }
    }
}