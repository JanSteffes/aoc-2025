namespace aoc_2025.Structures
{
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
}