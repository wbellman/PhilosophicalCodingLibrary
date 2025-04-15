using System.Text.Json;

using static System.Environment;

namespace Dasein.Sketch.Models;

public static class Contracts
{
    public record User(
        string FirstName,
        string LastName
    )
    {
        public string FullName => $"{FirstName} {LastName}";
    };

    public record Post(
        User User,
        string Content
    );

    public record Profile(
        string FirstName,
        string LastName,
        int CurrentScore,
        DateTime LastScored
    )
    {
        public override string ToString()
        => $"{FirstName} {LastName} ({CurrentScore}: {LastScored:yyyy-MM-dd})";
    }

    public record DebugPayload(
        int BaseScore,
        int FinalScore,
        int Threshold,
        bool Metaphysical,
        bool Valid,
        string AiResponseRaw,
        JsonElement? ParsedJson
    );

    public record EvaluationResult(bool Exists, DebugPayload? Debug);

    public record AiResponse(
        bool IsValidJson,
        bool IsMetaphysical,
        JsonElement? Parsed,
        bool Error = false
    )
    {
        public static AiResponse Failed => new(false, false, null, true);
        
        public string Verdict => IsMetaphysical ? "Metaphysical" : "Not Metaphysical";
        private string Validity => IsValidJson ? "Valid" : "Invalid";

        public override string ToString()
            => $"Response ({Validity}): Post is {Verdict}. {NewLine}Json:{NewLine}---{NewLine}{Parsed}{NewLine}";
    }
}