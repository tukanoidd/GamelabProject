using System;
using UnityEngine;

namespace DataTypes
{
    /// <summary>
    /// Stores block size in 3 dimensions
    /// </summary>
    [Serializable]
    public struct BlockSize
    {
        [Range(1, 5)] public int x;
        [Range(1, 5)] public int y;
        [Range(1, 5)] public int z;

        /// <summary>
        /// Constructor for BlockSize struct
        /// </summary>
        /// <param name="xSize">Size of the block on x axis</param>
        /// <param name="ySize">Size of the block on y axis</param>
        /// <param name="zSize">Size of the block on z axis</param>
        public BlockSize(int xSize, int ySize, int zSize)
        {
            x = xSize;
            y = ySize;
            z = zSize;
        }

        public int PlaneNormal(Plane plane)
        {
            switch (plane)
            {
                case Plane.XY: return z;
                case Plane.XZ: return y;
                case Plane.YZ: return x;
                default: return 0;
            }
        }

        public int PlaneNormal(GravitationalPlane gravitationalPlane) => gravitationalPlane != null ? PlaneNormal(gravitationalPlane.plane) : 0;

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }

        public static BlockSize operator /(Vector3 v1, BlockSize v2) => new BlockSize(
            (int) (v1.x / v2.x),
            (int) (v1.y / v2.y),
            (int) (v1.z / v2.z)
        );

        public static BlockSize operator /(BlockSize v1, Vector3 v2) => new BlockSize(
            (int) (v1.x / v2.x),
            (int) (v1.y / v2.y),
            (int) (v1.z / v2.z)
        );

        public static BlockSize operator /(BlockSize v1, BlockSize v2) => new BlockSize(
            v1.x / v2.x,
            v1.y / v2.y,
            v1.z / v2.z
        );
        
        public static BlockSize operator /(BlockSize bS, int i) => new BlockSize(
            bS.x / i,
            bS.y / i,
            bS.z / i
        );

        public static Vector3 operator %(Vector3 v1, BlockSize v2) => new Vector3(
            (int) v1.x % v2.x,
            (int) v1.y % v2.y,
            (int) v1.z % v2.z
        );
    }
}