namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Describes the position of a point with respect to an interval.
    /// </summary>
    public enum PointPosition
    {
        /// <summary>
        /// The point is less than all points in the interval.
        /// </summary>
        LOWER,
        /// <summary>
        /// The point is at the lower endpoint of the interval.
        /// The interval's lower endpoint must be inclusive.
        /// </summary>
        AT_LOWER_ENDPOINT,
        /// <summary>
        /// The point is strictly within the endpoints of the interval.
        /// </summary>
        WITHIN,
        /// <summary>
        /// The point is at the upper endpoint of the interval.
        /// The interval's upper endpoint must be inclusive.
        /// </summary>
        AT_UPPER_ENDPOINT,
        /// <summary>
        /// The point is greater than all points in the interval.
        /// </summary>
        UPPER
    }
}
