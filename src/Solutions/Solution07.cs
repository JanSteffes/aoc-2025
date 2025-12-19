using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using aoc_2025.Structures;
using System.Drawing;

namespace aoc_2025.Solutions
{
    public class Solution07 : ISolution
    {
        private const char StartChar = 'S';
        private const char SplitterChar = '^';
        private const char EmptyFieldChar = '.';

        public string RunPartA(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).Select(s => s.Select(c => c).ToArray()).ToArray();
            var myGrid = Grid<char>.FromLines(input);

            var startColumn = myGrid.GetColumnOfChar(StartChar, 0);
            var beamColumns = new HashSet<int>
            {
                startColumn
            };

            var hittedSplitters = 0;
            var maxRow = myGrid.MaxY;
            for (var currentRow = 1; currentRow < maxRow; currentRow++)
            {
                var splittersInLineIndexes = myGrid.GetColumnsOfChar(SplitterChar, currentRow);
                // get all columns matching current beams
                var beamsHittingSplitters = beamColumns.Where(beamColumn => splittersInLineIndexes.Contains(beamColumn)).ToList();
                // count those
                hittedSplitters += beamsHittingSplitters.Count;
                // remove beams that hit splitters
                beamColumns.RemoveWhere(beamsHittingSplitters.Contains);
                // split beams
                var newBeams = beamsHittingSplitters.SelectMany(beam => new[] { beam - 1, beam + 1 }).ToList();
                // distinct splitted beams and remove those at -1 and maxY+1
                var distinctedAndFiltered = newBeams.Distinct().Where(q => q != -1 && q != myGrid.MaxX + 1).ToList();
                foreach (var newBeam in distinctedAndFiltered)
                {
                    beamColumns.Add(newBeam);
                }
            }
            return hittedSplitters.ToString();
        }




        public string RunPartB(string inputData)
        {
            var inputLines = ParseUtils.ParseIntoLines(inputData);

            //var onlyPointsRegex = new Regex("^(\\.)+$");
            var inputArrays = inputLines
                //.Where(s => !onlyPointsRegex.IsMatch(s))
                .Select(s => s.Select(c => c).ToArray()).ToArray();

            var myGrid = Grid<char>.FromLines(inputArrays);

            var startColumn = myGrid.GetColumnOfChar(StartChar, 0);
            var timeLines = TimeLines(myGrid, new Point(startColumn, 1), []);

            return timeLines.ToString();

        }

        private long TimeLines(Grid<char> grid, Point pointToCheck, Dictionary<Point, long> memorizedPoints)
        {
            if (memorizedPoints.TryGetValue(pointToCheck, out var result))
            {
                return result;
            }
            var currentRow = pointToCheck.Y;
            var currentColumn = pointToCheck.X;
            if (currentRow == grid.MaxY)
            {
                return 1;
            }
            if (currentColumn == grid.MaxX + 1 || currentColumn == -1)
            {
                return 0;
            }
            var currentSpaceChar = grid.GetEntryAt(currentColumn, currentRow);
            return currentSpaceChar switch
            {
                SplitterChar => HandleSplitterCharAsync(grid, pointToCheck, memorizedPoints),
                EmptyFieldChar => HandleEmptyFieldChar(grid, pointToCheck, memorizedPoints),
                _ => 0,// should never occur!
            };
        }

        private long HandleSplitterCharAsync(Grid<char> grid, Point currentPoint, Dictionary<Point, long> memorizedPoints)
        {
            var leftTimelines = TimeLines(grid, new Point(currentPoint.X - 1, currentPoint.Y + 1), memorizedPoints);
            var rightTimeLines = TimeLines(grid, new Point(currentPoint.X + 1, currentPoint.Y + 1), memorizedPoints);
            var totalTimeLines = leftTimelines + rightTimeLines;
            memorizedPoints.Add(currentPoint, totalTimeLines);
            return totalTimeLines;
        }

        private long HandleEmptyFieldChar(Grid<char> grid, Point point, Dictionary<Point, long> memorizedPoints)
        {
            return TimeLines(grid, new Point(point.X, point.Y + 1), memorizedPoints);
        }
    }



    public static class GridFindExtensions
    {
        public static IEnumerable<int> GetColumnsOfChar(this Grid<char> grid, char charToSearch, int row)
        {
            var maxX = grid.MaxX;
            for (var columnIndex = 0; columnIndex < maxX; columnIndex++)
            {
                if (grid.GetEntryAt(columnIndex, row) == charToSearch)
                {
                    yield return columnIndex;
                }
            }
        }

        public static int GetColumnOfChar(this Grid<char> grid, char charToSearch, int row)
        {
            var maxX = grid.MaxX;
            for (var columnIndex = 0; columnIndex < maxX; columnIndex++)
            {
                if (grid.GetEntryAt(columnIndex, row) == charToSearch)
                {
                    return columnIndex;
                }
            }
            throw new KeyNotFoundException($"Could not find char {charToSearch} in row {row}");
        }
    }
}