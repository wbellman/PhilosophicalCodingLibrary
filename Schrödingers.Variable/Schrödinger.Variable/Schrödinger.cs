using System.Linq.Expressions;

namespace Schrodinger.Variable;

public record LockedSchrodingerBox<T>(
    Func<T> WhenTrue,
    Func<T> WhenFalse,
    Func<bool> DefaultResolver
) : IReadOnly
{
    public T Value => DefaultResolver() ? WhenTrue() : WhenFalse();
}

// implement CRTP
public interface ISchrodingerBox<T>
{
    T Value { get; }
}

public interface IReadOnly;

public interface IOpenSchrodingerBox<TValue, TSelf> : ISchrodingerBox<TValue>
    where TSelf : IOpenSchrodingerBox<TValue, TSelf>
{
    Expression<Func<bool>> Resolver { get; }

    TSelf When(Expression<Func<bool>> test);

    TSelf And(Expression<Func<bool>> test);
    TSelf Or(Expression<Func<bool>> test);

    TSelf AndGroup(Func<TSelf, TSelf> group);
    TSelf OrGroup(Func<TSelf, TSelf> group);

    TSelf Clone();
}

public interface ILockable<out T> where T : IReadOnly
{
    T Lock();
}

public class OpenSchrodingerBox<TValue>(
    Func<TValue> whenTrue,
    Func<TValue> whenFalse,
    Expression<Func<bool>>? initialResolver = null
) : IOpenSchrodingerBox<TValue, OpenSchrodingerBox<TValue>>, ILockable<LockedSchrodingerBox<TValue>>
{
    public static Expression<Func<bool>> DefaultResolver => () => true;

    public Expression<Func<bool>> Resolver { get; private set; } = initialResolver ?? DefaultResolver;

    public TValue Value => Resolver.Compile().Invoke()
        ? whenTrue()
        : whenFalse();

    public LockedSchrodingerBox<TValue> Lock()
        => new(whenTrue, whenFalse, Resolver.Compile());

    public OpenSchrodingerBox<TValue> Clone()
        => new(whenTrue, whenFalse, Resolver);

    public OpenSchrodingerBox<TValue> Clean()
        => new(whenTrue, whenFalse);

    public OpenSchrodingerBox<TValue> When(
        Expression<Func<bool>> test
    )
    {
        Resolver = test;
        return this;
    }

    public OpenSchrodingerBox<TValue> And(
        Expression<Func<bool>> test
    )
    {
        Resolver = Combine(Resolver, test, Expression.AndAlso);
        return this;
    }

    public OpenSchrodingerBox<TValue> Or(
        Expression<Func<bool>> test
    )
    {
        Resolver = Combine(Resolver, test, Expression.OrElse);
        return this;
    }

    public OpenSchrodingerBox<TValue> AndGroup(
        Func<OpenSchrodingerBox<TValue>, OpenSchrodingerBox<TValue>> group
    )
    {
        Group(group, Expression.OrElse);
        return this;
    }

    public OpenSchrodingerBox<TValue> OrGroup(
        Func<OpenSchrodingerBox<TValue>, OpenSchrodingerBox<TValue>> group
    )
    {
        Group(group, Expression.OrElse);
        return this;
    }

    private void Group(
        Func<OpenSchrodingerBox<TValue>, OpenSchrodingerBox<TValue>> group,
        Func<Expression, Expression, BinaryExpression> combiner
    ) => Combine(
        Resolver,
        group(Clean()).Resolver,
        combiner
    );

    // Combine new expression to the existing resolver
    private static Expression<Func<bool>> Combine(
        Expression<Func<bool>> left,
        Expression<Func<bool>> right,
        Func<Expression, Expression, BinaryExpression> combiner
    )
        => Expression.Lambda<Func<bool>>(
            combiner(left.Body, right.Body)
        );
}