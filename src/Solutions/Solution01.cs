using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions;

public class Solution01 : ISolution
{
    public string RunPartA(string inputData)
    {
        var rotations = ParseUtils.ParseIntoLines(inputData).Select(ParseRotation).ToList();
        var position = 50;
        var timesHitZero = 0;
        foreach (var rotation in rotations)
        {

            var rotationsLeft = rotation.Clicks;
            var rotated = 0;
            while (rotationsLeft > 0)
            {
                rotated++;
                if (rotation.Direction == DirectionEnum.Right)
                {
                    position++;
                    if (position > 99)
                    {
                        position = 0;
                    }
                }
                else
                {
                    position--;
                    if (position == -1)
                    {
                        position = 99;
                    }
                }
                rotationsLeft--;
            }
            if (position == 0)
            {
                timesHitZero++;
            }
        }
        return timesHitZero.ToString();
    }

    public string RunPartB(string inputData)
    {
        var rotations = ParseUtils.ParseIntoLines(inputData).Select(ParseRotation).ToList();
        var position = 50;
        var timesHitZero = 0;
        foreach (var rotation in rotations)
        {

            var rotationsLeft = rotation.Clicks;
            var rotated = 0;
            while (rotationsLeft > 0)
            {
                rotated++;
                if (rotation.Direction == DirectionEnum.Right)
                {
                    position++;
                    if (position > 99)
                    {
                        position = 0;
                    }
                }
                else
                {
                    position--;
                    if (position == -1)
                    {
                        position = 99;
                    }
                }
                if (position == 0)
                {
                    timesHitZero++;
                }
                rotationsLeft--;
            }
        }
        return timesHitZero.ToString();
    }

    private Rotation ParseRotation(string s)
    {
        var direction = s.StartsWith('R') ? DirectionEnum.Right : DirectionEnum.Left;
        var clicks = int.Parse(s[1..]);
        return new Rotation
        {
            Direction = direction,
            Clicks = clicks,
        };
    }
}

class Rotation
{
    public DirectionEnum Direction { get; set; }

    public int Clicks { get; set; }
}

enum DirectionEnum
{
    Left,
    Right
}