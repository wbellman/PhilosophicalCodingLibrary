using Dasein.Sketch.Models;

using static System.MathF;

namespace Dasein.Sketch.Functions;

public static class DaseinUtilities
{
    private const int HalfLifeDays = 30;
    private static readonly float HalfLifeMinutes = (float)TimeSpan.FromDays(HalfLifeDays).TotalMinutes;
    
    // We are starting with Log(2) as a simple adjustable fall-off
    private static readonly float DecayFactor = Log(2);

    public static Contracts.Profile AdjustScore(Contracts.Profile profile, int earnedPoints = 10)
        => profile with
        {
            CurrentScore = (int)Math.Round(CalculateNewScore(profile, earnedPoints)),
            LastScored = DateTime.UtcNow
        };

    public static float CalculateNewScore(Contracts.Profile profile, int earnedPoints = 10)
    {
        var minutesSinceLast = (float)(DateTime.UtcNow - profile.LastScored).TotalMinutes;

        var decayRate = DecayFactor / HalfLifeMinutes;
        var decayedScore = profile.CurrentScore * Exp(-decayRate * minutesSinceLast);

        var adjusted = decayedScore + earnedPoints;
        return adjusted;
    }
}