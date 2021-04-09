using UnityEngine;

namespace ExtensibleCharacterController.Core.Utility
{
    /// <summary>
    /// Static class containing various different helper methods related to Physics.
    /// </summary>
    public static class ECCPhysicsHelper
    {
        /// <summary>
        /// Calculates the start and end cap values required for a CapsuleCast.
        /// Outputs both cap values and the center position of the capsule.
        /// </summary>
        /// <param name="collider">CapsuleCollider to calculate from.</param>
        /// <param name="position">Position of CapsuleCollider.</param>
        /// <param name="rotation">Rotation of CapsuleCollider.</param>
        /// <param name="center">Outputted center position of CapsuleCollider.</param>
        /// <param name="start">Outputted start end cap of CapsuleCollider.</param>
        /// <param name="end">Outputted end end cap of CapsuleCollider.</param>
        /// <param name="height">Optional height that will be used instead of the CapsuleCollider's height.</param>
        /// <param name="radius">Optional radius that will be used instead of the CapsuleCollider's radius.</param>
        public static void CalculateCapsuleCaps(
            CapsuleCollider collider,
            Vector3 position,
            Quaternion rotation,
            out Vector3 center,
            out Vector3 start,
            out Vector3 end,
            float? height = null,
            float? radius = null
        )
        {
            Vector3 direction = GetCapsuleDirection(collider);
            float heightScale = GetCapsuleHeightScale(collider);
            float radiusScale = GetCapsuleRadiusScale(collider);

            float capAdjustment =
                ((height != null ? (float)height : collider.height) / 2.0f * heightScale) -
                ((radius != null ? (float)radius : collider.radius) * radiusScale);
            center = ECCMathHelper.TransformPoint(position, rotation, Vector3.Scale(collider.center, collider.transform.lossyScale));
            start = center - (direction * capAdjustment);
            end = center + (direction * capAdjustment);
        }

        /// <summary>
        /// Calculates the start and end cap values required for a CapsuleCast.
        /// Outputs both cap values of the capsule.
        /// </summary>
        /// <param name="collider">CapsuleCollider to calculate from.</param>
        /// <param name="position">Position of CapsuleCollider.</param>
        /// <param name="rotation">Rotation of CapsuleCollider.</param>
        /// <param name="start">Outputted start end cap of CapsuleCollider.</param>
        /// <param name="end">Outputted end end cap of CapsuleCollider.</param>
        /// <param name="height">Optional height that will be used instead of the CapsuleCollider's height.</param>
        /// /// <param name="radius">Optional radius that will be used instead of the CapsuleCollider's radius.</param>
        public static void CalculateCapsuleCaps(
            CapsuleCollider collider,
            Vector3 position,
            Quaternion rotation,
            out Vector3 start,
            out Vector3 end,
            float? height = null,
            float? radius = null
        )
        {
            Vector3 center;
            CalculateCapsuleCaps(collider, position, rotation, out center, out start, out end, height, radius);
        }

        /// <summary>
        /// Returns a Vector3 direction of a CapsuleCollider based on the direction type selected in the inspector.
        /// </summary>
        /// <param name="collider">CapsuleCollider to use.</param>
        /// <returns>Vector3 direction based on inspector direction.</returns>
        public static Vector3 GetCapsuleDirection(CapsuleCollider collider)
        {
            // X axis = 0, Y axis = 1, Z axis = 2
            int direction = collider.direction;
            if (direction == 0)
                return collider.transform.right;
            else if (direction == 1)
                return collider.transform.up;

            return collider.transform.forward;
        }

        /// <summary>
        /// Returns a float height multiplier of a CapsuleCollider based on the direction type selected in the inspector.
        /// </summary>
        /// <param name="collider">CapsuleCollider to use.</param>
        /// <returns>Float multiplier based on inspector direction.</returns>
        public static float GetCapsuleHeightScale(CapsuleCollider collider)
        {
            // X axis = 0, Y axis = 1, Z axis = 2
            int direction = collider.direction;
            if (direction == 0)
                return collider.transform.lossyScale.x;
            else if (direction == 1)
                return collider.transform.lossyScale.y;

            return collider.transform.lossyScale.z;
        }

