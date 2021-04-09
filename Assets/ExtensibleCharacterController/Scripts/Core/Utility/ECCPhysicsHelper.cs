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
        /// <param name="offset">Offset to apply to collider position.</param>
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
            RaycastHit? closestHit = null
        )
        {
            // horizontalHit = GetClosestRaycastHitRecursive(hitCount, m_RaycastHits);
            // hitPoint = horizontalHit.point;
            // hitNormal = horizontalHit.normal;
            // colliderPoint = GetClosestColliderPoint(m_Collider, m_DeltaVelocity, hitPoint);

            // distanceFromCollider = (hitPoint - colliderPoint).magnitude - COLLIDER_OFFSET;

            // Get next index, current hit, and the next hit.
            int nextIndex = index + 1;
            RaycastHit currentHit = hits[index];
            RaycastHit nextHit = nextIndex < hitCount ? hits[nextIndex] : currentHit;

            Vector3 hitPoint = currentHit.point;
            Vector3 hitNormal = currentHit.normal;
            Vector3 colliderPoint = GetClosestColliderPoint(collider, offset != null ? (Vector3)offset : Vector3.zero, hitPoint);
            float distanceFromCollider = (hitPoint - colliderPoint).magnitude;
            if (debug)
            {
                Debug.Log(distanceFromCollider);
            }

            // Set closest hit to current hit if it hasn't been assigned anything.
            closestHit = closestHit != null ? (RaycastHit)closestHit : default(RaycastHit);

            // If current hit is inside collider, use next ray instead.
            if (currentHit.distance == 0 && nextIndex < hitCount)
            {
                return GetClosestRaycastHitRecursive(collider, hitCount, hits, offset, distanceOffset, debug, nextIndex, closestHit);
            }

            // If the next hit is closer, then it is the new closest hit.
            if (nextHit.distance + distanceOffset < ((RaycastHit)closestHit).distance + distanceOffset)
            {
                closestHit = nextHit;
            }

            return nextIndex < hitCount ?
                GetClosestRaycastHitRecursive(collider, hitCount, hits, offset, distanceOffset, debug, nextIndex, closestHit) :
                (RaycastHit)closestHit;
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
