namespace DataTypes
{
    /// <summary>
    /// Indicates movement direction
    /// </summary>
    public struct MovementDirection
    {
        public AxisDirection xAxis;
        public AxisDirection yAxis;
        public AxisDirection zAxis;

        /// <summary>
        /// Constructor for MovementDirectionStruct struct
        /// </summary>
        /// <param name="xAxisDirection">Direction along x axis</param>
        /// <param name="yAxisDirection">Direction along y axis</param>
        /// <param name="zAxisDirection">Direction along z axis</param>
        public MovementDirection(AxisDirection xAxisDirection, AxisDirection yAxisDirection,
            AxisDirection zAxisDirection)
        {
            xAxis = xAxisDirection;
            yAxis = yAxisDirection;
            zAxis = zAxisDirection;
        }
    }
}