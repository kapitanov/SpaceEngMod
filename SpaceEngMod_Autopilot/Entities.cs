using System.Collections.Generic;

namespace SpaceEngMod
{
    public static class Entities
    {
        public static List<CubeGrid> CubeGrids = new List<CubeGrid>();

        public static void Add(CubeGrid entity)
        {
            CubeGrids.Add(entity);
        }

        public static void Remove(CubeGrid entity)
        {
            CubeGrids.Remove(entity);
        }
    }
}
