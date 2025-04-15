using System.Text.Json;
using Bogus;
using Dasein.Sketch.Models;

namespace Dasein.Sketch.Functions;

public static class ExternalSystems
{
    public static Contracts.Profile GetProfile()
        => new Faker<Contracts.Profile>()
            .CustomInstantiator(f =>
            {
                var score = f.Random.Int(0, 100);
                var offset = f.Date.Timespan(TimeSpan.FromDays(30));

                return new Contracts.Profile(
                    FirstName: f.Name.FirstName(),
                    LastName: f.Name.LastName(),
                    CurrentScore: score,
                    LastScored: DateTime.UtcNow - offset
                );
            });

}