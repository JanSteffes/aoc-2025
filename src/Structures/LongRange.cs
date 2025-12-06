namespace aoc_2025.Structures
{
    public class LongRange
    {
        public long Start { get; set; }

        public long End { get; set; }

        public static LongRange FromString(string s)
        {
            var values = s.Split('-');
            return new LongRange
            {
                Start = long.Parse(values[0]),
                End = long.Parse(values[1])
            };
        }

        public override string ToString()
        {
            return Start + " to " + End;
        }
    }
}
