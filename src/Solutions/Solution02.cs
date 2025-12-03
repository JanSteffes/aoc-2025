using aoc_2025.Interfaces;
using System.Text.RegularExpressions;

namespace aoc_2025.Solutions;

public class Solution02 : ISolution
{
    public string RunPartA(string inputData)
    {
        var ranges = inputData.Split(',').Select(Range.FromString).ToList();
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

    public string RunPartB(string inputData)
    {
        var ranges = inputData.Split(',').Select(Range.FromString).ToList();
        var sum = 0L;
        foreach (var range in ranges)
        {
            var invalidIdsInRange = range.GetMoreInvalidIds();
            foreach (var invalidId in invalidIdsInRange)
            {
                sum += invalidId;
            }
        }
        return sum.ToString();
    }

}

class Range
{
    public long Start { get; set; }

    public long End { get; set; }

    public static Range FromString(string s)
    {
        var values = s.Split('-');
        return new Range
        {
            Start = long.Parse(values[0]),
            End = long.Parse(values[1])
        };
    }

    public override string ToString()
    {
        return Start + " to " + End;
    }

    internal List<long> GetInvalidIds()
    {
        var invalidIds = new List<long>();
        for (var currentNumber = Start; currentNumber <= End; currentNumber++)
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

    internal List<long> GetMoreInvalidIds()
    {
        //Debug.WriteLine("CheckForInvalidIds in range " + this);
        var invalidIds = new List<long>();
        for (var currentNumber = Start; currentNumber <= End; currentNumber++)
        {
            //Debug.WriteLine("Test for number " + currentNumber);
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
                    //Debug.WriteLine("Found invalidId: " + asString);
                    found = true;
                    invalidIds.Add(currentNumber);
                }
                current++;
            }
        }
        return invalidIds;
    }
}