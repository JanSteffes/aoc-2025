using aoc_2025.Structures;
using System.Drawing;

namespace aoc_2025.StructureExtensions.Grid
{
    public static class GridValueManagementExtensions
    {
        public static T GetEntryAt<T>(this Grid<T> grid, Point point)
        {
            return grid.GetEntryAt(point.X, point.Y);
        }

        public static void SetEntryAt<T>(this Grid<T> grid, Point point, T entry)
        {
            grid.SetEntryAt(point.X, point.Y, entry);
        }
    }
}