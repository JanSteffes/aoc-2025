using aoc_2025.Structures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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

        public static void PrintToImage<T>(this Grid<T> grid, string filePath, IDictionary<Point, Color> colorValues, IDictionary<Point, Color> customColors, Color? backgroundColor = null)
        {
            var bitmap = new Image<Rgba32>(grid.MaxX, grid.MaxY);
            for (int y = 0; y < grid.MaxY; y++)
            {
                for (int x = 0; x < grid.MaxX; x++)
                {
                    var current = new Point(x, y);
                    if (!customColors.TryGetValue(current, out var color))
                    {
                        colorValues.TryGetValue(current, out color);
                    }
                    bitmap[x, y] = color;
                }
            }
            if (backgroundColor != null)
            {
                bitmap.Mutate(x => x.BackgroundColor(backgroundColor.Value));
            }
            // resize
            bitmap.Mutate(x => x.Resize(bitmap.Size.Width * 10, bitmap.Size.Height * 10));
            //Task.Factory.StartNew(() =>
            //{
            bitmap.Save(filePath, new PngEncoder());
            bitmap.Dispose();
            //});
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