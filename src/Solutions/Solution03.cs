using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;

namespace aoc_2025.Solutions
{
    public class Solution03 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var batteryBanks = ParseUtils.ParseIntoLines(inputData).Select(BatteryBank.FromString).ToList();
            var sum = batteryBanks.Sum(b => b.GetMaxmimumJoltage());
            return sum.ToString();
        }

        public string RunPartB(string inputData)
        {
            var batteryBanks = ParseUtils.ParseIntoLines(inputData).Select(BatteryBank.FromString).ToList();
            const int batteriesToTurnOn = 12;
            var sum = batteryBanks.Sum(b => b.GetSuperMaxmimumJoltageFromRange(batteriesToTurnOn));
            return sum.ToString();
        }
    }


    class Battery
    {
        /// <summary>
        /// 1 - 9
        /// </summary>
        public int JoltageRating { get; set; }

        public static Battery FromChar(char c)
        {
            return new Battery
            {
                JoltageRating = int.Parse(c.ToString())
            };
        }

        public override string ToString()
        {
            return JoltageRating.ToString();
        }
    }

    class BatteryBank
    {
        public Battery[] Batteries { get; set; } = [];

        public List<Battery> TurnedOn { get; set; } = [];

        public static BatteryBank FromString(string batteryBankString)
        {
            return new BatteryBank
            {
                Batteries = [.. batteryBankString.Select(Battery.FromChar)],
            };
        }

        internal long GetSuperMaxmimumJoltageFromRange(int numberLength)
        {
            // x numbers
            // take the largest, ignoring the last x in index
            // from there, take the largest ignoring the last (index of largest)  in index and repeat till x numbers taken (recursive?)
            var remainingNumberLength = numberLength - 1;
            var highest = Batteries.SkipLast(remainingNumberLength).MaxBy(j => j.JoltageRating);
            var indexOfHighest = Batteries.IndexOf(highest);
            //Debug.WriteLine("Found highest " + hihghest + " at index " + indexOfHighest);
            var result = highest!.JoltageRating.ToString();
            while (remainingNumberLength-- > 0)
            {
                //Debug.WriteLine("Will skip last " + --indexToExclude);
                //Debug.WriteLine("Will take after first " + indexOfHighest);
                var next = Batteries.Skip(indexOfHighest + 1).SkipLast(remainingNumberLength).MaxBy(j => j.JoltageRating);
                //Debug.WriteLine("Found " + next);
                indexOfHighest = Batteries.IndexOf(next);
                result += next!.JoltageRating;
            }
            //Debug.WriteLine("SuperMaxmimumJoltage of bank " + this + " is " + result);
            return long.Parse(result);
        }

        internal long GetMaxmimumJoltage()
        {
            var batteryWithHighestNumber = Batteries.MaxBy(b => b.JoltageRating);
            Battery? batteryWithHighestFollowupNumber = null;
            var indexOfHighestBatteryNumber = Batteries.IndexOf(batteryWithHighestNumber);
            if (indexOfHighestBatteryNumber == Batteries.Length - 1)
            {
                // set as second highest
                batteryWithHighestFollowupNumber = batteryWithHighestNumber;
            }
            else
            {
                batteryWithHighestFollowupNumber = Batteries.Skip(indexOfHighestBatteryNumber).Except([batteryWithHighestNumber]).MaxBy(b => b!.JoltageRating);
            }
            if (batteryWithHighestNumber == batteryWithHighestFollowupNumber)
            {
                batteryWithHighestNumber = Batteries.Except([batteryWithHighestFollowupNumber]).MaxBy(b => b!.JoltageRating);
            }
            var result = long.Parse(batteryWithHighestNumber!.JoltageRating.ToString() + batteryWithHighestFollowupNumber!.JoltageRating.ToString());
            //Debug.WriteLine("Bank: " + this);
            //Debug.WriteLine("Highest Rating: " + result);
            return result;
        }

        public override string ToString()
        {
            return string.Join("", Batteries);
        }
    }
}