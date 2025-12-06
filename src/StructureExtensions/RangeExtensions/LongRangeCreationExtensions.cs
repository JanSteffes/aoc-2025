using aoc_2025.Structures;

namespace aoc_2025.StructureExtensions.RangeExtensions
{
    public static class LongRangeCreationExtensions
    {
        /// <summary>
        /// Create range from string like <min>-<max>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInRange(this LongRange range, long value)
        {
            return value >= range.Start && value <= range.End;
        }
    }
}