        /// <summary>
        /// Returns a float radius multiplier of a CapsuleCollider based on the direction type selected in the inspector.
        /// </summary>
        /// <param name="collider">CapsuleCollider to use.</param>
        /// <returns>Float mutliplier based on inspector direction.</returns>
        public static float GetCapsuleRadiusScale(CapsuleCollider collider)
        {
            // X axis = 0, Y axis = 1, Z axis = 2
            int direction = collider.direction;
            if (direction == 0)
                return Mathf.Max(collider.transform.lossyScale.y, collider.transform.lossyScale.z);
            else if (direction == 1)
                return Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.z);

            return Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y);
        }

        /// <summary>
        /// Performs a CapsuleCast from a specified collider at an offset position in a direction.
        /// Outputs a RaycastHit.
        /// Returns a bool that is true if something was hit.
        /// </summary>
        /// <param name="collider">Collider to model CapsuleCast from.</param>
        /// <param name="offset">Offset position to apply to collider position for CapsuleCast.</param>
        /// <param name="direction">Direction of CapsuleCast.</param>
        /// <param name="layerMask">Layers to hit.</param>
        /// <param name="hit">Outputted RaycastHit struct.</param>
        /// <param name="invertLayerMask">If true, invert layer mask. Otherwise, use layer mask as normal.</param>
        /// <returns>Returns true if the CapsuleCast hit something. Otherwise, returns false. When false, outputted hit is null.</returns>
        public static bool CapsuleCast(
            CapsuleCollider collider,
            Vector3 offset,
            Vector3 direction,
            LayerMask layerMask,
            out RaycastHit hit,
            bool invertLayerMask = false
        )
        {
            Vector3 capStart, capEnd;
            CalculateCapsuleCaps(collider, collider.transform.position + offset, collider.transform.rotation, out capStart, out capEnd);
            return Physics.CapsuleCast(
                capStart,
                capEnd,
                collider.radius,
                direction.normalized,
                out hit,
                direction.magnitude,
                invertLayerMask ? ~layerMask.value : layerMask.value
            );
        }

        /// <summary>
        /// Performs a CapsuleCastAll from a specified collider at an offset position in a direction.
        /// Returns an array of RaycastHit for all objects hit.
        /// </summary>
        /// <param name="collider">Collider to model CapsuleCastAll from.</param>
        /// <param name="offset">Offset position to apply to collider position for CapsuleCastAll.</param>
        /// <param name="direction">Direction of CapsuleCastAll.</param>
        /// <param name="layerMask">Layers to hit.</param>
        /// <param name="invertLayerMask">If true, invert layer mask. Otherwise, use layer mask as normal.</param>
        /// <returns>Array of everything CapsuleCastAll hit.</returns>
        public static RaycastHit[] CapsuleCastAll(
            CapsuleCollider collider,
            Vector3 offset,
            Vector3 direction,
            LayerMask layerMask,
            bool invertLayerMask = false
        )
        {
            CalculateCapsuleCaps(
                collider,
                collider.transform.position + offset,
                collider.transform.rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );
            return Physics.CapsuleCastAll(
                capStart,
                capEnd,
                collider.radius,
                direction.normalized,
                direction.magnitude,
                invertLayerMask ? ~layerMask.value : layerMask.value
            );
        }

        /// <summary>
        /// Loops through an array of RaycastHits and returned the closest one, relative to a collider's position.
        /// </summary>
        /// <param name="collider">Collider to check against.</param>
        /// <param name="hitCount">Amount of Raycasts in array.</param>
        /// <param name="hits">Array of Raycasts.</param>
        /// <param name="offset">Optional offset to apply to collider position.</param>
        /// <param name="distanceOffset">Optional offset to apply to distance checking. Useful for adding small padding.</param>
        /// <param name="index">Parameter used by recursion for tracking the index of the RaycastHit to check.</param>
        /// <param name="closestHit">Parameter used by recursion for tracking the closest RaycastHit.</param>
        /// <returns></returns>
        public static RaycastHit GetClosestRaycastHitRecursive(
            Collider collider,
            int hitCount,
            RaycastHit[] hits,
            Vector3? offset = null,
            float distanceOffset = 0.0f,
            bool debug = false,
            int index = 0,
            RaycastHit closestHit = default(RaycastHit)
        )
        {
            // Get next index, current hit, and the next hit.
            int nextIndex = index + 1;
            RaycastHit currentHit = hits[index];
            RaycastHit nextHit = nextIndex < hitCount ? hits[nextIndex] : currentHit;

            // Set closest hit to current hit if it hasn't been assigned anything.
            closestHit = closestHit.Equals(default(RaycastHit)) ? currentHit : closestHit;

            // Calculate distances for next hit and closest hit.
            CalculateClosestRaycastHitDistances(
                collider,
                offset != null ? (Vector3)offset : Vector3.zero,
                nextHit,
                closestHit,
                out float nextDistance,
                out float closestDistance
            );

            // If the next hit is closer, then it is the new closest hit.
            if (nextDistance - distanceOffset < closestDistance)
            {
                closestHit = nextHit;
            }

            return nextIndex < hitCount ?
                GetClosestRaycastHitRecursive(collider, hitCount, hits, offset, distanceOffset, debug, nextIndex, closestHit) :
                closestHit;
        }

        /// <summary>
        /// Calculates the distance of the hit point from the closest collider point for the next RaycastHit and the closest RaycastHit.
        /// Used the GetClosestRaycastHitRecursive() method only.
        /// </summary>
        /// <param name="collider">Collider to check against.</param>
        /// <param name="offset">Offset to apply to collider position.</param>
        /// <param name="nextHit">Next RaycastHit to check.</param>
        /// <param name="closestHit">Closest RaycastHit to check.</param>
        /// <param name="nextDistance">Outputted distance from next Raycast's point to the next collider point.</param>
        /// <param name="closestDistance">Outputted distance from closest Raycast's point to the closest collider point.</param>
        private static void CalculateClosestRaycastHitDistances(
            Collider collider,
            Vector3 offset,
            RaycastHit nextHit,
            RaycastHit closestHit,
            out float nextDistance,
            out float closestDistance
        )
        {
            // Calculate distances for next hit and closest hit.
            Vector3 colliderOffset = offset != null ? (Vector3)offset : Vector3.zero;
            Vector3 nextHitPoint = nextHit.point;
            Vector3 nextColliderPoint = GetClosestColliderPoint(collider, colliderOffset, nextHitPoint);
            nextDistance = (nextHitPoint - nextColliderPoint).sqrMagnitude;

            Vector3 closestHitPoint = closestHit.point;
            Vector3 closestColliderPoint = GetClosestColliderPoint(collider, colliderOffset, closestHitPoint);
            closestDistance = (closestHitPoint - closestColliderPoint).sqrMagnitude;
        }

        /// <summary>
        /// Returns the closest world on a Collider relative to another point.
        /// </summary>
        /// <param name="collider">Collider to check.</param>
        /// <param name="offset">Offset to apply to collider's position before checking.</param>
        /// <param name="point">Relative point used to find closest point.</param>
        /// <returns>Vector3 world point on collider.</returns>
        public static Vector3 GetClosestColliderPoint(Collider collider, Vector3 offset, Vector3 point)
        {
            collider.transform.position += offset;
            Vector3 closestPoint = collider.ClosestPoint(point);
            collider.transform.position -= offset;

            return closestPoint;
        }
    }
}
