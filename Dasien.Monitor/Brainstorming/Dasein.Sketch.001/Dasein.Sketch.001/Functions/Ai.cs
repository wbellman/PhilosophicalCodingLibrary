using System.Net.Http.Headers;
using System.Text.Json;
using Dasein.Sketch.Models;

namespace Dasein.Sketch.Functions;

public static class Ai
{
    private static readonly string SystemMessage =
        """
        You are a digital philosopher, focused on evaluating user posted content to determine if their post demonstrates awareness or not.
        You should make the determination based on Heidegger's concept of Dasein.

        You should always respond only in json with the following payload:
        { "was_metaphysical": false }
        """;

    public static async Task<string> EvaluatePost(Contracts.Post post)
    {
        using var http = new HttpClient();

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", Systemic.GetApiKey()
        );

        var payload = new
        {
            model = "gpt-4.1-2025-04-14",
            messages = new[]
            {
                new { role = "system", content = SystemMessage },
                new { role = "user", content = post.Content }
            },
            temperature = 0.3
        };

        var response = await http.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            new StringContent(
                JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            )
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;
    }
    
    public static Contracts.AiResponse ParseResponse(string raw)
    {
        try
        {
            var parsed = JsonDocument.Parse(raw);
            var root = parsed.RootElement;

            return root.TryGetProperty("was_metaphysical", out var value)
                ? new Contracts.AiResponse(true, value.GetBoolean(), root)
                : new Contracts.AiResponse(false, false, root);
        }
        catch
        {
            // Hard return for POC only
            return Contracts.AiResponse.Failed;
        }
    }
}