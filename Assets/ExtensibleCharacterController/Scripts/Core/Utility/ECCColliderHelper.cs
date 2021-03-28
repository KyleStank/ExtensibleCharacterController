using UnityEngine;

namespace ExtensibleCharacterController.Core.Utility
{
    /// <summary>
    /// Static class containing various different helper methods related to colliders.
    /// </summary>
    public static class ECCColliderHelper
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
    }
}
