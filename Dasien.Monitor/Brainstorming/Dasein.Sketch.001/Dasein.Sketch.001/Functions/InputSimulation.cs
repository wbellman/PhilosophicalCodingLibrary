using Bogus;
using static Dasein.Sketch.Models.Contracts;


namespace Dasein.Sketch.Functions;

public static class InputSimulation
{
    private static Random Random => Random.Shared;

    // --- Content
    private static readonly string[] Content =
    [
        // 🧠 Metaphysical
        "Every action I take reshapes the version of myself I will become.",
        "Sometimes I question if my thoughts are mine or echoes of the world around me.",
        "To exist is not enough—I must choose to be aware of that existence.",
        "In solitude I find the sharpest mirror of who I really am.",
        "The weight of time reminds me that presence is a choice, not a default state.",

        // 🍕 Non-Metaphysical
        "Can’t believe the new burger place closes at 8. What is this, 1993?",
        "My dog just sneezed so hard he fell off the couch. 10/10 moment.",
        "New hoodie arrived. Softest thing I own. Might live in it forever.",
        "Woke up. Coffee. Traffic. Emails. Meetings. Repeat.",
        "Check out this playlist. Absolute bangers all the way down."
    ];

    private static string GetContent() => Content[Random.Next(Content.Length)];

    // --- Generators
    private static Faker<User> UserGenerator => new Faker<User>()
        .CustomInstantiator(f => new User(
            f.Name.FirstName(),
            f.Name.LastName()
        ));

    private static Faker<Post> PostGenerator => new Faker<Post>()
        .CustomInstantiator(f => new Post(
            UserGenerator.Generate(),
            GetContent()
        ));

    // --- Utility Function
    public static Post GetPost() => PostGenerator.Generate();
}