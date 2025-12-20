using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using SixLabors.ImageSharp;

namespace aoc_2025.Solutions
{
    public class Solution09 : ISolution
    {
        //private static readonly string BasePath = AppContext.BaseDirectory;
        //private readonly string NotesPath = Path.Combine(BasePath, "Nozizen", nameof(Solution09), "TestFiles");

        public string RunPartA(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).ToArray();

            var positionsOfRedTiles = input.Select(i => i.Split(",")).Select(p => new Point(int.Parse(p[0]), int.Parse(p[1]))).ToList();

            var rectangleSizes = CalculateSizes(positionsOfRedTiles);
            var orderedRectangles = rectangleSizes.OrderByDescending(d => d.Size).ToArray();
            var biggestRectangle = orderedRectangles.First();

            return biggestRectangle.Size.ToString();
        }

        public string RunPartB(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).ToArray();

            var positionsOfRedTiles = input.Select(i => i.Split(",")).Select(p => new Point(int.Parse(p[0]), int.Parse(p[1]))).ToList();
            // TODO rework (who would have guessed eh) -> green tiles should be every tile, but rather ranges, may for every y, then check if contains (reuse LongRange with dict key?)
            var positionsOfGreenFrameTiles = GetGreenFrameTilePositions(positionsOfRedTiles);
            var greenFillerTileRanges = GetGreenFillerTileRanges(positionsOfGreenFrameTiles);

            if (positionsOfRedTiles.Count > 20)
            {
                // only print for test
                PrintToFile(positionsOfRedTiles, positionsOfGreenFrameTiles, greenFillerTileRanges);
            }

            // do i even need the green tiles?
            // just check for all red tiles against all other red tiles if they can form a rectangle
            // either they are same x -> yes, same y -> yes
            // if not, check if there is some other red tile in the same x/y?
            // maybe keep the green tile ranges and check if theres a red tile or green tile range at x1/y2 and at x2,y1?
            // let's do it!
            var biggestAreaSize = 0L;
            foreach (var redTile in positionsOfRedTiles)
            {
                foreach (var otherRedTile in positionsOfRedTiles)
                {
                    if (otherRedTile == redTile)
                    {
                        continue;
                    }
                    if (redTile.X == otherRedTile.X)
                    {
                        var areaSize = Math.Abs(redTile.X - otherRedTile.X) + 1;
                        if (biggestAreaSize < areaSize)
                        {
                            biggestAreaSize = areaSize;
                        }
                    }
                    else if (redTile.Y == otherRedTile.Y)
                    {
                        var areaSize = Math.Abs(redTile.Y - otherRedTile.Y) + 1;
                        if (biggestAreaSize < areaSize)
                        {
                            biggestAreaSize = areaSize;
                        }
                    }
                    else
                    {
                        // check if theres a green or red tile at x1/y2 
                        var firstPointToCheck = new Point(redTile.X, otherRedTile.Y);
                        if (positionsOfRedTiles.Any(q => q == firstPointToCheck) || greenFillerTileRanges.Any(g => g.Contains(firstPointToCheck)))
                        {
                            // and if so if there's one at x2/y1
                            var secondPointToCheck = new Point(otherRedTile.X, redTile.Y);
                            if (positionsOfRedTiles.Any(q => q == secondPointToCheck) || greenFillerTileRanges.Any(g => g.Contains(secondPointToCheck)))
                            {
                                var length = Math.Abs(redTile.X - otherRedTile.X) + 1;
                                var width = Math.Abs(redTile.Y - otherRedTile.Y) + 1;
                                long areaSize = length * width;
                                if (biggestAreaSize < areaSize)
                                {
                                    biggestAreaSize = areaSize;
                                }
                            }
                        }
                    }
                }
            }

