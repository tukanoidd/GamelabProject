using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class BlockHelpers
    {
        public static Vector3 ScaleToDefaultSize(BlockSize size, BlockSize defaultSize)
        {
            return new Vector3(
                size.xSize / defaultSize.xSize,
                size.ySize / defaultSize.ySize,
                size.zSize / defaultSize.zSize
            );
        }

        private static bool CanBeInt(float x) => (int) x == x;
        private static bool CanBeInt(Vector3 vec) => CanBeInt(vec.x) && CanBeInt(vec.y) && CanBeInt(vec.z);
        
        private static bool IsDivisibleByNum(int x, int num) => x % num == 0;
        private static bool IsDivisibleByNum(Vector3 vec, Vector3 nums) =>
            IsDivisibleByNum((int) vec.x, (int) nums.x) && IsDivisibleByNum((int) vec.y, (int) nums.y) &&
            IsDivisibleByNum((int) vec.z, (int) nums.z);

        public static float Min(BlockSize size) => Mathf.Min(size.xSize, size.ySize, size.zSize);

        public static bool CheckIfPosInBlockGrid(Vector3 pos, BlockSize size)
        {
            if (!CanBeInt(pos)) return false;

            return IsDivisibleByNum(pos, size.ToVector());
        }
    }
}