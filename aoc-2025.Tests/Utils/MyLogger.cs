using System.Diagnostics;

namespace aoc_2025.Tests.Utils
{
    internal class MyLogger : IMyLogger
    {
        public void Log(string message, LogSeverity logSeverity)
        {
            Debug.WriteLine(message);
        }
    }
}
