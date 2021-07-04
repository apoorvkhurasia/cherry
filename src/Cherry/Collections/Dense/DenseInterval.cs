using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Cherry.Collections.Dense
{
    public sealed class DenseInterval<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {

        private readonly ImmutableList<DenseInterval<T>> _disjointRep;

        public DenseInterval(LowerEndpoint<T> le, UpperEndpoint<T> ue)
        {
            LowerEndpoint = le;
            UpperEndpoint = ue;
            _disjointRep = ImmutableList.Create(this);
        }

        public static DenseInterval<T> UniverseInstance { get; } =
            new(LowerEndpoint<T>.NegativeInfinity(), 
                UpperEndpoint<T>.PositiveInfinity());

        public LowerEndpoint<T> LowerEndpoint { get; }

        public UpperEndpoint<T> UpperEndpoint { get; }

        public bool IsEmpty => LowerEndpoint.IsFinite
            && UpperEndpoint.IsFinite
            && Comparer<T>.Default.Compare(
                LowerEndpoint.Value, UpperEndpoint.Value) == 0;

        public bool Contains(T value) =>
            LowerEndpoint <= value && UpperEndpoint >= value;

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            var disjointIntervals = 
                new UnionSet<T>(
                    Enumerable.Concat(other.AsDisjointIntervals(),
                    Enumerable.Repeat(this, 1))).AsDisjointIntervals();
            if (disjointIntervals.Count == 1)
            {
                return disjointIntervals[0];
            }
            else
            {
                return new UnionSet<T>(disjointIntervals);
            }
        }

        public IDenseOrderedSet<T> Union(DenseInterval<T> other)
        {
            if (IsSubsetOf(other))
            {
                return other;
            }
            else if (other.IsSubsetOf(this))
            {
                return this;
            }
            
            if (LowerEndpoint.IsInclusive && UpperEndpoint.IsInclusive)
            {

            }
            
            if ((LowerEndpoint.IsInclusive || other.UpperEndpoint.IsInclusive)
                && (other.UpperEndpoint.IsFinite && 
                    LowerEndpoint == other.UpperEndpoint.Value!))
            {
                return new DenseInterval<T>(other.LowerEndpoint, UpperEndpoint);
            }
            else if (
                (other.LowerEndpoint.IsInclusive || UpperEndpoint.IsInclusive)
                && other.LowerEndpoint == UpperEndpoint.Value!)
            {
                return new DenseInterval<T>(LowerEndpoint, other.UpperEndpoint);
            }
            else
            {
                return new UnionSet<T>(new[] { this, other });
            }
        }

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> other) =>
            new UnionSet<T>(
                this.AsDisjointIntervals()
                    .SelectMany(
                        i => other.AsDisjointIntervals(),
                        (i1, i2) => i1.Intersect(i2))
                    .Where(interval => !interval.IsEmpty));

        public IDenseOrderedSet<T> Interset(DenseInterval<T> other)
        {
            if (IsSubsetOf(other))
            {
                return other;
            }
            else if (other.IsSubsetOf(this))
            {
                return this;
            }
            else if (other.UpperEndpoint > this.LowerEndpoint &&
                other.UpperEndpoint <= this.UpperEndpoint)
            {
                return new DenseInterval<T>(
                    this.LowerEndpoint, other.UpperEndpoint);
            }
            else if (this.UpperEndpoint > other.LowerEndpoint &&
                this.UpperEndpoint <= other.UpperEndpoint)
            {
                return new DenseInterval<T>(
                    other.LowerEndpoint, this.UpperEndpoint);
            }
            else
            {
                return EmptySet<T>.Instance;
            }
        }

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsProperSubsetOf);

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsProperSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint < LowerEndpoint &&
            other.UpperEndpoint > UpperEndpoint;

        public bool IsSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsSubsetOf);

        public bool IsSubsetOf(DenseInterval<T> other) =>
            other.LowerEndpoint <= this.LowerEndpoint &&
            other.UpperEndpoint >= this.UpperEndpoint;

        public bool IsSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint <= LowerEndpoint &&
            other.UpperEndpoint >= UpperEndpoint;

        public bool Overlaps(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(Overlaps);

        public bool Overlaps(DenseInterval<T> other) 
        {
            return !Intersect(other).IsEmpty;
        }

        public bool IsConnected(DenseInterval<T> other) => 
            LowerEndpoint <= other.UpperEndpoint
                && other.LowerEndpoint <= UpperEndpoint;

        public bool SetEquals(IDenseOrderedSet<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is DenseInterval<T> interval
                && LowerEndpoint.Equals(interval.LowerEndpoint)
                && UpperEndpoint.Equals(interval.UpperEndpoint);
        }

        public IDenseOrderedSet<T> Complement()
        {
            if (ReferenceEquals(this, UniverseInstance))
            {
                return EmptySet<T>.Instance;
            }
            else if (!LowerEndpoint.IsFinite)
            {
                Debug.Assert(UpperEndpoint.Value is not null);
                var le = UpperEndpoint.IsInclusive ?
                    LowerEndpoint<T>.FiniteExclusive(UpperEndpoint.Value) :
                    LowerEndpoint<T>.FiniteInclusive(UpperEndpoint.Value);

                return new DenseInterval<T>(
                    le, UpperEndpoint<T>.PositiveInfinity());
            }
            else if (!UpperEndpoint.IsFinite)
            {
                Debug.Assert(LowerEndpoint.Value is not null);
                var ue = LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.FiniteExclusive(LowerEndpoint.Value) :
                    UpperEndpoint<T>.FiniteInclusive(LowerEndpoint.Value);

                return new DenseInterval<T>(
                    LowerEndpoint<T>.NegativeInfinity(), ue);
            }
            else
            {
                Debug.Assert(UpperEndpoint.Value is not null);
                Debug.Assert(LowerEndpoint.Value is not null);
                var le = UpperEndpoint.IsInclusive ?
                       LowerEndpoint<T>.FiniteExclusive(UpperEndpoint.Value) :
                       LowerEndpoint<T>.FiniteInclusive(UpperEndpoint.Value);
                var ue = LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.FiniteExclusive(LowerEndpoint.Value) :
                    UpperEndpoint<T>.FiniteInclusive(LowerEndpoint.Value);
                return new UnionSet<T>(new[]
                {
                    new DenseInterval<T>(
                        LowerEndpoint<T>.NegativeInfinity(), ue),
                    new DenseInterval<T>(
                        le, UpperEndpoint<T>.PositiveInfinity())
                });
            }
        }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() => 
            _disjointRep;

    }
}
