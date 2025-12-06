using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using aoc_2025.StructureExtensions.RangeExtensions;
using aoc_2025.Structures;

namespace aoc_2025.Solutions
{
    public class Solution05 : ISolution
    {
        const char rangeIndicator = '-';

        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var ranges = GetRanges(lines);

            var valueLines = lines.Where(l => !l.Contains(rangeIndicator)).ToList();
            var values = valueLines.Select(l => long.Parse(l)).ToList();

            var validIds = values.Count(v => ranges.Any(r => r.IsInRange(v)));
            return validIds.ToString();
        }


        public string RunPartB(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var ranges = GetRanges(lines);

            var sortedByStart = ranges.OrderBy(r => r.Start).ThenBy(r => r.End).GetEnumerator();
            sortedByStart.MoveNext();
            var currentRange = sortedByStart.Current;
            var combinedRangesList = new List<LongRange> { currentRange };
            while (sortedByStart.MoveNext())
            {
                var nextRange = sortedByStart.Current;
                if (nextRange.Start <= currentRange.End)
                {
                    currentRange.End = Math.Max(currentRange.End, nextRange.End);
                }
                else
                {
                    currentRange = nextRange;
                    combinedRangesList.Add(currentRange);
                }
            }
            var sum = 0L;
            foreach (var range in combinedRangesList)
            {
                var numbersInRange = range.CountOfNumbersInRange();
                sum += numbersInRange;
            }
            return sum.ToString();
        }


        private static LongRange[] GetRanges(string[] lines)
        {
            var rangeLines = lines.Where(l => l.Contains(rangeIndicator)).ToList();
            var ranges = rangeLines.Select(LongRange.FromString).ToArray();
            return ranges;
        }
    }

    internal static class LongRangeExtensions
    {
        internal static long CountOfNumbersInRange(this LongRange range)
        {
            return range.End - range.Start + 1;
        }
    }
}