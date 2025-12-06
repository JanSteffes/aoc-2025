using aoc_2025.Structures;
using System.Drawing;

namespace aoc_2025.StructureExtensions.Grid
{
    public static class GridOutputExtensions
    {
        public static bool Print { get; set; }

        public static void PrintColoredMap<T>(this Grid<T> grid, Dictionary<T, ConsoleColor> colors, ConsoleColor defaultColor = ConsoleColor.White) where T : notnull
        {
            PrintMap(grid, point =>
            {
                return colors.TryGetValue(grid.GetEntryAt(point), out var colorForValue) ? colorForValue : default;
            });
        }

        public static void PrintColoredPointsMap<T>(this Grid<T> grid, Dictionary<Point, ConsoleColor>? colorsForPoints = null, IDictionary<Point, ConsoleColor>? customColors = null)
        {
            PrintMap(grid, point =>
            {
                if (customColors?.TryGetValue(point, out var customColor) ?? false)
                {
                    return customColor;
                }
                if (colorsForPoints?.TryGetValue(point, out var colorForPoint) ?? false)
                {
                    return colorForPoint;
                }
                return Console.ForegroundColor;
            });
        }

        private static void PrintMap<T>(Grid<T> grid, Func<Point, ConsoleColor> callBack)
        {
            if (!Print)
            {
                return;
            }
            Console.WriteLine();
            for (var y = 0; y < grid.MaxY; y++)
            {
                for (var x = 0; x < grid.MaxX; x++)
                {
                    var currentPoint = new Point(x, y);
                    var color = callBack.Invoke(currentPoint);
                    PrintValue(grid.GetEntryAt(currentPoint), color);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
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