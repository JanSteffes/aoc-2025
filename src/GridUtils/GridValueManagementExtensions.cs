using System.Drawing;

namespace aoc_2025.GridUtils
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

    public static class GridOutputExtensions
    {
        public static void PrintColoredMap<T>(this Grid<T> grid, Dictionary<T, ConsoleColor> colors, ConsoleColor defaultColor = ConsoleColor.White) where T : notnull
        {
            Console.WriteLine();
            for (var y = 0; y < grid.MaxY; y++)
            {
                for (var x = 0; x < grid.MaxX; x++)
                {
                    var value = grid.GetEntryAt(x, y);
                    PrintValue(value, colors.TryGetValue(value, out var colorForValue) ? colorForValue : default);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void PrintColoredPointsMap<T>(this Grid<T> grid, Dictionary<Point, ConsoleColor>? colorsForPoints = null, IDictionary<Point, ConsoleColor>? customColors = null)
        {
            for (var y = 0; y < grid.MaxY; y++)
            {
                for (var x = 0; x < grid.MaxX; x++)
                {
                    PrintColoredPoint(grid, colorsForPoints, customColors, x, y);
                }
                Console.WriteLine();
            }
        }

        private static void PrintColoredPoint<T>(Grid<T> grid, Dictionary<Point, ConsoleColor>? colorsForPoints, IDictionary<Point, ConsoleColor>? customColors, int x, int y)
        {
            var currentPoint = new Point(x, y);
            var prevColor = Console.ForegroundColor;
            if (customColors?.TryGetValue(currentPoint, out ConsoleColor color) ?? false)
            {
                Console.ForegroundColor = color;
            }
            else if (colorsForPoints?.TryGetValue(currentPoint, out color) ?? false)
            {
                Console.ForegroundColor = color;
            }
            Console.Write(grid.GetEntryAt(currentPoint));
            Console.ForegroundColor = prevColor;
        }

        private static void PrintValue<T>(T value, ConsoleColor color)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = prevColor;
        }
    }
}