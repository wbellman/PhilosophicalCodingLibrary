namespace Dasein.Sketch.Functions;

public static class Systemic
{
    public static string GetApiKey() => Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
}