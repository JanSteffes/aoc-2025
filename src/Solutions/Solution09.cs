using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using System.Drawing;

namespace aoc_2025.Solutions
{
    public class Solution09 : ISolution
    {
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
            throw new NotImplementedException();
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
    }
}