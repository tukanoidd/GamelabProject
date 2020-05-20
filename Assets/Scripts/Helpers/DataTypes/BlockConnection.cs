using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    /// <summary>
    /// Stores information about connected block
    /// </summary>
    [Serializable]
    public class BlockConnection
    {
        public List<Block> connectedBlocks;
        public GravitationalPlane gravitationalPlane;
        public List<ConnectionPoint> connectionPoints;
        public List<Vector3> customCameraPositions;
        public bool isNear;

        /// <summary>
        /// Constructor for BlockConnection class
        /// </summary>
        /// <param name="connectedBlocks">Blocks that are connected</param>
        /// <param name="gravitationalPlane">Plane of their connection</param>
        /// <param name="connectionPoints">Connection points blocks are connected with</param>
        /// <param name="customCameraPositions">Camera positions that allow this connection to be viable</param>>
        /// <param name="isNear">If the connection is between near blocks</param>
        public BlockConnection(List<Block> connectedBlocks, GravitationalPlane gravitationalPlane,
            List<ConnectionPoint> connectionPoints, List<Vector3> customCameraPositions,
            bool isNear)
        {
            this.connectedBlocks = connectedBlocks;
            this.gravitationalPlane = gravitationalPlane;
            this.connectionPoints = connectionPoints;
            this.customCameraPositions = customCameraPositions;
            this.isNear = isNear;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is BlockConnection) return Equals((BlockConnection) obj);
            else return base.Equals(obj);
        }

        public bool Equals(BlockConnection blockConnection)
        {
            if (blockConnection == null) return false;

            return connectedBlocks == blockConnection.connectedBlocks
                   && gravitationalPlane.Equals(blockConnection.gravitationalPlane)
                   && connectionPoints == blockConnection.connectionPoints
                   && customCameraPositions == blockConnection.customCameraPositions
                   && isNear == blockConnection.isNear;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + connectedBlocks.GetHashCode();
            hash = hash * 23 + gravitationalPlane.GetHashCode();
            hash = hash * 23 + connectionPoints.GetHashCode();
            hash = hash * 23 + customCameraPositions.GetHashCode();
            hash = hash * 23 + isNear.GetHashCode();

            return hash;
        }
    }
}