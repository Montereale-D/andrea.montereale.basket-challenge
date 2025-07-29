using Enums;
using Gameplay.Throw;
using UnityEngine;

namespace Gameplay.Basketball
{
    /// <summary>
    /// Provides methods to compute the velocity needed to throw the ball.
    /// </summary>
    public static class BallTrajectory
    {
        private static readonly float HorizonalRange = 0.3f;
        private static readonly float PerfectVerticalRange = 0.05f;
        private static readonly float BackboardVerticalRange = 0.3f;

        public static Vector3 ComputeVelocity(TargetType targetType, ThrowSpot throwSpot)
        {
            Vector3 targetPos = targetType switch
            {
                TargetType.Perfect => throwSpot.PerfectTarget,
                TargetType.Backboard => throwSpot.BackboardTarget,
                TargetType.UnderBackboard=> throwSpot.PerfectTarget + ComputeSmallOffset(throwSpot),
                _ => throwSpot.PerfectTarget + ComputeBigOffset(targetType),
                
            };
            
            var velocity = ComputeShot(throwSpot.StartPos, targetPos,
                throwSpot.ThrowSpotData.throwAngle);

            if (targetType == TargetType.OverBackboard) velocity *= 1.2f;
            else if (targetType == TargetType.UnderPerfect) velocity *= 0.9f;
            
            return velocity;
        }

        private static Vector3 ComputeBigOffset(TargetType targetType)
        {
            float verticalRange = targetType == TargetType.UnderPerfect ? PerfectVerticalRange : BackboardVerticalRange;
                
            Vector3 randomOffset = new Vector3(
                Random.Range(-HorizonalRange, HorizonalRange),
                Random.Range(-verticalRange, verticalRange),
                Random.Range(-HorizonalRange, HorizonalRange)
            );
            
            return randomOffset;
        }
        
        private static Vector3 ComputeSmallOffset(ThrowSpot throwSpot)
        {
            var offset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0f, 0.5f),
                Random.Range(-0.5f, 0.5f)
            );
            
            return offset;
        }
        
        private static Vector3 ComputeShot(Vector3 startPosition, Vector3 targetPosition, float shootingAngle)
        {
            TryCalculateLaunchVelocity(startPosition, targetPosition, shootingAngle, out Vector3 velocity);
            return velocity;
        }
        
        private static bool TryCalculateLaunchVelocity(Vector3 origin, Vector3 target, float throwAngleDeg, out Vector3 velocity, float gravity = 9.81f)
        {
            // Projectile motion equation as reference
            
            Vector3 toTarget = target - origin;
            float verticalDistance = toTarget.y;
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            Vector3 horizontalDirection = new Vector3(toTarget.x, 0, toTarget.z).normalized;
            
            float throwAngleRad = throwAngleDeg * Mathf.Deg2Rad;
            float cosTheta = Mathf.Cos(throwAngleRad);
            float sinTheta = Mathf.Sin(throwAngleRad);
            float tanTheta = Mathf.Tan(throwAngleRad);
            
            float denominator = 2 * cosTheta * cosTheta * (horizontalDistance * tanTheta - verticalDistance);

            // prevent invalid result when denominator approaches zero via EPSILON check
            if (denominator <= 0.0001f)
            {
                velocity = Vector3.zero;
                return false;
            }
            
            float launchSpeed = Mathf.Sqrt((gravity * horizontalDistance * horizontalDistance) / denominator);
            velocity = horizontalDirection * (launchSpeed * cosTheta) + Vector3.up * (launchSpeed * sinTheta); 
            
            return true;
        }
    }
}