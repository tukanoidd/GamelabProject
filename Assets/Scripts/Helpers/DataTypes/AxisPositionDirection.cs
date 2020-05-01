using System;

namespace DataTypes
{
    /// <summary>
    /// Stores position and direction of the object on another object's axis
    /// </summary>
    [Serializable]
    public struct AxisPositionDirection
    {
        public Axis axis;
        public AxisDirection dir;

        /// <summary>
        /// Constructor for AxisPositionDirection struct
        /// </summary>
        /// <param name="axis">Axis name</param>
        /// <param name="axisDirection">Axis direction</param>
        public AxisPositionDirection(Axis axis, AxisDirection axisDirection)
        {
            this.axis = axis;
            dir = axisDirection;
        }

        public static AxisDirection GetDirection(float val)
        {
            if (val > 0) return AxisDirection.Positive;
            if (Math.Abs(val) < 0.05f) return AxisDirection.Zero;
            return AxisDirection.Negative;
        }

        public static int NormalizeDirection(AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.Positive:
                    return 1;
                case AxisDirection.Negative:
                    return -1;
                default:
                    return 0;
            }
        }
    }
}