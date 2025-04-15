using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dasein.Sketch.Functions;

// --- Static includes
using static System.Console;
using static Dasein.Sketch.Models.Contracts;

// --- Environment Configuration
OutputEncoding = Encoding.UTF8;

const bool debugEnabled = true;
const int threshold = 50;

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

// --- Incoming Data
var post = InputSimulation.GetPost();
WriteLine($"🤵 {post.User.FullName} Posted: {post.Content}");

var rawResponse = await Ai.EvaluatePost(post);
var aiResponse = Ai.ParseResponse(rawResponse);

WriteLine($"🔳 Raw: {rawResponse}");
WriteLine($"💬 Ai:  {aiResponse}");

if (aiResponse.Error)
{
    WriteLine($"💥 AI response was not understood: {rawResponse}");
    return;
}

WriteLine($"🖲️ AI philosopher's verdict: {aiResponse.Verdict}");

if (aiResponse.IsMetaphysical is false)
{
    WriteLine("😔 User is probably not self aware.");
    return;
}


WriteLine("😀 User seems self-aware!");

var profile = ExternalSystems.GetProfile();

WriteLine($"📓 Got profile: {profile}");

var adjustedProfile = DaseinUtilities.AdjustScore(profile);

var result = new EvaluationResult(
    aiResponse.IsMetaphysical,
    debugEnabled
        ? new DebugPayload(
            profile.CurrentScore,
            adjustedProfile.CurrentScore,
            threshold,
            aiResponse.IsMetaphysical,
            aiResponse.IsValidJson,
            rawResponse,
            aiResponse.Parsed
        )
        : null
);

var json = JsonSerializer.Serialize(result, options);

WriteLine($"🟢 Final Result: {json}");