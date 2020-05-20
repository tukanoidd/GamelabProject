using System;
using UnityEngine;

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
        
        public Vector3 ToGravityVector()
        {
            Vector3 gravity = Vector3.zero;

            if (plane == Plane.XY) gravity.z = 1;
            if (plane == Plane.XZ) gravity.y = 1;
            if (plane == Plane.YZ) gravity.x = 1;

            gravity *= -PlaneSideToInt(planeSide);

            return gravity;
        }
        
        public Vector3 ToRotationEuler(Vector3 rotationFrom)
        {
            Vector3 eulerRotation = Vector3.zero;
            eulerRotation.y = rotationFrom.y;

            if (plane == Plane.XZ)
            {
                if (planeSide == PlaneSide.PlaneNormalNegative) eulerRotation.x = 180;
            } else if (plane == Plane.XY)
            {
                if (planeSide == PlaneSide.PlaneNormalPositive) eulerRotation.x = 90;
                else if (planeSide == PlaneSide.PlaneNormalNegative) eulerRotation.x = 270;
            } else if (plane == Plane.YZ)
            {
                if (planeSide == PlaneSide.PlaneNormalPositive) eulerRotation.z = 270;
                else if (planeSide == PlaneSide.PlaneNormalNegative) eulerRotation.z = 90;
            }

            return eulerRotation;
        }

        public GravitationalPlane Opposite => new GravitationalPlane(
            plane,
            OppositePlaneSide(planeSide)
        );

        public static PlaneSide OppositePlaneSide(PlaneSide planeSide)
        {
            switch (planeSide)
            {
                case PlaneSide.PlaneNormalNegative: return PlaneSide.PlaneNormalPositive;
                case PlaneSide.PlaneNormalPositive: return PlaneSide.PlaneNormalNegative;
                default: return PlaneSide.PlaneNormalZero;
            }
        }

        public static PlaneSide OppositePlaneSide(GravitationalPlane gravitationalPlane) =>
            OppositePlaneSide(gravitationalPlane.planeSide);

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