            return biggestAreaSize.ToString(); ;
        }

        private static void PrintToFile(List<Point> positionsOfRedTiles, List<Point> positionsOfGreenFrameTiles, List<TileRange> greenFillerTileRanges)
        {
            var redTilesColors = positionsOfRedTiles.ToDictionary(q => q, q => Color.Red);
            var greenTileColors = positionsOfGreenFrameTiles.ToDictionary(q => q, q => Color.Green);
            var combined = redTilesColors.Concat(greenTileColors).ToDictionary(q => q.Key, q => q.Value);
            var tempFilePath = Path.GetTempFileName() + ".png";
            OutputUtils.PrintToImage(tempFilePath, combined, greenFillerTileRanges.Select(g => (g, Color.Green)), Color.Black, 30);
            OutputUtils.OpenFile(tempFilePath);
        }

        private List<TileRange> GetGreenFillerTileRanges(List<Point> positionsOfGreenFrameTiles)
        {
            var greenFillerTileRanges = new List<TileRange>();
            // group by y, get lowest and highest x, do AddPointsBetween, remove all where already in positionsOfRedTiles
            var groupedByY = positionsOfGreenFrameTiles.GroupBy(p => p.Y);
            foreach (var yGroup in groupedByY)
            {
                var minX = yGroup.Min(g => g.X);
                var maxX = yGroup.Max(g => g.X);
                var range = new TileRange
                {
                    Y = yGroup.Key,
                    StartX = minX,
                    EndX = maxX
                };
                greenFillerTileRanges.Add(range);
            }
            return greenFillerTileRanges;
        }

        private List<(long Size, Point FirstPoint, Point SecondPoint)> CalculateSizes(List<Point> positionsOfRedTiles)
        {
            var result = new List<(long Size, Point FirstPoint, Point SecondPoint)>();
            foreach (var position in positionsOfRedTiles)
            {
                foreach (var otherPosition in positionsOfRedTiles)
                {
                    if (position == otherPosition)
                    {
                        continue;
                    }
                    var areaSize = CalculateArea(position, otherPosition);
                    result.Add((areaSize, position, otherPosition));
                }
            }
            return result;
        }

        private long CalculateArea(Point position, Point otherPosition)
        {
            var xLength = (long)Math.Abs(position.X - otherPosition.X) + 1;
            var yLength = (long)Math.Abs(position.Y - otherPosition.Y) + 1;
            return xLength * yLength;
        }

        private List<Point> GetGreenFrameTilePositions(List<Point> positionsOfRedTiles)
        {
            var points = new List<Point>();
            var currentPoint = positionsOfRedTiles.First();
            for (var index = 1; index < positionsOfRedTiles.Count; index++)
            {
                var nextPoint = positionsOfRedTiles[index];
                AddPointsBetween(points, currentPoint, nextPoint);
                currentPoint = nextPoint;
            }
            // last point to first
            var lastPoint = positionsOfRedTiles.Last();
            var firstPoint = positionsOfRedTiles.First();
            AddPointsBetween(points, lastPoint, firstPoint);
            return points;
        }

        private void AddPointsBetween(List<Point> points, Point currentPoint, Point nextPoint, List<Point>? pointsToIgnore = null)
        {
            // x or y has to be same
            var sameX = currentPoint.X == nextPoint.X;
            var sameY = currentPoint.Y == nextPoint.Y;
            if (sameX)
            {
                // travel y from next biggest y
                var nextY = Math.Min(currentPoint.Y, nextPoint.Y) + 1;
                var maxY = Math.Max(currentPoint.Y, nextPoint.Y);
                var x = currentPoint.X;
                while (nextY < maxY)
                {
                    var pointToAdd = new Point(x, nextY);
                    if (pointsToIgnore == null || !pointsToIgnore.Contains(pointToAdd))
                    {
                        points.Add(pointToAdd);
                    }
                    nextY++;
                }
            }
            else if (sameY)
            {
                var nextX = Math.Min(currentPoint.X, nextPoint.X) + 1;
                var maxX = Math.Max(currentPoint.X, nextPoint.X);
                var y = currentPoint.Y;
                while (nextX < maxX)
                {
                    var pointToAdd = new Point(nextX, y);
                    if (pointsToIgnore == null || !pointsToIgnore.Contains(pointToAdd))
                    {
                        points.Add(pointToAdd);
                    }
                    nextX++;
                }
            }
            else
            {
                throw new InvalidOperationException("This should never happen!");
            }
        }
    }

    public class TileRange
    {
        public int Y { get; init; }

        public int StartX { get; init; }

        public int EndX { get; init; }

        public bool Contains(Point point)
        {
            return point.Y == Y && point.X >= StartX && point.X <= EndX;
        }
    }
}