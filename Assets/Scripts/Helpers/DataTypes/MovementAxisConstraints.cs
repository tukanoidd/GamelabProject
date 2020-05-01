namespace DataTypes
{
    /// <summary>
    /// Indicates of a constraint along different axis for player movement
    /// </summary>
    public class MovementAxisConstraints
    {
        public bool x;
        public bool y;
        public bool z;

        /// <summary>
        /// Constructor for MovementAxisConstraint struct
        /// </summary>
        /// <param name="xAxisConstrained">Is movement constrained on x Axis</param>
        /// <param name="yAxisConstrained">Is movement constrained on y Axis</param>
        /// <param name="zAxisConstrained">Is movement constrained on z Axis</param>
        public MovementAxisConstraints(bool xAxisConstrained, bool yAxisConstrained, bool zAxisConstrained)
        {
            x = xAxisConstrained;
            y = yAxisConstrained;
            z = zAxisConstrained;
        }
    }
}