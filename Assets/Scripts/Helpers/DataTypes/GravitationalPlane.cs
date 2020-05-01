using System;

namespace DataTypes
{
    /// <summary>
    /// Indicates which plane character should move on based on gravity direction
    /// </summary>
    [Serializable]
    public class GravitationalPlane
    {
        public Plane plane;
        public PlaneSide planeSide;

        /// <summary>
        /// Constructor for GravitationalPlane class
        /// </summary>
        /// <param name="plane">Which plane character moves on</param>
        /// <param name="planeSide">Which side of a plane character moves on</param>
        public GravitationalPlane(Plane plane, PlaneSide planeSide)
        {
            this.plane = plane;
            this.planeSide = planeSide;
        }

        public static PlaneSide GetPlaneSide(AxisDirection normal)
        {
            if (normal == AxisDirection.Positive) return PlaneSide.PlaneNormalPositive;
            if (normal == AxisDirection.Negative) return PlaneSide.PlaneNormalNegative;
            return PlaneSide.PlaneNormalZero;
        }

        public static PlaneSide GetPlaneSide(float normal)
        {
            if (normal > 0) return PlaneSide.PlaneNormalPositive;
            if (normal < 0) return PlaneSide.PlaneNormalNegative;
            return PlaneSide.PlaneNormalZero;
        }

        public static int PlaneSideToInt(PlaneSide planeSide)
        {
            switch (planeSide)
            {
                case PlaneSide.PlaneNormalNegative:
                    return -1;
                case PlaneSide.PlaneNormalPositive:
                    return 1;
                default:
                    return 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is GravitationalPlane) return Equals((GravitationalPlane) obj);
            else return base.Equals(obj);
        }

        public bool Equals(GravitationalPlane gravitationalPlane)
        {
            if (gravitationalPlane == null) return false;
            return plane == gravitationalPlane.plane && planeSide == gravitationalPlane.planeSide;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + plane.GetHashCode();
            hash = hash * 23 + planeSide.GetHashCode();

            return hash;
        }
    }
}