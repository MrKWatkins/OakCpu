namespace MrKWatkins.OakCpu.Z80.TestSuites.SingleStepTestCaseGenerator;

public static class Directory
{
    [PathReference]
    private static string Solution
    {
        get
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            while (!directory.EnumerateFiles("OakCpu.sln").Any())
            {
                directory = directory.Parent ?? throw new InvalidOperationException("Could not find the solution directory.");
            }

            return directory.FullName;
        }
    }

    [PathReference]
    public static string Output => Path.Combine(Solution, "MrKWatkins.OakCpu.Z80.TestSuites", "Instruction", "SingleStep", "TestCases");

    [PathReference]
    public static string JsonTemp => Path.Combine(Solution, "SingleStepTemp");
}