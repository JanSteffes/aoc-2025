using aoc_2025.Interfaces;
using aoc_2025.Tests.Utils;
using System.Diagnostics;

namespace aoc_2025.Tests
{
    [TestFixture]
    internal class GenericSolutionTests
    {
        [TestCase("1", 1, "A")]
        [TestCase("1", 1, "B")]
        [TestCase("2", 1, "A")]
        [TestCase("2", 1, "B")]
        [TestCase("3", 1, "A")]
        [TestCase("3", 1, "B")]
        //[TestCase("4", 1, "A")]
        //[TestCase("4", 1, "B")]
        //[TestCase("5", 1, "A")]
        //[TestCase("5", 1, "B")]
        //[TestCase("6", 1, "A")]
        //[TestCase("6", 1, "B")]
        //[TestCase("7", 1, "A")]
        //[TestCase("7", 1, "B")]
        //[TestCase("8", 1, "A")]
        //[TestCase("8", 1, "B")]
        //[TestCase("9", 1, "A")]
        //[TestCase("9", 1, "B")]
        //[TestCase("10", 1, "A")]
        //[TestCase("10", 1, "B")]
        //[TestCase("11", 1, "A")]
        //[TestCase("11", 1, "B")]
        //[TestCase("12", 1, "A")]
        //[TestCase("12", 1, "B")]
        public void TestCaseTests(int day, int testNumber, string part)
        {
            // arrange
            var solutionClass = GetSolutionClass(day);

            var testManager = new UnitTestManager(new MyLogger());
            var testCasesForDay = testManager.Parse(day);
            var testCase = testCasesForDay.First(test => test.TestNumber == testNumber);
            var expected = part == "A" ? testCase.AnswerA : testCase.AnswerB;
            var input = testCase.Input;

            // act
            var result = part == "A" ? solutionClass.RunPartA(input) : solutionClass.RunPartB(input);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("1", 1, "A", "1185", 1)]
        [TestCase("1", 1, "B", "6599", 1)]
        [TestCase("2", 1, "A", "23560874270", 1)]
        [TestCase("2", 1, "B", "44143124633", 1)]
        [TestCase("3", 1, "A", "17244", 1)]
        [TestCase("3", 1, "B", "171435596092638", 1)]
        //[TestCase("4", 1, "A", "TODO", 1)]
        //[TestCase("4", 1, "B", "TODO", 1)]
        //[TestCase("5", 1, "A", "TODO", 1)]
        //[TestCase("5", 1, "B", "TODO", 1)]
        //[TestCase("6", 1, "A", "TODO", 1)]
        //[TestCase("6", 1, "B", "TODO", 1)]
        //[TestCase("7", 1, "A", "TODO", 1)]
        //[TestCase("7", 1, "B", "TODO", 1)]
        //[TestCase("8", 1, "A", "TODO", 1)]
        //[TestCase("8", 1, "B", "TODO", 1)]
        //[TestCase("9", 1, "A", "TODO", 1)]
        //[TestCase("9", 1, "B", "TODO", 1)]
        //[TestCase("10", 1, "A", "TODO", 1)]
        //[TestCase("10", 1, "B", "TODO", 1)]
        //[TestCase("11", 1, "A", "TODO", 1)]
        //[TestCase("11", 1, "B", "TODO", 1)]
        //[TestCase("12", 1, "A", "TODO", 1)]
        //[TestCase("12", 1, "B", "TODO", 1)]        
        public void InputCaseTests(int day, int testNumber, string part, string expectedResult, int maxSecondsToRun)
        {
            // arrange
            var solutionClass = GetSolutionClass(day);

            var testManager = new InputTestManager(new MyLogger());
            var testCase = testManager.Parse(day);
            var input = testCase.First().Input;

            // act
            var sw = Stopwatch.StartNew();
            var result = part == "A" ? solutionClass.RunPartA(input) : solutionClass.RunPartB(input);
            sw.Stop();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(sw.Elapsed.TotalSeconds, Is.LessThanOrEqualTo(maxSecondsToRun));
            });
        }

        private static ISolution GetSolutionClass(int day)
        {
            var aoc2025Assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "aoc-2025");

            var typeToSearchFor = "Solution" + (day > 9 ? day : "0" + day);
            var solutionType = aoc2025Assembly.GetTypes().First(t => t.Name.Contains(typeToSearchFor));
            var instanceOftype = (ISolution)(Activator.CreateInstance(solutionType) ?? throw new Exception($"Failed to create instancer of type {solutionType.Name}!"));
            return instanceOftype;
        }
    }
}
