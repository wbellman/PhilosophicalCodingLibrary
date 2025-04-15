using System.ComponentModel;
using Dasein.Sketch.Functions;
using Dasein.Sketch.Models;
using FsCheck.Xunit;

namespace Dasein.Sketch.Tests.Units;

[Category("Decay Calculation")]
public class DecayCalculation
{
    [Fact(DisplayName = "Score should increase when metaphysical post is given")]
    public void ScoreShouldIncrease_WhenMetaphysicalPostGiven()
    {
        // Arrange
        var profile = new Contracts.Profile(
            "Test",
            "User",
            50,
            DateTime.UtcNow.AddDays(-1)
        );

        // Act
        var result = DaseinUtilities.CalculateNewScore(profile, earnedPoints: 10);

        // Assert
        Assert.True(result > profile.CurrentScore);
    }

    [Fact(DisplayName = "Score should decrease when post is old")]
    public void ScoreShouldDecrease_WhenPostIsOld()
    {
        // Arrange
        var freshProfile = new Contracts.Profile(
            "Fresh",
            "User",
            80,
            DateTime.UtcNow.AddDays(-1)
        );

        var staleProfile = new Contracts.Profile(
            "Stale",
            "User",
            80,
            DateTime.UtcNow.AddDays(-90)
        );

        // Act
        var freshScore = DaseinUtilities.CalculateNewScore(freshProfile);
        var staleScore = DaseinUtilities.CalculateNewScore(staleProfile);

        // Assert
        Assert.True(staleScore < freshScore);
    }

    [Property]
    public bool ScoreNeverDecreases_WithNewPoints(int score, int minutesAgo, int earnedPoints)
    {
        // Arrange
        score = Math.Clamp(score, 0, 100);
        earnedPoints = Math.Clamp(earnedPoints, 1, 20);
        minutesAgo = Math.Clamp(minutesAgo, 10, 43200); // Up to 30 days

        var profile = new Contracts.Profile(
            "Prop",
            "Test",
            score,
            DateTime.UtcNow - TimeSpan.FromMinutes(minutesAgo)
        );

        // Act
        var newScore = DaseinUtilities.CalculateNewScore(profile, earnedPoints);

        // Assert
        return IsPositive(newScore) && ExceedsOrMatches(newScore, score); // Score should not decay below original baseline
        
        // ---------- Invariants
        // score never exceeds maximum adjusted score
        bool ExceedsOrMatches(float x, float y) => x >= y;

        // score is always positive
        bool IsPositive(float x) => x >= 0;
    }

    [Property]
    public bool ScoreDecay_IsBounded(int score)
    {
        // Arrange
        const int earnedPoints = 10;
        score = Math.Clamp(score, 0, 100);
        
        var maxScore = score + earnedPoints;
                   
        var profile = new Contracts.Profile(
            "Prop",
            "Decay",
            score,
            DateTime.UtcNow.AddDays(-30)
        );
        
        // Act
        var newScore = DaseinUtilities.CalculateNewScore(profile);

        // Assert
        return IsPositive(newScore) && DoesNotExceed(newScore, maxScore);
        
        // ---------- Invariants
        
        // score never exceeds maximum adjusted score
        bool DoesNotExceed(float x, float y) => x <= y;

        // score is always positive
        bool IsPositive(float x) => x >= 0;
    }
}