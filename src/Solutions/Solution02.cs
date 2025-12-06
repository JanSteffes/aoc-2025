using aoc_2025.Interfaces;
using aoc_2025.Structures;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace aoc_2025.Solutions;

public class Solution02 : ISolution
{
    public string RunPartA(string inputData)
    {
        var ranges = inputData.Split(',').Select(LongRange.FromString).ToList();
        var sum = 0L;
        foreach (var range in ranges)
        {
            var invalidIdsInRange = range.GetInvalidIds();
            foreach (var invalidId in invalidIdsInRange)
            {
                sum += invalidId;
            }
        }
        return sum.ToString();
    }

    // TODO make this faster!
    public string RunPartB(string inputData)
    {
        var ranges = inputData.Split(',').Select(LongRange.FromString).ToList();
        var sum = 0L;
        var invalidIdsBag = new ConcurrentBag<long>();
        Parallel.ForEach(ranges, range =>
        {
            var invalidIdsInRange = range.GetMoreInvalidIdsSlow();
            foreach (var invalidId in invalidIdsInRange)
            {
                invalidIdsBag.Add(invalidId);
            }
        });
        sum = invalidIdsBag.Sum();
        return sum.ToString();
    }

}

internal static class RangeExtensions
{
    internal static List<long> GetInvalidIds(this LongRange range)
    {
        var invalidIds = new List<long>();
        for (var currentNumber = range.Start; currentNumber <= range.End; currentNumber++)
        {
            var asString = currentNumber.ToString();
            var length = asString.Length;
            if (length % 2 != 0)
            {
                continue;
            }
            var size = length / 2;
            var left = asString.Substring(0, size);
            var right = asString.Substring(size);
            if (left == right)
            {
                invalidIds.Add(currentNumber);
            }
        }
        return invalidIds;
    }

    internal static List<long> GetMoreInvalidIdsSlow(this LongRange range)
    {
        var invalidIds = new List<long>();
        for (var currentNumber = range.Start; currentNumber <= range.End; currentNumber++)
        {
            var asString = currentNumber.ToString();
            var length = asString.Length;
            var maxLength = length / 2;
            var found = false;
            var current = 1;
            while (current <= maxLength && !found)
            {
                var regexString = $"^({asString.Substring(0, current)}){{2,}}$";
                var regex = new Regex(regexString);
                if (regex.IsMatch(asString))
                {
                    found = true;
                    invalidIds.Add(currentNumber);
                }
                current++;
            }
        }
        return invalidIds;
    }
}
