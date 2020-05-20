using UnityEngine;

namespace DataTypes
{
    /// <summary>
    /// Stores block location on the map 
    /// </summary>
    public class MapLocation
    {
        public int row;
        public int col;

        /// <summary>
        /// Constructor for MapLocation struct
        /// </summary>
        /// <param name="row">First coordinate</param>
        /// <param name="col">Secind coordinate</param>
        public MapLocation(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        
        public static MapLocation Zero => new MapLocation(0, 0);

        public Vector2 ToVector2() => new Vector2(row, col);

        public Vector3 ToWorldLoc(Plane plane)
        {
            if (plane == Plane.XY) return new Vector3(col, row, 0);
            if (plane == Plane.XZ) return new Vector3(col, 0, row);    
            if (plane == Plane.YZ) return new Vector3(0, row, col); 
            return Vector3.zero;
        }

        public static MapLocation operator -(MapLocation mL1, MapLocation mL2)
        {
            return new MapLocation(mL1.row - mL2.row, mL1.col - mL2.col);
        }

        public static MapLocation operator +(MapLocation mL1, MapLocation mL2)
        {
            return new MapLocation(mL1.row + mL2.row, mL1.col + mL2.col);
        }

        public override bool Equals(object obj)
        {
            if (obj is MapLocation) return Equals((MapLocation) obj);
            else return base.Equals(obj);
        }

        public bool Equals(MapLocation mapLocation)
        {
            if (mapLocation == null) return false;
            return row == mapLocation.row && col == mapLocation.col;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + row.GetHashCode();
            hash = hash * 23 + col.GetHashCode();

            return hash;
        }
    }
}