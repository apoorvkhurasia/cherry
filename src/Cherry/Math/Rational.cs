﻿using System;

namespace Cherry.Math
{
    /// <summary>
    /// Represents a rational number.
    /// </summary>
    public readonly struct Rational : IComparable<Rational>, IEquatable<Rational>
        //IComparable<double>, IEquatable<double>
    {

        static Rational()
        {
            TypeConfiguration.RegisterNegativeInfinityInstance(
                NegativeInfinity);
            TypeConfiguration.RegisterNegativeInfinityInstance(
                PositiveInfinity);
        }

        public static Rational PositiveInfinity { get; } = new(1, 0);

        public static Rational NegativeInfinity { get; } = new(-1, 0);

        public static Rational Zero { get; } = new(0, 1);

        public static Rational NaN { get; } = new(0, 0);

        private Rational(long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public static Rational Of(long numerator, long denominator)
        {
            if (numerator == 0 && denominator != 0)
            {
                return Zero;
            }
            else if (denominator != 0)
            {
                long gcd = Arithmetic.GCD(numerator, denominator);
                unchecked
                {
                    //Standardise the sign to numerator
                    if (denominator < 0)
                    {
                        numerator = -numerator;
                        denominator = -denominator;
                    }
                    return new(numerator / gcd, denominator / gcd);
                }
            }
            else if (numerator == 0)
            {
                return NaN;
            }
            else
            {
                return numerator > 0 ? PositiveInfinity : NegativeInfinity;
            }
        }

        public long Numerator { get; }

        public long Denominator { get; }

        public bool IsPositiveInfinity => Denominator == 0 && Numerator > 0;

        public bool IsNegativeInfinity => Denominator == 0 && Numerator < 0;

        public bool IsNaN => Numerator == 0 && Denominator == 0;

        public override bool Equals(object? obj) =>
            obj is Rational r && Equals(r);

        public override int GetHashCode() =>
            HashCode.Combine(Numerator, Denominator);

        public override string ToString() => $"{Numerator}/{Denominator}";

        #region Arithmetic

        public Rational Add(Rational other)
        {
            //Optimisations
            if (this.IsNaN || other.IsNaN)
            {
                return NaN;
            }
            else if ((this.IsPositiveInfinity && !other.IsNegativeInfinity)
                || (other.IsPositiveInfinity && !this.IsNegativeInfinity))
            {
                return PositiveInfinity;
            }
            else if ((this.IsNegativeInfinity && !other.IsPositiveInfinity)
                || (other.IsNegativeInfinity && !this.IsPositiveInfinity))
            {
                return NegativeInfinity;
            }
            else if ((this.IsPositiveInfinity && other.IsNegativeInfinity)
                || (other.IsPositiveInfinity && this.IsNegativeInfinity))
            {
                return NaN;
            }

            /* This is our best shot at preventing an overflow but it can
             * still happen. */
            var gcd = Arithmetic.GCD(Denominator, other.Denominator);
            var den = (other.Denominator / gcd) * Denominator;
            var num = Numerator * (den / Denominator) + 
                other.Numerator * (den / other.Denominator);
            return Of(num, den);
        }

        public Rational Multiply(Rational by)
        {
            if (this.IsNaN || by.IsNaN)
            {
                return NaN;
            }
            else if ((this.IsPositiveInfinity || this.IsNegativeInfinity) 
                && by.Numerator == 0)
            {
                return NaN;
            }
            else if ((by.IsPositiveInfinity || by.IsNegativeInfinity)
               && this.Numerator == 0)
            {
                return NaN;
            }

            var gcd1 = Arithmetic.GCD(Numerator, by.Denominator);
            var gcd2 = Arithmetic.GCD(by.Numerator, Denominator);
            return new((Numerator / gcd1) * (by.Numerator / gcd2),
                (Denominator / gcd1) * (by.Denominator / gcd2));
        }

        public Rational Inverse() => Of(Denominator, Numerator);

        public Rational Divide(Rational by) => Multiply(by.Inverse());

        public static Rational operator +(Rational one, Rational two) =>
            one.Add(two);

        public static Rational operator -(Rational r) 
            => new(-r.Numerator, r.Denominator);

        public static Rational operator -(Rational one, Rational two) =>
            one.Add(-two);

        public static Rational operator *(Rational one, Rational by) =>
            one.Multiply(by);

        public static Rational operator /(Rational one, Rational by) =>
            one.Divide(by);

        #endregion

        #region Comparision to other Rationals

        public int CompareTo(Rational other)
        {
            if (other.Numerator == Numerator &&
                other.Denominator == Denominator)
            {
                return 0;
            }
            else if (this.IsPositiveInfinity)
            {
                return 1;
            }
            else if (this.IsNegativeInfinity)
            {
                return -1;
            }
            else
            {
                return ((double)this).CompareTo(other);
            }
        }

        public bool Equals(Rational other) => CompareTo(other) == 0;

        public static bool operator ==(Rational left, Rational right)
        {
            if (left.IsNaN || right.IsNaN)
            {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(Rational left, Rational right)
        {
            return !(left == right);
        }

        public static bool operator <(Rational left, Rational right)
        {
            if (left.IsNaN || right.IsNaN)
            {
                return false;
            }
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Rational left, Rational right)
        {
            if (left.IsNaN || right.IsNaN)
            {
                return false;
            }
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Rational left, Rational right)
        {
            if (left.IsNaN || right.IsNaN)
            {
                return false;
            }
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Rational left, Rational right)
        {
            if (left.IsNaN || right.IsNaN)
            {
                return false;
            }
            return left.CompareTo(right) >= 0;
        }

        #endregion

        //#region Comparision to doubles

        //public int CompareTo(double other)
        //{
        //    if (this.IsPositiveInfinity)
        //    {
        //        return other == double.PositiveInfinity ? 0 : 1;
        //    }
        //    else if (this.IsNegativeInfinity)
        //    {
        //        return other == double.NegativeInfinity ? 0 : -1;
        //    }
        //    else
        //    {
        //        return ((double)this).CompareTo(other);
        //    }
        //}

        //public bool Equals(double other) => ((double)this).Equals(other);

        //#endregion

        //#region Comparision to ints

        //public int CompareTo(int other)
        //{
        //    if (this.IsNaN)
        //    {
        //        return 1;
        //    }
        //    else if (this.IsPositiveInfinity)
        //    {
        //        return 1;
        //    }
        //    else if (this.IsNegativeInfinity)
        //    {
        //        return -1;
        //    }
        //    else
        //    {
        //        return ((double) Numerator/Denominator).CompareTo(other);
        //    }
        //}

        //public bool Equals(int other) => Denominator == 1 && Numerator == other;

        //#endregion

        //#region Comparision to uints

        //public int CompareTo(uint other) =>
        //    ((double)Numerator / Denominator).CompareTo(other);

        //public bool Equals(uint other) =>
        //    ((double)Numerator / Denominator).Equals(other);

        //#endregion

        //#region Comparision to longs

        //public int CompareTo(long other) =>
        //    ((double)Numerator / Denominator).CompareTo(other);

        //public bool Equals(long other) => 
        //    Denominator == 1 && Numerator == other;

        //#endregion

        //#region Comparision to ulongs

        //public int CompareTo(ulong other) =>
        //    ((double)Numerator / Denominator).CompareTo(other);

        //public bool Equals(ulong other) => 
        //    ((double)Numerator / Denominator).Equals(other);

        //#endregion

        #region Conversions

        public static implicit operator double(Rational v)
        {
            if (v.IsPositiveInfinity)
            {
                return double.PositiveInfinity;
            }
            else if (v.IsNegativeInfinity)
            {
                return double.NegativeInfinity;
            }
            else if (v.IsNaN)
            {
                return double.NaN;
            }
            return (double)v.Numerator / v.Denominator;
        }

        #endregion

    }
}