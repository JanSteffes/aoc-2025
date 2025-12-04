using aoc_2025.GridUtils;
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
            var colorDict = new Dictionary<char, ConsoleColor>
            {
                { freeSymbol, ConsoleColor.Gray },
                { occupiedSymbol, ConsoleColor.White }
            };
            //myGrid.PrintColoredMap(colorDict);
            var maxCountToHave = 4;
            var count = 0;
            for (var x = 0; x < myGrid.MaxX; x++)
            {
                for (var y = 0; y < myGrid.MaxY; y++)
                {
                    var countUp = RoleCanBeRemoved(myGrid, x, y, freeSymbol, occupiedSymbol, maxCountToHave);
                    if (countUp)
                    {
                        count++;
                    }
                }
            }
            return count.ToString();
        }

        public string RunPartB(string inputData)
        {
            var input = ParseUtils.ParseIntoLines(inputData).Select(s => s.Select(c => c).ToArray()).ToArray();
            var myGrid = Grid<char>.FromLines(input);
            var freeSymbol = '.';
            var occupiedSymbol = '@';
            var maxCountToHave = 4;
            var count = 0;
            var colorDict = new Dictionary<char, ConsoleColor>
            {
                { freeSymbol, ConsoleColor.Gray },
                { occupiedSymbol, ConsoleColor.White }
            };
            //myGrid.PrintColoredMap(colorDict);
            while (GetRolesToRemove(myGrid, freeSymbol, occupiedSymbol, maxCountToHave) is List<Point> rolesToRemoveList && rolesToRemoveList.Count > 0)
            {
                count += rolesToRemoveList.Count;
                foreach (var role in rolesToRemoveList)
                {
                    myGrid.SetEntryAt(role, freeSymbol);
                }
                //myGrid.PrintColoredMap(colorDict);
            }
            return count.ToString();
        }

        private static List<Point> GetRolesToRemove(Grid<char> myGrid, char freeSymbol, char occupiedSymbol, int maxCountToHave)
        {
            var rolesToRemove = new List<Point>();
            for (var x = 0; x < myGrid.MaxX; x++)
            {
                for (var y = 0; y < myGrid.MaxY; y++)
                {
                    var countUp = RoleCanBeRemoved(myGrid, x, y, freeSymbol, occupiedSymbol, maxCountToHave);
                    if (countUp)
                    {
                        rolesToRemove.Add(new Point(x, y));
                    }
                }
            }
            return rolesToRemove;
        }

        private static bool RoleCanBeRemoved(Grid<char> myGrid, int x, int y, char freeSymbol, char occupiedSymbol, int maxCountToHave)
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
    }
}