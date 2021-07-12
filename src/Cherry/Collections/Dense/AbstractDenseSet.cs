using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Collections.Dense
{
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

        public bool Contains(T item)
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

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
