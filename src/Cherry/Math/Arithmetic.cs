namespace Cherry.Math
{
    public static class Arithmetic
    {
        /// <summary>
        /// Returns the greatest common divisor of given numbers. The sign of
        /// the numbers is ignored for the purposes of this calculation.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long GCD(long a, long b)
        {
            a = a < 0 ? -a : a;
            b = b < 0 ? -b : b;
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }
            return a | b;
        }

        /// <summary>
        /// Returns -1 if the number if negative, 1 if the number is positive,
        /// and 0 if the number is 0.
        /// </summary>
        /// <param name="integer"></param>
        /// <returns>-1 if the number if negative, 1 if the number is positive,
        /// and 0 if the number is 0.</returns>
        public static int Sign(this int integer)
        {
            if (integer < 0)
            {
                return -1;
            }
            else if (integer > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
