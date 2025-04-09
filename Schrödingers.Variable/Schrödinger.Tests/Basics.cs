using Schrodinger.Variable;

namespace Schrodinger.Tests;

public class SchrodingerBoxTests
{
    [Fact(DisplayName = "Basics (open): Value resolves correctly with DefaultResolver")]
    public void ValueResolvesCorrectlyWithDefaultResolver()
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            null
        );

        Assert.Equal("True case", box.Value);
    }

    [Fact(DisplayName = "Basics (open): Value resolves correctly with custom resolver")]
    public void ValueResolvesCorrectlyWithCustomResolver()
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => false
        );

        Assert.Equal("False case", box.Value);
    }

    [Fact(DisplayName = "Basics (open): Lock converts to LockedSchrodingerBox")]
    public void LockConvertsToLockedSchrodingerBox()
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => true
        );

        var lockedBox = box.Lock();

        Assert.Equal("True case", lockedBox.Value);
    }

    [Fact(DisplayName = "Basics (closed): Value remains immutable after locking")]
    public void LockedBoxValueRemainsImmutable()
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => true
        );

        var lockedBox = box.Lock();
        // Change the original box's resolver to ensure immutability
        box = new OpenSchrodingerBox<string>(
            () => "Changed true case",
            () => "Changed false case",
            () => false
        );

        Assert.Equal("True case", lockedBox.Value);
    }

    [Theory(DisplayName = "Basics (open): And combines conditions correctly")]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void AndCombinesConditionsCorrectly(bool condition1, bool condition2, bool expected)
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => condition1
        );

        var updatedBox = box.And(() => condition2);
        Assert.Equal(expected ? "True case" : "False case", updatedBox.Value);
    }

    [Theory(DisplayName = "Basics (open): Or combines conditions correctly")]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void OrCombinesConditionsCorrectly(bool condition1, bool condition2, bool expected)
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => condition1
        );

        var updatedBox = box.Or(() => condition2);
        Assert.Equal(expected ? "True case" : "False case", updatedBox.Value);
    }

    [Fact(DisplayName = "Basics (open): Clone preserves current state")]
    public void ClonePreservesCurrentState()
    {
        var box = new OpenSchrodingerBox<string>(
            () => "True case",
            () => "False case",
            () => true
        );

        var clone = box.Clone();
        Assert.Equal(box.Value, clone.Value);
    }
}
