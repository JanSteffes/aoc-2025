namespace aoc_2025;

public static class Consts
{
    public const int year = 2025;
    public const string baseUri = "https://adventofcode.com";
}

public enum Mode
{
    Repeat,
    Run,
    Test,
    Init,
    Check,
    ToggleGridPrint,
    Exit
}

public enum Part
{
    A,
    B
}

public enum ClientResponseType
{
    Success,
    Failure,
}

public enum LogSeverity
{
    Log,
    Error,
    Runner,
    Other
}
