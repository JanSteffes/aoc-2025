using aoc_2025.Solutions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace aoc_2025.SolutionUtils
{
    public static class OutputUtils
    {
        public static void PrintToImage(string filePath, IDictionary<Point, Color> colorValues, Color? backgroundColor = null, int resizeFactor = 10)
        {
            var allPoints = colorValues.Select(p => p.Key).ToList();
            var maxX = allPoints.Max(q => q.X) + 2;
            var maxY = allPoints.Max(q => q.Y) + 2;
            var bitmap = new Image<Rgba32>(maxX, maxY);
            foreach (var entry in colorValues)
            {
                bitmap[entry.Key.X, entry.Key.Y] = entry.Value;
            }
            if (backgroundColor != null)
            {
                bitmap.Mutate(x => x.BackgroundColor(backgroundColor.Value));
            }
            // resize
            bitmap.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(bitmap.Size.Width * resizeFactor, bitmap.Size.Height * resizeFactor),
                Sampler = KnownResamplers.NearestNeighbor,
                Mode = ResizeMode.Stretch
            }));
            //Task.Factory.StartNew(() =>
            //{
            bitmap.Save(filePath, new PngEncoder());
            bitmap.Dispose();
            //});
        }

        public static void PrintToImage(string filePath, IDictionary<Point, Color> colorValues, IEnumerable<(TileRange TileRange, Color Color)> rangeColors, Color? backgroundColor = null, int resizeFactor = 10)
        {
            var allPoints = colorValues.Select(p => p.Key).ToList();
            var maxX = allPoints.Max(q => q.X) + 2;
            var maxY = allPoints.Max(q => q.Y) + 2;
            var bitmap = new Image<Rgba32>(maxX, maxY);
            foreach (var entry in colorValues)
            {
                bitmap[entry.Key.X, entry.Key.Y] = entry.Value;
            }
            foreach (var rangeColorEntry in rangeColors)
            {
                for (var x = rangeColorEntry.TileRange.StartX; x < rangeColorEntry.TileRange.EndX; x++)
                {
                    if (!colorValues.ContainsKey(new Point(x, rangeColorEntry.TileRange.Y)))
                    {
                        bitmap[x, rangeColorEntry.TileRange.Y] = rangeColorEntry.Color;
                    }
                }
            }
            if (backgroundColor != null)
            {
                bitmap.Mutate(x => x.BackgroundColor(backgroundColor.Value));
            }
            // resize
            bitmap.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(bitmap.Size.Width * resizeFactor, bitmap.Size.Height * resizeFactor),
                Sampler = KnownResamplers.NearestNeighbor,
                Mode = ResizeMode.Stretch
            }));
            //Task.Factory.StartNew(() =>
            //{
            bitmap.Save(filePath, new PngEncoder());
            bitmap.Dispose();
            //});
        }

        internal static void OpenFile(string tempFilePath)
        {
            Process.Start(new ProcessStartInfo { FileName = tempFilePath, UseShellExecute = true });
        }
    }
}
