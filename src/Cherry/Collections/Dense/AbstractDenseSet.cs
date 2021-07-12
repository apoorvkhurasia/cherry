using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Base implementation for dense sets. Common sense but slightly
    /// expensive implementations. Base classes can improve them when they
    /// are able to utilise the additional information only available to them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractDenseSet<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {
        public virtual bool IsEmpty => 
            !AsDisjointIntervals().Any(i => !i.IsEmpty);

        public abstract ImmutableList<DenseInterval<T>> AsDisjointIntervals();

        public virtual IDenseOrderedSet<T> Complement()
        {
            if (IsEmpty)
            {
                return DenseInterval<T>.UniverseInstance;
            }
            var myIntervals = 
                new Queue<DenseInterval<T>>(AsDisjointIntervals());
            var complementIntervals = 
                new List<DenseInterval<T>>(myIntervals.Count + 1);
            
            while (myIntervals.TryDequeue(out var interval))
            {
                if (interval.IsUniverse)
                {
                    return EmptySet<T>.Instance; //Game over
                }

                LowerEndpoint<T> start;
                if (interval.LowerEndpoint.IsInfinite)
                {
                    var startVal = interval.UpperEndpoint.Value!;
                    start = interval.UpperEndpoint.IsInclusive ?
                        LowerEndpoint<T>.Exclusive(startVal):
                        LowerEndpoint<T>.Inclusive(startVal);
                }
                else
                {
                    start = LowerEndpoint<T>.NegativeInfinity();
                }

                if (myIntervals.TryPeek(out var nextInterval))
                {
                    var endVal = nextInterval.LowerEndpoint.Value!;
                    var end = nextInterval.LowerEndpoint.IsInclusive ?
                        UpperEndpoint<T>.Exclusive(endVal) :
                        UpperEndpoint<T>.Inclusive(endVal);
                    complementIntervals.Add(new DenseInterval<T>(start, end));
                }
            }
            return UnionSet<T>.Of(complementIntervals);
        }

        public virtual bool Contains(T item)
        {
            foreach (var interval in AsDisjointIntervals())
            {
                var pos = interval.GetPosition(item);
                if (pos == Position.UPPER)
                {
                    return false;
                }
                else if (pos != Position.LOWER)
                {
                    return true;
                }
            }
            return false; //We will never reach here
        }

        public virtual IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return UnionSet<T>.Of(new[] { this, other });
        }

        public virtual IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            var remDisjointIntervals = new List<IDenseOrderedSet<T>>();
            foreach (var interval in AsDisjointIntervals())
            {
                foreach (var dj in other.AsDisjointIntervals())
                {
                    var intersection = dj.Intersect(interval);
                    if (!intersection.IsEmpty)
                    {
                        remDisjointIntervals.Add(intersection);
                    }
                    else if (dj.LowerEndpoint >= interval.UpperEndpoint)
                    {
                        break;
                    }
                }
            }
            return UnionSet<T>.Of(remDisjointIntervals);
        }

        public virtual bool IsProperSubsetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.AsDisjointIntervals().All(dji =>
                this.AsDisjointIntervals().Any(
                    thisDji => dji.IsProperSubsetOf(thisDji)
                )
            );
        }

        public virtual bool IsProperSupersetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.AsDisjointIntervals().All(dji =>
                this.AsDisjointIntervals().Any(
                    thisDji => dji.IsProperSubsetOf(thisDji)
                )
            );
        }

        public virtual bool IsSubsetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.AsDisjointIntervals().All(dji =>
                this.AsDisjointIntervals().Any(
                    thisDji => dji.IsSubsetOf(thisDji)
                )
            );
        }

        public virtual bool IsSupersetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            var otherIntervals = other.AsDisjointIntervals();
            return this.AsDisjointIntervals().All(dji =>
             otherIntervals.Any(o => dji.IsSubsetOf(o)));
        }

        public virtual bool Overlaps(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return AsDisjointIntervals().Any(i => i.Overlaps(other));
        }

        public virtual bool SetEquals(IDenseOrderedSet<T> other) 
        {
            SE.RequireNonNull(other, nameof(other));
            return IsSubsetOf(other) && other.IsSubsetOf(this);
        }

    }
}
