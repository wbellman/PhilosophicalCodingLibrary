using System.Diagnostics;
using System.Linq.Expressions;

namespace Chain.Link.Function;

public interface IChainLink<TSelf, TValue> where TSelf : IChainLink<TSelf, TValue>
{
    TSelf Transform(Expression<Func<TValue, TValue>> transform);
    TSelf Finite();
    TSelf Infinite();
    Func<TValue, IEnumerable<TValue>> Lock();
}

public class ChainLinkBuilder<T> : IChainLink<ChainLinkBuilder<T>, T>
{
    private List<Expression<Func<T, T>>> Chain { get; } = new();

    private bool InfiniteChain { get; set; }

    private Func<T, T> Compile()
    {
        return x => Chain.Aggregate(x, (current, expression) => expression.Compile()(current));
    }

    public ChainLinkBuilder<T> Transform(Expression<Func<T, T>> transform)
    {
        Chain.Add(transform);
        return this;
    }

    public ChainLinkBuilder<T> Infinite()
    {
        InfiniteChain = true;
        return this;
    }

    public ChainLinkBuilder<T> Finite()
    {
        InfiniteChain = false;
        return this;
    }

    public Func<T, IEnumerable<T>> Lock()
    {
        return Build();
    }

    private Func<T, IEnumerable<T>> Build()
    {
        var compiledChain = Chain
                .Select(x => x.Compile())
                .ToList()
                .AsReadOnly()
            ;

        var at = 0;
        
        IEnumerable<T> Generator(T x)
        {
            var last = x;
            while (true)
            {
                last = compiledChain[at++](last);
                yield return last;

                if (at != compiledChain.Count) continue;
                
                if (InfiniteChain)
                {
                    at = 0;
                    continue;
                }

                yield break;
            }
        }

        return Generator;
    }
}

public static class ChainLinkFunction
{
    public static ChainLinkBuilder<T> Start<T>() => new();
}