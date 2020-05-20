using System;
using System.Collections.Generic;

namespace DataTypes
{
    [Serializable]
    public class ConnectionPointPositionDirections
    {
        public List<GravitationalPlane> gravitationalPlanes;
        public List<AxisPositionDirection> axisPositionDirections;

        public ConnectionPointPositionDirections(List<GravitationalPlane> gravitationalPlanes, List<AxisPositionDirection> axisPositionDirections)
        {
            this.gravitationalPlanes = gravitationalPlanes;
            this.axisPositionDirections = axisPositionDirections;
        }
    }
}