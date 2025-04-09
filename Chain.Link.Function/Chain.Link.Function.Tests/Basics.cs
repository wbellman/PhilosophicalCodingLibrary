namespace Chain.Link.Function.Tests;

public class ChainLinkFunctionTests
{
    [Fact(DisplayName = "Basics: Pipeline applies single transformation correctly")]
    public void PipelineAppliesSingleTransformationCorrectly()
    {
        var generator = ChainLinkFunction.Start<int>()
            .Transform(x => x + 1)
            .Lock();

        var result = generator(5).First(); // Apply transformations
        Assert.Equal(6, result);
    }

    [Fact(DisplayName = "Basics: Pipeline applies multiple transformations correctly")]
    public void PipelineAppliesMultipleTransformationsCorrectly()
    {
        var generator = ChainLinkFunction.Start<int>()
            .Transform(x => x + 1)
            .Transform(x => x * 2)
            .Lock();

        var actual = generator(5).ToList().Last();
        Assert.Equal(12, actual);
    }

    [Fact(DisplayName = "Basics: Lock creates immutable pipeline")]
    public void LockCreatesImmutablePipeline()
    {
        var builder = ChainLinkFunction.Start<int>()
            .Transform(x => x + 1);

        var generator = builder.Lock();

        // Attempt to modify after locking
        builder.Transform(x => x * 2);

        var result = generator(5).First(); // Original generator remains unchanged
        Assert.Equal(6, result);
    }

    [Theory(DisplayName = "Basics: (Finite) Transformation applies each in pipeline")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void TransformationFiniteAppliesEach(int initial)
    {
        int[] expected =
        [
            initial + 1,
            initial + 1 + 2,
            initial + 1 + 2 + 3
        ];

        var generator = ChainLinkFunction.Start<int>()
                .Transform(x => x + 1)
                .Transform(x => x + 2)
                .Transform(x => x + 3)
                .Lock()
            ;

        var values = generator(initial); // Original generator remains unchanged

        var at = 0;

        foreach (var value in values)
        {
            Assert.Equal(expected[at++], value);
        }
    }

    [Theory(DisplayName = "Basics: (Infinite) Transformation applies each in pipeline")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void TransformationInfiniteAppliesEach(int initial)
    {
        int[] expected =
        [
            initial + 1,
            initial + 1 + 2,
            initial + 1 + 2 + 3,
            initial + 1 + 2 + 3 + 1,
            initial + 1 + 2 + 3 + 1 + 2,
            initial + 1 + 2 + 3 + 1 + 2 + 3,
            initial + 1 + 2 + 3 + 1 + 2 + 3 + 1,
            initial + 1 + 2 + 3 + 1 + 2 + 3 + 1 + 2
        ];

        var generator = ChainLinkFunction.Start<int>()
                .Transform(x => x + 1)
                .Transform(x => x + 2)
                .Transform(x => x + 3)
                .Lock()
            ;

        var values = generator(initial).Take(expected.Length); // Original generator remains unchanged

        var at = 0;

        foreach (var value in values)
        {
            Assert.Equal(expected[at++], value);
        }
    }

    [Theory(DisplayName = "Basics: Transformation applies correctly with varying inputs")]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    public void TransformationAppliesCorrectlyWithVaryingInputs(int input, int expected)
    {
        var generator = ChainLinkFunction.Start<int>()
            .Transform(x => x + 1)
            .Lock();

        var result = generator(input).First();
        Assert.Equal(expected, result);
    }
}