using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using aoc_2025.Structures;
using System.Drawing;

namespace aoc_2025.Solutions
{
    public class Solution06 : ISolution
    {
        private static Dictionary<char, OperationsEnum> operationMapping = new Dictionary<char, OperationsEnum>
        {
            {'+', OperationsEnum.Add  },
            {'*', OperationsEnum.Multiply }
        };

        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var operationsLine = lines.Last();
            var operations = operationsLine.Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).Select(o => operationMapping[o.First()]).ToList();

            var numberLines = lines.SkipLast(1).ToList();
            var numbersInLines = numberLines.Select(l => l.Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).Select(long.Parse).ToList()).ToList();

            var overAllResult = 0L;
            for (int operationIndex = 0; operationIndex < operations.Count; operationIndex++)
            {
                var operation = operations[operationIndex];
                var numbers = numbersInLines.Select(l => l[operationIndex]).ToList();
                var currentNumber = numbers.First();
                foreach (var nextNumber in numbers.Skip(1))
                {
                    switch (operation)
                    {
                        case OperationsEnum.Add:
                            currentNumber += nextNumber;
                            break;
                        case OperationsEnum.Multiply:
                            currentNumber *= nextNumber;
                            break;
                    }
                }
                overAllResult += currentNumber;

            }
            return overAllResult.ToString();

        }

        public string RunPartB(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);

            var operationsWithRange = GetOperationsWithRange(lines, lines.Max(l => l.Length));

            // select from numberlines char and y index and x index and group them by the operation tuples according to the next matching index?
            var numberLines = lines.SkipLast(1).Select(s => s.ToArray()).ToList();

            var charsWithPoints = numberLines.SelectMany((line, columnIndex) => line.Select((charInRow, rowIndex) => (charInRow, new Point(rowIndex, columnIndex)))).ToArray();
            var groupedByOperation = charsWithPoints.GroupBy(g => operationsWithRange.First(o => o.LongRange.IsInRange(g.Item2.X))).ToList();
            long sum = 0L;
            foreach (var operation in groupedByOperation)
            {
                sum += CalculateOperation(operation);
            }
            return sum.ToString();
        }

        private long CalculateOperation(IGrouping<OperationWithRange, (char charInRow, Point)> operation)
        {
            var groupedByX = operation.GroupBy(o => o.Item2.X).ToList();
            TryParseNumber(groupedByX.First(), out var currentSum);
            foreach (var groupedByXEntry in groupedByX.Skip(1))
            {
                if (!TryParseNumber(groupedByXEntry, out var currentNumber))
                {
                    continue;
                }
                switch (operation.Key.Operation)
                {
                    case OperationsEnum.Add:
                        currentSum += currentNumber;
                        break;
                    case OperationsEnum.Multiply:
                        currentSum *= currentNumber;
                        break;
                }
            }
            return currentSum;
        }

        private bool TryParseNumber(IGrouping<int, (char charInRow, Point)> groupedByXEntry, out long number)
        {
            var ordererdByY = groupedByXEntry.OrderBy(entry => entry.Item2.Y).Select(q => q.charInRow).ToList();

            var numberString = string.Join(string.Empty, ordererdByY);
            if (string.IsNullOrWhiteSpace(numberString))
            {
                number = 0;
                return false;
            }
            number = long.Parse(numberString);
            return true;
        }

        private static List<OperationWithRange> GetOperationsWithRange(string[] lines, int maxLength)
        {
            var operationLine = lines.Last();
            var operationLineChars = operationLine.ToList();
            var operationsWithIndex = operationLineChars.Select((currentChar, index) => (currentChar, index)).Where(currentPosition => operationMapping.ContainsKey(currentPosition.currentChar))
                .Select(entry => (Index: entry.index, Operation: operationMapping[entry.currentChar])).ToList();
            var operationWithRange = new List<OperationWithRange>();
            for (var operationIndex = 0; operationIndex < operationsWithIndex.Count; operationIndex++)
            {
                var currentOperation = operationsWithIndex[operationIndex];
                if (operationIndex == operationsWithIndex.Count - 1)
                {
                    operationWithRange.Add((currentOperation.Operation, new LongRange { Start = currentOperation.Index, End = maxLength }));
                    break;
                }
                var nextOperation = operationsWithIndex[operationIndex + 1];
                operationWithRange.Add((currentOperation.Operation, new LongRange { Start = currentOperation.Index, End = nextOperation.Index - 1 }));
            }
            return operationWithRange;
        }
    }
    enum OperationsEnum
    {
        Add,
        Multiply
    }

    internal record struct OperationWithRange(OperationsEnum Operation, LongRange LongRange)
    {
        public static implicit operator (OperationsEnum operation, LongRange longRange)(OperationWithRange value)
        {
            return (value.Operation, value.LongRange);
        }

        public static implicit operator OperationWithRange((OperationsEnum operation, LongRange longRange) value)
        {
            return new OperationWithRange(value.operation, value.longRange);
        }
    }
}