using System;

namespace Cherry.Math
{
    /// <summary>
    /// Represents a rational number as a fraction. The maximum precision
    /// for numerator and denominator is that of <see cref="long" />. 
    /// Instances of this class can be implicitly converted to 
    /// <see cref="double"/>.
    /// </summary>
    public readonly struct Rational : 
        IComparable<Rational>, IEquatable<Rational>, IFormattable
    {

        static Rational()
        {
            TypeConfiguration.RegisterNegativeInfinityInstance(
                NegativeInfinity);
            TypeConfiguration.RegisterNegativeInfinityInstance(
                PositiveInfinity);
        }

        /// <summary>
        /// The fixed rational representing positive infinity. Is
        /// equivalent to <see cref="double.PositiveInfinity"/>.
        /// </summary>
        public static Rational PositiveInfinity { get; } = new(1, 0);

        /// <summary>
        /// The fixed rational representing negative infinity. Is
        /// equivalent to <see cref="double.NegativeInfinity"/>.
        /// </summary>
        public static Rational NegativeInfinity { get; } = new(-1, 0);

        /// <summary>
        /// The fixed rational representing the zero number.
        /// </summary>
        public static Rational Zero { get; } = new(0, 1);

        /// <summary>
        /// A dummy value which serves as a placeholder for non numeric
        /// values. Is equivalent to <see cref="double.NaN"/>
        /// </summary>
        public static Rational NaN { get; } = new(0, 0);

        private Rational(long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// Returns an instance of <see cref="Rational"/> which is 
        /// mathematically equivalent to the fraction obtained by the
        /// given numerator and the given denominator. 
        /// <remarks>
        /// <para>Please note that it is not guarenteed that the instance 
        /// will have the same numerator and denominator as the one you 
        /// supplied. For example, the returned instance could be simplified
        /// by cancelling common factors or the sign could be transferred 
        /// from the denominator to the numerator. 
        /// No specific assumptions should be made about the 
        /// simplifications performed.</para>
        /// <para>Some combinations of numerators and denominators will
        /// be assigned specific values as follows:
        /// <list type="bullet">
        ///     <item>0/0 will always be assigned <see cref="NaN"/></item>.
        ///     <item>Any positive numerator and 0 denominator will
        ///     be assigned <see cref="PositiveInfinity"/>.</item>
        ///     <item>Any negative numerator and 0 denominator will
        ///     be assigned <see cref="NegativeInfinity"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <returns>A <see cref="Rational"/> that is mathematically
        /// equivalent to the fraction <paramref name="numerator"/>/
        /// <paramref name="denominator"/>.</returns>
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

        /// <summary>
        /// The numerator of this instance.
        /// </summary>
        public long Numerator { get; }

        /// <summary>
        /// The denominator of this instance.
        /// </summary>
        public long Denominator { get; }

        /// <summary>
        /// Returns <see langword="true" /> if and only if this instance
        /// represents the special constant <see cref="PositiveInfinity"/>.
        /// <see langword="false"/> otherwise.
        /// </summary>
        public bool IsPositiveInfinity => Denominator == 0 && Numerator > 0;

        /// <summary>
        /// Returns <see langword="true" /> if and only if this instance
        /// represents the special constant <see cref="NegativeInfinity"/>.
        /// <see langword="false"/> otherwise.
        /// </summary>
        public bool IsNegativeInfinity => Denominator == 0 && Numerator < 0;

        /// <summary>
        /// Returns <see langword="true" /> if and only if this instance
        /// represents the special constant <see cref="NaN"/>.
        /// <see langword="false"/> otherwise.
        /// </summary>
        public bool IsNaN => Numerator == 0 && Denominator == 0;

        /// <summary>
        /// Returns <see langword="true" /> if and only if the given object
        /// is a <see cref="Rational"/> which is arithmatically equal to this 
        /// instance. <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true" /> if and only if the given object
        /// is a <see cref="Rational"/> which is arithmatically equal to this 
        /// instance. <see langword="false"/> otherwise.</returns>
        public override bool Equals(object? obj) =>
            obj is Rational r && Equals(r);

        /// <summary>
        /// A hash code for this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override int GetHashCode() =>
            HashCode.Combine(Numerator, Denominator);

        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString() => $"{Numerator}/{Denominator}";

        /// <summary>
        /// Formats the <see cref="Numerator"/> and <see cref="Denominator"/>
        /// with the given <paramref name="format"/> and 
        /// <paramref name="provider"/> and returns a string representing
        /// this instance.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies 
        /// culture-specific formatting information about this instance.</param>
        /// <returns>The string representation of the value of this instance 
        /// with <see cref="Numerator"/> and <see cref="Denominator"/> formatted
        /// as specified by format and provider.</returns>
        public string ToString(string? format, IFormatProvider? provider) =>
            $"{ Numerator.ToString(format, provider)}/${ Denominator.ToString(format, provider)}";

        #region Arithmetic

        /// <summary>
        /// Creates a new instance representing the sum of this
        /// instance and <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="Rational"/> to add.</param>
        /// <returns>The sum of this instance and the given instance.</returns>
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

        /// <summary>
        /// Creates a new instance representing the product of this
        /// instance and <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="Rational"/> to multiply.</param>
        /// <returns>The product of this instance and the given instance.
        /// </returns>
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
            
            /* The instances are already simplified. To prevent an overflow
             * try to simplify the resulting rational before performing
             * an actual multiplication. This does not avoid an overflow
             * all the time but is our best shot at preventing it. */
            var gcd1 = Arithmetic.GCD(Numerator, by.Denominator);
            var gcd2 = Arithmetic.GCD(by.Numerator, Denominator);
            return new((Numerator / gcd1) * (by.Numerator / gcd2),
                (by.Denominator / gcd1) * (Denominator / gcd2));
        }

        /// <summary>
        /// Creates a new instance representing the reciprocal of this
        /// instance.
        /// </summary>
        /// <returns>The reciprocal of this instance.</returns>
        public Rational Reciprocal() => Of(Denominator, Numerator);

        /// <summary>
        /// Creates a new instance obtained by dividing this instance by 
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="Rational"/> to divide by.</param>
        /// <returns>The result of dividing this instance by the given instance.
        /// </returns>
        public Rational Divide(Rational by) => Multiply(by.Reciprocal());

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
                var diff = this - other;
                if (diff.Numerator == 0)
                {
                    return 0;
                }
                else if (diff.Numerator < 0 ^ diff.Denominator < 0)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
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

        public static implicit operator Rational(long v) => Of(v, 1);

        #endregion

    }
}
