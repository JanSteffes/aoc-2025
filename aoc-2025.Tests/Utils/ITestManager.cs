namespace aoc_2025.Tests.Utils
{
    public interface ITestManager
    {
        List<TestCase> Parse(int dayNumber);
        int[] GetAvailableTests();
    }
}
