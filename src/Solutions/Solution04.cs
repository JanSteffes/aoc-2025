using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using System.Drawing;

namespace aoc_2025.Solutions
{
    public class Solution04 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).Select(s => s.Select(c => c).ToArray()).ToArray();
            var myGrid = Grid<char>.FromLines(input);
            var freeSymbol = '.';
            var occupiedSymbol = '@';
            var maxCountToHave = 4;
            //TODO: foreach entry, check positions x-1,y-1 to x+1,y+1 how many roles there are
            // if < 4, increase count
            var count = 0;
            for (var x = 0; x < myGrid.MaxX; x++)
            {
                for (var y = 0; y < myGrid.MaxY; y++)
                {
                    var countUp = Check(myGrid, x, y, freeSymbol, occupiedSymbol, maxCountToHave);
                    if (countUp)
                    {
                        count++;
                    }
                }
            }
            return count.ToString();
        }

        private bool Check(Grid<char> myGrid, int x, int y, char freeSymbol, char occupiedSymbol, int maxCountToHave)
        {
            var currentSymbol = myGrid.GetEntryAt(x, y);
            if (currentSymbol == freeSymbol)
            {
                return false;
            }
            var count = 0;
            for (var currentX = x - 1; currentX <= x + 1; currentX++)
            {
                for (var currentY = y - 1; currentY <= y + 1; currentY++)
                {
                    if (currentX == x && currentY == y)
                    {
                        continue;
                    }
                    if (!myGrid.HasEntryAt(currentX, currentY))
                    {
                        continue;
                    }
                    if (myGrid.GetEntryAt(currentX, currentY) == occupiedSymbol)
                    {
                        count++;
                        if (count == maxCountToHave)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public string RunPartB(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).Select(s => s.Select(c => c).ToArray()).ToArray();
            var myGrid = Grid<char>.FromLines(input);
            var freeSymbol = '.';
            var occupiedSymbol = '@';
            var maxCountToHave = 4;
            //TODO: foreach entry, check positions x-1,y-1 to x+1,y+1 how many roles there are
            // if < 4, increase count
            var count = 0;
            while (GetRolesToRemove(myGrid, freeSymbol, occupiedSymbol, maxCountToHave) is List<Point> rolesToRemoveList && rolesToRemoveList.Count > 0)
            {
                count += rolesToRemoveList.Count;
                foreach (var role in rolesToRemoveList)
                {
                    myGrid.SetEntryAt(role, freeSymbol);
                }
            }
            return count.ToString();
        }

        private List<Point> GetRolesToRemove(Grid<char> myGrid, char freeSymbol, char occupiedSymbol, int maxCountToHave)
        {
            var rolesToRemove = new List<Point>();
            for (var x = 0; x < myGrid.MaxX; x++)
            {
                for (var y = 0; y < myGrid.MaxY; y++)
                {
                    var countUp = Check(myGrid, x, y, freeSymbol, occupiedSymbol, maxCountToHave);
                    if (countUp)
                    {
                        rolesToRemove.Add(new Point(x, y));
                    }
                }
            }
            return rolesToRemove;
        }
    }

    public class Grid<T>
    {
        /// <summary>
        /// holds rows with columns, e.g. 1(x) , 5(y) is GrindEntris[1][5]
        /// </summary>
        public T[][] GridEntries { get; }

        public int MaxX { get; }

        public int MaxY { get; }

        public Grid(int rows, int columns)
        {
            MaxY = columns;
            MaxX = rows;
            GridEntries = new T[columns][];
            for (var column = 0; column < columns; column++)
            {
                GridEntries[column] = new T[rows];
            }
        }

        public T GetEntryAt(int x, int y)
        {
            return GridEntries[y][x];
        }

        public void SetEntryAt(int x, int y, T entry)
        {
            GridEntries[y][x] = entry;
        }

        public static Grid<T> FromLines(T[][] lines)
        {
            var y = lines.Length;
            var x = lines[0].Length;
            var grid = new Grid<T>(x, y);
            var currentY = 0;
            while (currentY < y)
            {
                var currentX = 0;
                while (currentX < x)
                {
                    grid.SetEntryAt(currentX, currentY, lines[currentY][currentX]);
                    currentX++;
                }
                currentY++;
            }
            return grid;
        }

        internal bool HasEntryAt(int x, int y)
        {
            return x >= 0 && x < MaxX && y >= 0 && y < MaxY;
        }
    }

    public static class GridExtensions
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