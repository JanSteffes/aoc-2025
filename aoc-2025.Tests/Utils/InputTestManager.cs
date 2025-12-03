namespace aoc_2025.Tests.Utils
{
    internal class InputTestManager : ITestManager
    {
        private readonly IMyLogger logger;

        public InputTestManager(IMyLogger logger)
        {
            this.logger = logger;
        }

        public int[] GetAvailableTests()
        {
            return [];
        }

        public List<TestCase> Parse(int dayNumber)
        {
            var input = ReadInputFile(dayNumber);
            return [new TestCase { Input = input }];
        }

        private string ReadInputFile(int dayNumber)
        {
            string? basePath = FileUtils.FindProjectFolder();
            if (string.IsNullOrEmpty(basePath))
            {
                logger.Log("Input folder not found", LogSeverity.Error);
                return string.Empty;
            }

            basePath = basePath.Replace("aoc-2025.Tests", "src");
            string filePath = Path.Combine(basePath, "Inputs", $"input-{dayNumber.ToString().PadLeft(2, '0')}.txt");

            if (!File.Exists(filePath))
            {
                logger.Log("Input file not found", LogSeverity.Error);
                return string.Empty;
            }

            return File.ReadAllText(filePath);
        }
    }
}