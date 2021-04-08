using RotaryHeart.Lib.UnityGLDebug;
using UnityEngine;
using UEPhysics = UnityEngine.Physics;
using UEPhysics2D = UnityEngine.Physics2D;

namespace RotaryHeart.Lib.PhysicsExtension
{
    public enum PreviewCondition
    {
        None, Editor, Game, Both
    }

    /// <summary>
    /// This is an extension for UnityEngine.Physics, it have all the cast, overlap, and checks with an option to preview them.
    /// </summary>
    public static class Physics
    {
        #region Unity Engine Physics
        //Global variables for use on default values, this is left here so that it can be changed easily
        static Quaternion M_orientation = default(Quaternion);
        static float M_maxDistance = Mathf.Infinity;
        static int M_layerMask = -1;
        static QueryTriggerInteraction M_queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
        static Color M_castColor = new Color(1, 0.5f, 0, 1);

        #region BoxCast

        #region Boxcast single
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return BoxCast(center, halfExtents, direction, out rayInfo, M_orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit rayInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(center, halfExtents, direction, out rayInfo, M_orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit rayInfo, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit rayInfo, Quaternion orientation, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit rayInfo, Quaternion orientation, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(center, halfExtents, direction, out rayInfo, orientation, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            bool collided = UEPhysics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                if (collided)
                {
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    maxDistance = hitInfo.distance;
                }

                DebugExtensions.DebugBox(center, halfExtents, direction, maxDistance, M_castColor, orientation, collided ? (hitColor ?? Color.green) : M_castColor, true, drawDuration, preview, drawDepth);
            }

            return collided;
        }
        #endregion

        #region Boxcast all
        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(center, halfExtents, direction, M_orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(center, halfExtents, direction, orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(center, halfExtents, direction, orientation, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, LayerMask layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, LayerMask layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit[] hitInfo = UEPhysics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                float previewDistance = 0;

                foreach (RaycastHit hit in hitInfo)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugBox(center + direction * hit.distance, halfExtents, (hitColor ?? Color.green), orientation, drawDuration, preview, drawDepth);

                    if (hit.distance > previewDistance)
                        previewDistance = hit.distance;
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                DebugExtensions.DebugBox(center, halfExtents, direction, maxDistance, M_castColor, orientation, M_castColor, true, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }
        #endregion

        #region Boxcast non alloc
        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(center, halfExtents, direction, results, M_orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, int layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int size = UEPhysics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                float previewDistance = 0;

                for (int i = 0; i < size; i++)
                {
                    RaycastHit hit = results[i];
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugBox(center + direction * hit.distance, halfExtents, (hitColor ?? Color.green), orientation, drawDuration, preview, drawDepth);

                    if (hit.distance > previewDistance)
                        previewDistance = hit.distance;
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                DebugExtensions.DebugBox(center, halfExtents, direction, maxDistance, M_castColor, orientation, M_castColor, true, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion

        #endregion

        #region Capsule Cast

        #region Capsulecast single
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return CapsuleCast(point1, point2, radius, direction, out rayInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return CapsuleCast(point1, point2, radius, direction, out rayInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return CapsuleCast(point1, point2, radius, direction, out rayInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return CapsuleCast(point1, point2, radius, direction, out rayInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            bool collided = UEPhysics.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                if (collided)
                {
                    maxDistance = hitInfo.distance;
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                }

                Vector3 midPoint = (point2 + point1) / 2;

                DebugExtensions.DebugCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(midPoint, midPoint + direction * maxDistance, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(midPoint, midPoint + direction * maxDistance, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugCapsule(point1 + direction * maxDistance, point2 + direction * maxDistance, collided ? (hitColor ?? Color.green) : M_castColor, radius, true, drawDuration, preview, drawDepth);

            }

            return collided;
        }
        #endregion

        #region Capsulecast all
        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(point1, point2, radius, direction, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(point1, point2, radius, direction, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(point1, point2, radius, direction, maxDistance, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit[] hitInfo = UEPhysics.CapsuleCastAll(point1, point2, radius, direction, maxDistance, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = false;
                float maxDistanceRay = 0;

                foreach (RaycastHit hit in hitInfo)
                {
                    collided = true;

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugCapsule(point1 + direction * hit.distance, point2 + direction * hit.distance, (hitColor ?? Color.green), radius, true, drawDuration, preview, drawDepth);
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                Vector3 midPoint = (point2 + point1) / 2;

                DebugExtensions.DebugCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = midPoint + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    GLDebug.DrawLine(endCollisionPoint, midPoint + direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugCapsule(point1 + direction * maxDistance, point2 + direction * maxDistance, M_castColor, radius, true, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }
        #endregion

        #region Capsulecast non alloc
        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(point1, point2, radius, direction, results, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int size = UEPhysics.CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = false;
                float maxDistanceRay = 0;

                for (int i = 0; i < size; i++)
                {
                    collided = true;

                    RaycastHit hit = results[i];

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugCapsule(point1 + direction * hit.distance, point2 + direction * hit.distance, (hitColor ?? Color.green), radius, true, drawDuration, preview, drawDepth);
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                Vector3 midPoint = (point2 + point1) / 2;

                DebugExtensions.DebugCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = midPoint + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugCapsule(point1 + direction * maxDistance, point2 + direction * maxDistance, M_castColor, radius, true, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion

        #endregion

        #region Check Box
        public static bool CheckBox(Vector3 center, Vector3 halfExtents, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckBox(center, halfExtents, M_orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckBox(center, halfExtents, orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckBox(center, halfExtents, orientation, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            bool collided = UEPhysics.CheckBox(center, halfExtents, orientation, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugBox(center, halfExtents, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), orientation, drawDuration, preview, drawDepth);
            }

            return collided;
        }
        #endregion

        #region Check Capsule
        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckCapsule(start, end, radius, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckCapsule(start, end, radius, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            bool collided = UEPhysics.CheckCapsule(start, end, radius, layermask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugCapsule(start, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, false, drawDuration, preview, drawDepth);
            }

            return collided;
        }
        #endregion

        #region Check Sphere
        public static bool CheckSphere(Vector3 position, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckSphere(position, radius, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckSphere(Vector3 position, float radius, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CheckSphere(position, radius, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool CheckSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            bool collided = UEPhysics.CheckSphere(position, radius, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugWireSphere(position, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return collided;
        }
        #endregion

        #region Linecast
        public static bool Linecast(Vector3 start, Vector3 end, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Linecast(start, end, out rayInfo, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Linecast(Vector3 start, Vector3 end, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Linecast(start, end, out rayInfo, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Linecast(Vector3 start, Vector3 end, int layerMask, QueryTriggerInteraction querryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Linecast(start, end, out rayInfo, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Linecast(start, end, out hitInfo, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Linecast(start, end, out hitInfo, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask, QueryTriggerInteraction querryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Vector3 heading = end - start;
            return Raycast(start, heading, out hitInfo, heading.magnitude, layerMask, querryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Overlap Box
        #region OverlapBox alloc
        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(center, halfExtents, M_orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(center, halfExtents, orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(center, halfExtents, orientation, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider[] colliders = UEPhysics.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = colliders.Length > 0;

                DebugExtensions.DebugBox(center, halfExtents, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), orientation, drawDuration, preview, drawDepth);
            }

            return colliders;
        }
        #endregion

        #region OverlapBox non alloc
        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(center, halfExtents, results, M_orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(center, halfExtents, results, orientation, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int size = UEPhysics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = size > 0;

                DebugExtensions.DebugBox(center, halfExtents, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), orientation, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion
        #endregion

        #region Overlap Capsule
        #region OverlapCapsule alloc
        public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsule(point0, point1, radius, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsule(point0, point1, radius, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider[] colliders = UEPhysics.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = colliders.Length > 0;

                DebugExtensions.DebugCapsule(point0, point1, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, false, drawDuration, preview, drawDepth);
            }

            return colliders;
        }
        #endregion

        #region OverlapCapsule non alloc
        public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleNonAlloc(point0, point1, radius, results, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int size = UEPhysics.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = size > 0;

                DebugExtensions.DebugCapsule(point0, point1, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, false, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion
        #endregion

        #region Overlap Sphere
        #region OverlapSphere alloc
        public static Collider[] OverlapSphere(Vector3 position, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapSphere(position, radius, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapSphere(position, radius, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider[] colliders = UEPhysics.OverlapSphere(position, radius, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = colliders.Length > 0;

                DebugExtensions.DebugWireSphere(position, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return colliders;
        }
        #endregion

        #region OverlapSphere non alloc
        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapSphereNonAlloc(position, radius, results, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapSphereNonAlloc(position, radius, results, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int size = UEPhysics.OverlapSphereNonAlloc(position, radius, results, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = size > 0;

                DebugExtensions.DebugWireSphere(position, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion
        #endregion

        #region Raycast

        #region Raycast single
        #region Vector3
        public static bool Raycast(Vector3 origin, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(origin, direction, out rayInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(origin, direction, out rayInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(origin, direction, out rayInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(origin, direction, out rayInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            return Raycast(new Ray(origin, direction), out hitInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static bool Raycast(Ray ray, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(ray, out rayInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(ray, out rayInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(ray, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(ray, out rayInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(ray, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit rayInfo;
            return Raycast(ray, out rayInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(ray, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            bool collided = UEPhysics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                Vector3 end = ray.origin + ray.direction * (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                if (collided)
                {
                    collided = true;
                    end = hitInfo.point;

                    DebugExtensions.DebugPoint(end, Color.red, 0.5f, drawDuration, preview, drawDepth);
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(ray.origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(ray.origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
            }

            return collided;
        }

        #endregion
        #endregion

        #region Raycast all
        #region Vector3
        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, maxDistance, (int)layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(new Ray(origin, direction), maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static RaycastHit[] RaycastAll(Ray ray, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(ray, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(ray, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, LayerMask layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(ray, maxDistance, (int)layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            RaycastHit[] raycastInfo = UEPhysics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                Vector3 end = ray.origin + ray.direction * (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                Vector3 previewOrigin = ray.origin;
                Vector3 sectionOrigin = ray.origin;

                foreach (RaycastHit hit in raycastInfo)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    if (preview == PreviewCondition.Editor)
                        Debug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);
                    else
                        GLDebug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);

                    if (Vector3.Distance(ray.origin, hit.point) > Vector3.Distance(ray.origin, previewOrigin))
                        previewOrigin = hit.point;

                    sectionOrigin = hit.point;
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(previewOrigin, end, (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(previewOrigin, end, (noHitColor ?? Color.red), drawDuration);
            }

            return raycastInfo;
        }
        #endregion
        #endregion

        #region Raycast non alloc
        #region Vector3
        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, LayerMask layermask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, maxDistance, layermask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, LayerMask layermask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(new Ray(origin, direction), results, maxDistance, layermask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(ray, results, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(ray, results, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, LayerMask layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(ray, results, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            int size = UEPhysics.RaycastNonAlloc(ray, results, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                Vector3 end = ray.origin + ray.direction * (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);
                Vector3 previewOrigin = ray.origin;
                Vector3 sectionOrigin = ray.origin;

                for (int i = 0; i < size; i++)
                {
                    RaycastHit hit = results[i];
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    if (preview == PreviewCondition.Editor)
                        Debug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);
                    else
                        GLDebug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);

                    if (Vector3.Distance(ray.origin, hit.point) > Vector3.Distance(ray.origin, previewOrigin))
                        previewOrigin = hit.point;

                    sectionOrigin = hit.point;
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(previewOrigin, end, (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(previewOrigin, end, (noHitColor ?? Color.red), drawDuration);
            }

            return size;
        }
        #endregion
        #endregion

        #endregion

        #region Sphere Cast
        #region Spherecast single
        #region Vector3
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(origin, radius, direction, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(origin, radius, direction, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(new Ray(origin, direction), radius, out hitInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static bool SphereCast(Ray ray, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(ray, radius, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(ray, radius, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(ray, radius, out hitInfo, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(ray, radius, out hitInfo, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            RaycastHit hitInfo;
            return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            bool collided = UEPhysics.SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                if (collided)
                {
                    maxDistance = hitInfo.distance;
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                }

                DebugExtensions.DebugWireSphere(ray.origin, M_castColor, radius, drawDuration, preview, drawDepth);

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugWireSphere(ray.origin + ray.direction * maxDistance, collided ? (hitColor ?? Color.green) : M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return collided;
        }
        #endregion
        #endregion

        #region Spherecast all
        #region Vector3
        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(origin, radius, direction, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(origin, radius, direction, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(origin, radius, direction, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(new Ray(origin, direction), radius, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static RaycastHit[] SphereCastAll(Ray ray, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(ray, radius, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(ray, radius, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastAll(ray, radius, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            RaycastHit[] hitInfo = UEPhysics.SphereCastAll(ray, radius, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = false;
                float maxDistanceRay = 0;

                foreach (RaycastHit hit in hitInfo)
                {
                    collided = true;

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugWireSphere(ray.origin + ray.direction * hit.distance, (hitColor ?? Color.green), radius, drawDuration, preview, drawDepth);
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                DebugExtensions.DebugWireSphere(ray.origin, M_castColor, radius, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = ray.origin + ray.direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(ray.origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, ray.origin + ray.direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(ray.origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    GLDebug.DrawLine(endCollisionPoint, ray.origin + ray.direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugWireSphere(ray.origin + ray.direction * maxDistance, M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }
        #endregion
        #endregion

        #region Spherecast non alloc
        #region Vector3
        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(origin, radius, direction, results, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(new Ray(origin, direction), radius, results, maxDistance, layerMask, queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }
        #endregion

        #region Ray
        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(ray, radius, results, M_maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(ray, radius, results, maxDistance, M_layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, M_queryTriggerInteraction, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            ray.direction.Normalize();
            int size = UEPhysics.SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, queryTriggerInteraction);

            if (preview != PreviewCondition.None)
            {
                bool collided = false;
                float maxDistanceRay = 0;

                foreach (RaycastHit hit in results)
                {
                    collided = true;

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugWireSphere(ray.origin + ray.direction * hit.distance, (hitColor ?? Color.green), radius, drawDuration, preview, drawDepth);
                }

                maxDistance = (maxDistance == M_maxDistance ? 1000 * 1000 : maxDistance);

                DebugExtensions.DebugWireSphere(ray.origin, M_castColor, radius, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = ray.origin + ray.direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(ray.origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, ray.origin + ray.direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(ray.origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    Debug.DrawLine(endCollisionPoint, ray.origin + ray.direction * maxDistance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugWireSphere(ray.origin + ray.direction * maxDistance, M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return size;
        }
        #endregion
        #endregion
        #endregion

        #endregion

        public static class Extended
        {
            static System.Collections.Generic.List<Collider> M_colliders = new System.Collections.Generic.List<Collider>();

            #region Sight cone cast

            #region Sight cone cast single

            #region Vector3
            public static bool SightConeCast(Vector3 origin, Vector3 direction, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCast(origin, direction, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static bool SightConeCast(Vector3 origin, Vector3 direction, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCast(origin, direction, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static bool SightConeCast(Vector3 origin, Vector3 direction, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                direction.Normalize();

                Collider[] colliders = UEPhysics.OverlapSphere(origin, length, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(origin, direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (var collider in colliders)
                {
                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - origin).normalized;

                    if (Vector3.Angle(direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                            else
                                GLDebug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                        }

                        return true;
                    }
                }

                return false;
            }

            #endregion

            #region Ray
            public static bool SightConeCast(Ray ray, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCast(ray, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static bool SightConeCast(Ray ray, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCast(ray, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static bool SightConeCast(Ray ray, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                ray.direction.Normalize();

                Collider[] colliders = UEPhysics.OverlapSphere(ray.origin, length, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(ray.origin, ray.direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (var collider in colliders)
                {
                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - ray.origin).normalized;

                    if (Vector3.Angle(ray.direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                            else
                                GLDebug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                        }
                        return true;
                    }
                }

                return false;
            }
            #endregion

            #endregion

            #region Sight cone cast all

            #region Vector3
            public static Collider[] SightConeCastAll(Vector3 origin, Vector3 direction, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastAll(origin, direction, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static Collider[] SightConeCastAll(Vector3 origin, Vector3 direction, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastAll(origin, direction, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static Collider[] SightConeCastAll(Vector3 origin, Vector3 direction, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                direction.Normalize();
                M_colliders.Clear();

                Collider[] colliders = UEPhysics.OverlapSphere(origin, length, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(origin, direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (var collider in colliders)
                {
                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - origin).normalized;

                    if (Vector3.Angle(direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                            else
                                GLDebug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                        }

                        M_colliders.Add(collider);
                    }
                }

                return M_colliders.ToArray();
            }
            #endregion

            #region Ray
            public static Collider[] SightConeCastAll(Ray ray, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastAll(ray, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static Collider[] SightConeCastAll(Ray ray, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastAll(ray, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static Collider[] SightConeCastAll(Ray ray, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                ray.direction.Normalize();
                M_colliders.Clear();

                Collider[] colliders = UEPhysics.OverlapSphere(ray.origin, length, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(ray.origin, ray.direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (var collider in colliders)
                {
                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - ray.origin).normalized;

                    if (Vector3.Angle(ray.direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                            else
                                GLDebug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration);
                        }

                        M_colliders.Add(collider);
                    }
                }

                return M_colliders.ToArray();
            }
            #endregion

            #endregion

            #region Sight cone cast non alloc

            #region Vector3
            public static int SightConeCastNonAlloc(Vector3 origin, Vector3 direction, Collider[] results, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastNonAlloc(origin, direction, results, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static int SightConeCastNonAlloc(Vector3 origin, Vector3 direction, Collider[] results, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastNonAlloc(origin, direction, results, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static int SightConeCastNonAlloc(Vector3 origin, Vector3 direction, Collider[] results, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                direction.Normalize();

                int size = UEPhysics.OverlapSphereNonAlloc(origin, length, results, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(origin, direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (Collider collider in results)
                {
                    if (collider == null)
                        break;

                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - origin).normalized;

                    if (Vector3.Angle(direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration, drawDepth);
                            else
                                GLDebug.DrawLine(origin, collider.transform.position, hitColor ?? Color.green, drawDuration, drawDepth);
                        }
                    }
                }

                return size;
            }
            #endregion

            #region Ray
            public static int SightConeCastNonAlloc(Ray ray, Collider[] results, float length, float angle, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastNonAlloc(ray, results, length, angle, M_layerMask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static int SightConeCastNonAlloc(Ray ray, Collider[] results, float length, float angle, LayerMask mask, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                return SightConeCastNonAlloc(ray, results, length, angle, mask, M_queryTriggerInteraction, ignoreTransforms, preview, drawDuration, hitColor, noHitColor, drawDepth);
            }

            public static int SightConeCastNonAlloc(Ray ray, Collider[] results, float length, float angle, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction, System.Collections.Generic.List<Transform> ignoreTransforms = null, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
            {
                ray.direction.Normalize();

                int size = UEPhysics.OverlapSphereNonAlloc(ray.origin, length, results, mask, queryTriggerInteraction);

                if (preview != PreviewCondition.None)
                {
                    DebugExtensions.DebugConeSight(ray.origin, ray.direction, length, noHitColor ?? Color.red, angle, drawDuration, preview, drawDepth);
                }

                float tempAngle = angle / 1.9f;

                foreach (Collider collider in results)
                {
                    if (collider == null)
                        break;

                    if (ignoreTransforms != null && ignoreTransforms.Contains(collider.transform))
                        continue;

                    Vector3 heading = (collider.transform.position - ray.origin).normalized;

                    if (Vector3.Angle(ray.direction, heading) <= tempAngle)
                    {
                        if (preview != PreviewCondition.None)
                        {
                            DebugExtensions.DebugPoint(collider.transform.position, hitColor ?? Color.green, 0.5f, drawDuration, preview, drawDepth);

                            if (preview == PreviewCondition.Editor)
                                Debug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration, drawDepth);
                            else
                                GLDebug.DrawLine(ray.origin, collider.transform.position, hitColor ?? Color.green, drawDuration, drawDepth);
                        }
                    }
                }

                return size;
            }
            #endregion

            #endregion

            #endregion
        }
    }

    /// <summary>
    /// This is an extesion for UnityEngine.Physics2D, it have all the cast and overlap with an option to preview them.
    /// </summary>
    public static class Physics2D
    {
        #region Unity Engine Physics
        //Global variables for use on default values, this is left here so that it can be changed easily
        static float M_maxDistance = Mathf.Infinity;
        static Color M_castColor = new Color(1, 0.5f, 0, 1);

        #region BoxCast

        #region BoxCast Single
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(origin, size, angle, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(origin, size, angle, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(origin, size, angle, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D hitInfo = UEPhysics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                bool collided = hitInfo.collider != null;

                if (collided)
                {
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    distance = hitInfo.distance;
                }

                DebugExtensions.DebugBox(origin, size, direction, distance, M_castColor, Quaternion.Euler(0, 0, angle), collided ? (hitColor ?? Color.green) : M_castColor, true, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }

        public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
        {
            return BoxCast(origin, size, angle, direction, contactFilter, results, M_maxDistance);
        }

        public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.BoxCast(origin, size, angle, direction, contactFilter, results, distance);

            if (preview != PreviewCondition.None)
            {
                size /= 2;

                float previewDistance = 0;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                foreach (RaycastHit2D hit in results)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugBox(origin + direction * hit.distance, size, (hitColor ?? Color.green), rot, drawDuration, preview, drawDepth);

                    if (hit.distance > previewDistance)
                        previewDistance = hit.distance;
                }

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);
                DebugExtensions.DebugBox(origin, size, direction, distance, M_castColor, rot, M_castColor, true, drawDuration, preview, drawDepth);
            }

            return count;
        }
        #endregion

        #region BoxCast All

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(origin, size, angle, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(origin, size, angle, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(origin, size, angle, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastAll(origin, size, angle, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D[] hitInfo = UEPhysics2D.BoxCastAll(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                float previewDistance = 0;
                Quaternion rot = Quaternion.Euler(0, 0, angle);

                foreach (RaycastHit2D hit in hitInfo)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugBox(origin + direction * hit.distance, size, (hitColor ?? Color.green), rot, drawDuration, preview, drawDepth);

                    if (hit.distance > previewDistance)
                        previewDistance = hit.distance;
                }

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);
                DebugExtensions.DebugBox(origin, size, direction, distance, M_castColor, rot, M_castColor, true, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }
        #endregion

        #region BoxCast non alloc

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(origin, size, angle, direction, results, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(origin, size, angle, direction, results, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(origin, size, angle, direction, results, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return BoxCastNonAlloc(origin, size, angle, direction, results, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.BoxCastNonAlloc(origin, size, angle, direction, results, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;

                float previewDistance = 0;
                Quaternion rot = Quaternion.Euler(0, 0, angle);

                for (int i = 0; i < count; i++)
                {
                    RaycastHit2D hit = results[i];
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugBox(origin + direction * hit.distance, size, (hitColor ?? Color.green), rot, drawDuration, preview, drawDepth);

                    if (hit.distance > previewDistance)
                        previewDistance = hit.distance;
                }

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);
                DebugExtensions.DebugBox(origin, size, direction, distance, M_castColor, rot, M_castColor, true, drawDuration, preview, drawDepth);
            }

            return count;
        }
        #endregion

        #endregion

        #region CapsuleCast

        #region CapsuleCast single

        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D hitInfo = UEPhysics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (hitInfo.collider != null)
                {
                    distance = hitInfo.distance;
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                }

                if (capsuleDirection == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + origin;
                point2 = (Vector2)(rot * point2) + origin;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, origin + direction * distance, hitInfo ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, origin + direction * distance, hitInfo ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugOneSidedCapsule(point1 + direction * distance, point2 + direction * distance, hitInfo ? (hitColor ?? Color.green) : M_castColor, radius, true, drawDuration, preview, drawDepth);

            }

            return hitInfo;
        }

        public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, contactFilter, results, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, contactFilter, results, distance);

            if (preview != PreviewCondition.None)
            {
                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (capsuleDirection == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + origin;
                point2 = (Vector2)(rot * point2) + origin;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                foreach (RaycastHit2D hitInfo in results)
                {
                    if (hitInfo.collider != null)
                    {
                        distance = hitInfo.distance;
                        DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    }
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, origin + direction * distance, count != 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, origin + direction * distance, count != 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugOneSidedCapsule(point1 + direction * distance, point2 + direction * distance, count != 0 ? (hitColor ?? Color.green) : M_castColor, radius, true, drawDuration, preview, drawDepth);
            }

            return count;
        }
        #endregion

        #region CapsuleCast All

        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(origin, size, capsuleDirection, angle, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(origin, size, capsuleDirection, angle, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(origin, size, capsuleDirection, angle, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastAll(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D[] hitInfo = UEPhysics2D.CapsuleCastAll(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                bool collided = false;
                float maxDistanceRay = 0;

                if (capsuleDirection == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + origin;
                point2 = (Vector2)(rot * point2) + origin;

                foreach (RaycastHit2D hit in hitInfo)
                {
                    collided = true;

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugOneSidedCapsule(point1 + direction * hit.distance, point2 + direction * hit.distance, (hitColor ?? Color.green), radius, true, drawDuration, preview, drawDepth);
                }

                Vector2 midPoint = (point2 + point1) / 2;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = midPoint + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    GLDebug.DrawLine(endCollisionPoint, midPoint + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugOneSidedCapsule(point1 + direction * distance, point2 + direction * distance, M_castColor, radius, true, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }
        #endregion

        #region CapsuleCast non alloc

        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, direction, results, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, direction, results, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, direction, results, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, direction, results, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, direction, results, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                bool collided = false;
                float maxDistanceRay = 0;

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (capsuleDirection == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + origin;
                point2 = (Vector2)(rot * point2) + origin;


                for (int i = 0; i < count; i++)
                {
                    collided = true;

                    RaycastHit2D hit = results[i];

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    DebugExtensions.DebugOneSidedCapsule(point1 + direction * hit.distance, point2 + direction * hit.distance, (hitColor ?? Color.green), radius, true, drawDuration, preview, drawDepth);
                }

                Vector2 midPoint = (point2 + point1) / 2;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, M_castColor, radius, true, drawDuration, preview, drawDepth);

                Vector3 endCollisionPoint = midPoint + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(midPoint, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    Debug.DrawLine(endCollisionPoint, midPoint + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugOneSidedCapsule(point1 + direction * distance, point2 + direction * distance, M_castColor, radius, true, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region CircleCast

        #region CircleCast single

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCast(origin, radius, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCast(origin, radius, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCast(origin, radius, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCast(origin, radius, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D hitInfo = UEPhysics2D.CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                bool collided = hitInfo.collider != null;

                if (collided)
                {
                    DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    distance = hitInfo.distance;
                }

                DebugExtensions.DebugCircle(origin, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, origin + direction * distance, hitInfo ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, origin + direction * distance, hitInfo ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugCircle(origin + direction * distance, Vector3.forward, hitInfo ? (hitColor ?? Color.green) : M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }

        public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCast(origin, radius, direction, contactFilter, results, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.CircleCast(origin, radius, direction, contactFilter, results, distance);

            if (preview != PreviewCondition.None)
            {
                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                bool notEmpty = false;
                foreach (var hitInfo in results)
                {
                    bool collided = hitInfo.collider != null;

                    if (collided)
                    {
                        notEmpty = true;
                        DebugExtensions.DebugPoint(hitInfo.point, Color.red, 0.5f, drawDuration, preview, drawDepth);
                        distance = hitInfo.distance;
                    }
                }

                DebugExtensions.DebugCircle(origin, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, origin + direction * distance, notEmpty ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, origin + direction * distance, notEmpty ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);

                DebugExtensions.DebugCircle(origin + direction * distance, Vector3.forward, notEmpty ? (hitColor ?? Color.green) : M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #region CircleCast All

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastAll(origin, radius, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastAll(origin, radius, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastAll(origin, radius, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastAll(origin, radius, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D[] hitInfo = UEPhysics2D.CircleCastAll(origin, radius, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                float maxDistanceRay = 0;
                bool collided = false;

                foreach (RaycastHit2D hit in hitInfo)
                {
                    if (hit.collider)
                        collided = true;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugCircle(origin + direction * hit.distance, Vector3.forward, (hitColor ?? Color.green), radius, drawDuration, preview, drawDepth);

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;
                }

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                Vector3 endCollisionPoint = origin + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    GLDebug.DrawLine(endCollisionPoint, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugCircle(origin, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);
                DebugExtensions.DebugCircle(origin + direction * distance, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return hitInfo;
        }

        #endregion

        #region CircleCast non alloc

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastNonAlloc(origin, radius, direction, results, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastNonAlloc(origin, radius, direction, results, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastNonAlloc(origin, radius, direction, results, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return CircleCastNonAlloc(origin, radius, direction, results, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int count = UEPhysics2D.CircleCastNonAlloc(origin, radius, direction, results, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                float maxDistanceRay = 0;
                bool collided = false;

                foreach (RaycastHit2D hit in results)
                {
                    if (hit.collider)
                        collided = true;

                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    DebugExtensions.DebugCircle(origin + direction * hit.distance, Vector3.forward, (hitColor ?? Color.green), radius, drawDuration, preview, drawDepth);

                    if (hit.distance > maxDistanceRay)
                        maxDistanceRay = hit.distance;
                }

                distance = (distance == M_maxDistance ? 1000 * 1000 : distance);

                Vector3 endCollisionPoint = origin + direction * maxDistanceRay;

                if (preview == PreviewCondition.Editor)
                {
                    Debug.DrawLine(origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                    Debug.DrawLine(endCollisionPoint, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }
                else
                {
                    GLDebug.DrawLine(origin, endCollisionPoint, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration, true);
                    GLDebug.DrawLine(endCollisionPoint, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                }

                DebugExtensions.DebugCircle(origin, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);
                DebugExtensions.DebugCircle(origin + direction * distance, Vector3.forward, M_castColor, radius, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region LineCast

        #region LineCast single

        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Linecast(start, end, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Linecast(start, end, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Linecast(start, end, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Vector2 direction = (end - start);
            return Raycast(start, direction, direction.magnitude, layerMask, minDepth, maxDepth, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Vector2 direction = (end - start);
            return Raycast(start, direction, contactFilter, results, direction.magnitude, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        #endregion

        #region LineCast All

        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastAll(start, end, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastAll(start, end, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastAll(start, end, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Vector2 direction = (end - start);
            return RaycastAll(start, direction, direction.magnitude, layerMask, minDepth, maxDepth, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        #endregion

        #region LineCast non alloc

        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastNonAlloc(start, end, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastNonAlloc(start, end, results, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return LinecastNonAlloc(start, end, results, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Vector2 direction = (end - start);
            return RaycastNonAlloc(start, direction, results, direction.magnitude, layerMask, minDepth, maxDepth, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        #endregion

        #endregion

        #region OverlapArea

        #region OverlapArea single

        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapArea(pointA, pointB, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapArea(pointA, pointB, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapArea(pointA, pointB, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D collider = UEPhysics2D.OverlapArea(pointA, pointB, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector2 topLeft = pointA;
                Vector2 bottomLeft = new Vector2(pointA.x, pointB.y);
                Vector2 bottomRight = pointB;
                Vector2 topRight = new Vector2(pointB.x, pointA.y);

                Color color = collider != null ? (hitColor ?? Color.green) : (noHitColor ?? Color.red);

                //|
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topLeft, bottomLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topLeft, bottomLeft, color, drawDuration);

                //_
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomLeft, bottomRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomLeft, bottomRight, color, drawDuration);

                // |
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomRight, topRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomRight, topRight, color, drawDuration);

                //-
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topRight, topLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topRight, topLeft, color, drawDuration);
            }

            return collider;
        }

        #endregion

        #region OverlapArea all

        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaAll(pointA, pointB, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaAll(pointA, pointB, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaAll(pointA, pointB, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D[] colliders = UEPhysics2D.OverlapAreaAll(pointA, pointB, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector2 topLeft = pointA;
                Vector2 bottomLeft = new Vector2(pointA.x, pointB.y);
                Vector2 bottomRight = pointB;
                Vector2 topRight = new Vector2(pointB.x, pointA.y);

                Color color = colliders.Length > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red);

                //|
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topLeft, bottomLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topLeft, bottomLeft, color, drawDuration);

                //_
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomLeft, bottomRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomLeft, bottomRight, color, drawDuration);

                // |
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomRight, topRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomRight, topRight, color, drawDuration);

                //-
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topRight, topLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topRight, topLeft, color, drawDuration);
            }

            return colliders;
        }

        #endregion

        #region OverlapArea non alloc

        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaNonAlloc(pointA, pointB, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaNonAlloc(pointA, pointB, results, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapAreaNonAlloc(pointA, pointB, results, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int size = UEPhysics2D.OverlapAreaNonAlloc(pointA, pointB, results, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector2 topLeft = pointA;
                Vector2 bottomLeft = new Vector2(pointA.x, pointB.y);
                Vector2 bottomRight = pointB;
                Vector2 topRight = new Vector2(pointB.x, pointA.y);

                Color color = size > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red);

                //|
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topLeft, bottomLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topLeft, bottomLeft, color, drawDuration);

                //_
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomLeft, bottomRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomLeft, bottomRight, color, drawDuration);

                // |
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(bottomRight, topRight, color, drawDuration);
                else
                    GLDebug.DrawLine(bottomRight, topRight, color, drawDuration);

                //-
                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(topRight, topLeft, color, drawDuration);
                else
                    GLDebug.DrawLine(topRight, topLeft, color, drawDuration);
            }

            return size;
        }

        #endregion

        #endregion

        #region OverlapBox

        #region OverlapBox single

        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(point, size, angle, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(point, size, angle, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBox(point, size, angle, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D collider = UEPhysics2D.OverlapBox(point, size, angle, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                DebugExtensions.DebugBox(point, size, collider ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), Quaternion.Euler(0, 0, angle), drawDuration, preview, drawDepth);
            }

            return collider;
        }

        public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapBox(point, size, angle, contactFilter, results);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                DebugExtensions.DebugBox(point, size, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), Quaternion.Euler(0, 0, angle), drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #region OverlapBox all

        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxAll(point, size, angle, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxAll(point, size, angle, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxAll(point, size, angle, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D[] colliders = UEPhysics2D.OverlapBoxAll(point, size, angle, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                DebugExtensions.DebugBox(point, size, colliders != null && colliders.Length > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), Quaternion.Euler(0, 0, angle), drawDuration, preview, drawDepth);
            }

            return colliders;
        }

        #endregion

        #region OverlapBox non alloc

        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(point, size, angle, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(point, size, angle, results, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapBoxNonAlloc(point, size, angle, results, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapBoxNonAlloc(point, size, angle, results, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                DebugExtensions.DebugBox(point, size, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), Quaternion.Euler(0, 0, angle), drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region OverlapCapsule

        #region OverlapCapsule single

        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsule(point, size, direction, angle, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsule(point, size, direction, angle, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsule(point, size, direction, angle, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D collider = UEPhysics2D.OverlapCapsule(point, size, direction, angle, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                if (direction == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                Quaternion rot = Quaternion.Euler(0, 0, angle);
                point1 = (Vector2)(rot * point1) + point;
                point2 = (Vector2)(rot * point2) + point;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, collider ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, true, drawDuration, preview, drawDepth);
            }

            return collider;
        }

        public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapCapsule(point, size, direction, angle, contactFilter, results);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (direction == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + point;
                point2 = (Vector2)(rot * point2) + point;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, true, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #region OverlapCapsule all

        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleAll(point, size, direction, angle, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleAll(point, size, direction, angle, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleAll(point, size, direction, angle, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D[] colliders = UEPhysics2D.OverlapCapsuleAll(point, size, direction, angle, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (direction == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + point;
                point2 = (Vector2)(rot * point2) + point;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, colliders != null && colliders.Length > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, true, drawDuration, preview, drawDepth);
            }

            return colliders;
        }

        #endregion

        #region OverlapCapsule non alloc

        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleNonAlloc(point, size, direction, angle, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleNonAlloc(point, size, direction, angle, results, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCapsuleNonAlloc(point, size, direction, angle, results, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapCapsuleNonAlloc(point, size, direction, angle, results, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                size /= 2;
                Vector2 point1;
                Vector2 point2;
                float radius;

                Quaternion rot = Quaternion.Euler(0, 0, angle);

                if (direction == CapsuleDirection2D.Vertical)
                {
                    if (size.y > size.x)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.x;
                }
                else
                {
                    if (size.x > size.y)
                    {
                        point1 = new Vector3(0, 0 - size.y + size.x);
                        point2 = new Vector3(0, 0 + size.y - size.x);
                    }
                    else
                    {
                        point1 = new Vector3(0 - .01f, 0);
                        point2 = new Vector3(0 + .01f, 0);
                    }

                    radius = size.y;
                }

                point1 = (Vector2)(rot * point1) + point;
                point2 = (Vector2)(rot * point2) + point;

                DebugExtensions.DebugOneSidedCapsule(point1, point2, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, true, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region OverlapCircle

        #region OverlapCircle single

        public static Collider2D OverlapCircle(Vector2 point, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircle(point, radius, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircle(point, radius, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircle(point, radius, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D collider = UEPhysics2D.OverlapCircle(point, radius, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugCircle(point, Vector3.forward, collider ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return collider;
        }

        public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapCircle(point, radius, contactFilter, results);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugCircle(point, Vector3.forward, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #region OverlapCircle all

        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleAll(point, radius, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleAll(point, radius, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleAll(point, radius, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D[] colliders = UEPhysics2D.OverlapCircleAll(point, radius, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugCircle(point, Vector3.forward, colliders != null && colliders.Length > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return colliders;
        }

        #endregion

        #region OverlapCircle non alloc

        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleNonAlloc(point, radius, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleNonAlloc(point, radius, results, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapCircleNonAlloc(point, radius, results, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapCircleNonAlloc(point, radius, results, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugCircle(point, Vector3.forward, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), radius, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region OverlapPoint

        #region OverlapPoint single

        public static Collider2D OverlapPoint(Vector2 point, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPoint(point, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapPoint(Vector2 point, int layerMask, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPoint(point, layerMask, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPoint(point, layerMask, minDepth, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth, float maxDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D collider = UEPhysics2D.OverlapPoint(point, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugPoint(point, collider ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), size, drawDuration, preview, drawDepth);
            }

            return collider;
        }

        public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, Collider2D[] results, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapPoint(point, contactFilter, results);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugPoint(point, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), size, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #region OverlapPoint all

        public static Collider2D[] OverlapPointAll(Vector2 point, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointAll(point, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointAll(point, layerMask, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointAll(point, layerMask, minDepth, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth, float maxDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            Collider2D[] colliders = UEPhysics2D.OverlapPointAll(point, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugPoint(point, colliders != null && colliders.Length > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), size, drawDuration, preview, drawDepth);
            }

            return colliders;
        }

        #endregion

        #region OverlapPoint non alloc

        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointNonAlloc(point, results, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointNonAlloc(point, results, layerMask, -M_maxDistance, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return OverlapPointNonAlloc(point, results, layerMask, minDepth, M_maxDistance, size, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth, float maxDepth, float size = 6, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            int count = UEPhysics2D.OverlapPointNonAlloc(point, results, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                DebugExtensions.DebugPoint(point, count > 0 ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), size, drawDuration, preview, drawDepth);
            }

            return count;
        }

        #endregion

        #endregion

        #region RayCast

        #region RayCast single

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D hitInfo = UEPhysics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector3 end = origin + direction * (distance == M_maxDistance ? 1000 * 1000 : distance);
                bool collided = false;

                if (hitInfo.collider != null)
                {
                    collided = true;
                    end = hitInfo.point;

                    DebugExtensions.DebugPoint(end, Color.red, 0.5f, drawDuration, preview, drawDepth);
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
            }

            return hitInfo;
        }


        public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return Raycast(origin, direction, contactFilter, results, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int size = UEPhysics2D.Raycast(origin, direction, contactFilter, results, distance);

            if (preview != PreviewCondition.None)
            {
                Vector3 end = origin + direction * (distance == M_maxDistance ? 1000 * 1000 : distance);
                bool collided = false;

                foreach (var hitInfo in results)
                {
                    if (hitInfo.collider != null)
                    {
                        collided = true;
                        end = hitInfo.point;

                        DebugExtensions.DebugPoint(end, Color.red, 0.5f, drawDuration, preview, drawDepth);
                    }
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(origin, end, collided ? (hitColor ?? Color.green) : (noHitColor ?? Color.red), drawDuration);
            }

            return size;
        }

        #endregion

        #region RayCast all

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastAll(origin, direction, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            RaycastHit2D[] raycastInfo = UEPhysics2D.RaycastAll(origin, direction, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector3 previewOrigin = origin;
                Vector3 sectionOrigin = origin;

                foreach (RaycastHit2D hit in raycastInfo)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    if (preview == PreviewCondition.Editor)
                        Debug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);
                    else
                        GLDebug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);

                    if (Vector3.Distance(origin, hit.point) > Vector3.Distance(origin, previewOrigin))
                        previewOrigin = hit.point;

                    sectionOrigin = hit.point;
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(previewOrigin, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(previewOrigin, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
            }

            return raycastInfo;
        }

        #endregion

        #region RayCast non alloc

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, M_maxDistance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, distance, UEPhysics2D.AllLayers, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, distance, layerMask, -M_maxDistance, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            return RaycastNonAlloc(origin, direction, results, distance, layerMask, minDepth, M_maxDistance, preview, drawDuration, hitColor, noHitColor, drawDepth);
        }

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth, PreviewCondition preview = PreviewCondition.None, float drawDuration = 0, Color? hitColor = null, Color? noHitColor = null, bool drawDepth = false)
        {
            direction.Normalize();
            int size = UEPhysics2D.RaycastNonAlloc(origin, direction, results, distance, layerMask, minDepth, maxDepth);

            if (preview != PreviewCondition.None)
            {
                Vector3 previewOrigin = origin;
                Vector3 sectionOrigin = origin;

                foreach (RaycastHit2D hit in results)
                {
                    DebugExtensions.DebugPoint(hit.point, Color.red, 0.5f, drawDuration, preview, drawDepth);

                    if (preview == PreviewCondition.Editor)
                        Debug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);
                    else
                        GLDebug.DrawLine(sectionOrigin, hit.point, (hitColor ?? Color.green), drawDuration);

                    if (Vector3.Distance(origin, hit.point) > Vector3.Distance(origin, previewOrigin))
                        previewOrigin = hit.point;

                    sectionOrigin = hit.point;
                }

                if (preview == PreviewCondition.Editor)
                    Debug.DrawLine(previewOrigin, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
                else
                    GLDebug.DrawLine(previewOrigin, origin + direction * distance, (noHitColor ?? Color.red), drawDuration);
            }

            return size;
        }

        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Class used to draw additional debugs, this was based of the Debug Drawing Extension from the asset store (https://www.assetstore.unity3d.com/en/#!/content/11396)
    /// </summary>
    public static class DebugExtensions
    {
        public static void DebugSquare(Vector3 origin, Vector3 halfExtents, Color color, Quaternion orientation,
                                       float drawDuration = 0, PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            Vector3 forward = orientation * Vector3.forward;
            Vector3 up = orientation * Vector3.up;
            Vector3 right = orientation * Vector3.right;

            Vector3 topMinY1 = origin + (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 topMaxY1 = origin - (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 botMinY1 = origin + (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 botMaxY1 = origin - (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                Debug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
            }

            if (drawGame)
            { 
                GLDebug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
            }
        }

        public static void DebugBox(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance, Color color,
                                    Quaternion orientation, Color endColor, bool drawBase = true, float drawDuration = 0,
                                    PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            direction.Normalize();
            Vector3 end = origin + direction * (float.IsPositiveInfinity(maxDistance) ? 1000 * 1000 : maxDistance);
            
            Vector3 forward = orientation * Vector3.forward;
            Vector3 up = orientation * Vector3.up;
            Vector3 right = orientation * Vector3.right;

            #region Coords
            #region End coords
            //trans.position = end;
            Vector3 topMinX0 = end + (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMaxX0 = end - (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMinY0 = end + (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 topMaxY0 = end - (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);

            Vector3 botMinX0 = end + (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMaxX0 = end - (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMinY0 = end + (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 botMaxY0 = end - (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            #endregion

            #region Origin coords
            //trans.position = origin;
            Vector3 topMinX1 = origin + (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMaxX1 = origin - (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMinY1 = origin + (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 topMaxY1 = origin - (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);

            Vector3 botMinX1 = origin + (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMaxX1 = origin - (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMinY1 = origin + (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 botMaxY1 = origin - (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            #endregion
            #endregion

            #region Draw lines
            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                #region Origin box
                if (drawBase)
                {
                    Debug.DrawLine(topMinX1, botMinX1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMaxX1, botMaxX1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);

                    Debug.DrawLine(topMinX1, topMaxX1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMinX1, topMinY1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                    Debug.DrawLine(topMaxY1, topMaxX1, color, drawDuration, drawDepth);

                    Debug.DrawLine(botMinX1, botMaxX1, color, drawDuration, drawDepth);
                    Debug.DrawLine(botMinX1, botMinY1, color, drawDuration, drawDepth);
                    Debug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
                    Debug.DrawLine(botMaxY1, botMaxX1, color, drawDuration, drawDepth);
                }
                #endregion

                #region Connection between boxes
                Debug.DrawLine(topMinX0, topMinX1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxX0, topMaxX1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY0, topMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY0, topMaxY1, color, drawDuration, drawDepth);

                Debug.DrawLine(botMinX0, botMinX1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinX0, botMinX1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinY0, botMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMaxY0, botMaxY1, color, drawDuration, drawDepth);
                #endregion

                #region End box
                color = endColor;

                Debug.DrawLine(topMinX0, botMinX0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxX0, botMaxX0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY0, botMinY0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY0, botMaxY0, color, drawDuration, drawDepth);

                Debug.DrawLine(topMinX0, topMaxX0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinX0, topMinY0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY0, topMaxY0, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY0, topMaxX0, color, drawDuration, drawDepth);

                Debug.DrawLine(botMinX0, botMaxX0, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinX0, botMinY0, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinY0, botMaxY0, color, drawDuration, drawDepth);
                Debug.DrawLine(botMaxY0, botMaxX0, color, drawDuration, drawDepth);
                #endregion
            }

            if (drawGame)
            {
                #region Origin box
                if (drawBase)
                {
                    GLDebug.DrawLine(topMinX1, botMinX1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMaxX1, botMaxX1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);

                    GLDebug.DrawLine(topMinX1, topMaxX1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMinX1, topMinY1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(topMaxY1, topMaxX1, color, drawDuration, drawDepth);

                    GLDebug.DrawLine(botMinX1, botMaxX1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(botMinX1, botMinY1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(botMaxY1, botMaxX1, color, drawDuration, drawDepth);
                }
                #endregion

                #region Connection between boxes
                GLDebug.DrawLine(topMinX0, topMinX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxX0, topMaxX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY0, topMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY0, topMaxY1, color, drawDuration, drawDepth);

                GLDebug.DrawLine(botMinX0, botMinX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinX0, botMinX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinY0, botMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMaxY0, botMaxY1, color, drawDuration, drawDepth);
                #endregion

                #region End box
                color = endColor;

                GLDebug.DrawLine(topMinX0, botMinX0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxX0, botMaxX0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY0, botMinY0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY0, botMaxY0, color, drawDuration, drawDepth);

                GLDebug.DrawLine(topMinX0, topMaxX0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinX0, topMinY0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY0, topMaxY0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY0, topMaxX0, color, drawDuration, drawDepth);

                GLDebug.DrawLine(botMinX0, botMaxX0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinX0, botMinY0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinY0, botMaxY0, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMaxY0, botMaxX0, color, drawDuration, drawDepth);
                #endregion
            }
            #endregion
        }

        public static void DebugBox(Vector3 origin, Vector3 halfExtents, Color color, Quaternion orientation,
                                    float drawDuration = 0, PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            Vector3 forward = orientation * Vector3.forward;
            Vector3 up = orientation * Vector3.up;
            Vector3 right = orientation * Vector3.right;

            Vector3 topMinX1 = origin + (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMaxX1 = origin - (right * halfExtents.x) + (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 topMinY1 = origin + (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 topMaxY1 = origin - (right * halfExtents.x) + (up * halfExtents.y) + (forward * halfExtents.z);

            Vector3 botMinX1 = origin + (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMaxX1 = origin - (right * halfExtents.x) - (up * halfExtents.y) - (forward * halfExtents.z);
            Vector3 botMinY1 = origin + (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            Vector3 botMaxY1 = origin - (right * halfExtents.x) - (up * halfExtents.y) + (forward * halfExtents.z);
            
            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                Debug.DrawLine(topMinX1, botMinX1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxX1, botMaxX1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);

                Debug.DrawLine(topMinX1, topMaxX1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinX1, topMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                Debug.DrawLine(topMaxY1, topMaxX1, color, drawDuration, drawDepth);

                Debug.DrawLine(botMinX1, botMaxX1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinX1, botMinY1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
                Debug.DrawLine(botMaxY1, botMaxX1, color, drawDuration, drawDepth);
            }

            if (drawGame)
            {
                GLDebug.DrawLine(topMinX1, botMinX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxX1, botMaxX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY1, botMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY1, botMaxY1, color, drawDuration, drawDepth);

                GLDebug.DrawLine(topMinX1, topMaxX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinX1, topMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMinY1, topMaxY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(topMaxY1, topMaxX1, color, drawDuration, drawDepth);

                GLDebug.DrawLine(botMinX1, botMaxX1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinX1, botMinY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMinY1, botMaxY1, color, drawDuration, drawDepth);
                GLDebug.DrawLine(botMaxY1, botMaxX1, color, drawDuration, drawDepth);
            }
        }

        public static void DebugOneSidedCapsule(Vector3 baseSphere, Vector3 endSphere, Color color, float radius = 1,
                                                bool colorizeBase = false, float drawDuration = 0,
                                                PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            Vector3 up = (endSphere - baseSphere).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                //Side lines
                Debug.DrawLine(baseSphere + right, endSphere + right, color, drawDuration, drawDepth);
                Debug.DrawLine(baseSphere - right, endSphere - right, color, drawDuration, drawDepth);

                //Draw end caps
                for (int i = 1; i < 26; i++)
                {
                    //Start endcap
                    Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);

                    //End endcap
                    Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + endSphere, Vector3.Slerp(right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + endSphere, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                }
            }

            if (drawGame)
            {
                //Side lines
                GLDebug.DrawLine(baseSphere + right, endSphere + right, color, drawDuration, drawDepth);
                GLDebug.DrawLine(baseSphere - right, endSphere - right, color, drawDuration, drawDepth);

                //Draw end caps
                for (int i = 1; i < 26; i++)
                {
                    //Start endcap
                    GLDebug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);

                    //End endcap
                    GLDebug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + endSphere, Vector3.Slerp(right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + endSphere, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                }
            }
        }

        public static void DebugCapsule(Vector3 baseSphere, Vector3 endSphere, Color color, float radius = 1,
                                        bool colorizeBase = true, float drawDuration = 0,
                                        PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            Vector3 up = (endSphere - baseSphere).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            //Radial circles
            DebugCircle(baseSphere, up, colorizeBase ? color : Color.red, radius, drawDuration, preview, drawDepth);
            DebugCircle(endSphere, -up, color, radius, drawDuration, preview, drawDepth);

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                //Side lines
                Debug.DrawLine(baseSphere + right, endSphere + right, color, drawDuration, drawDepth);
                Debug.DrawLine(baseSphere - right, endSphere - right, color, drawDuration, drawDepth);

                Debug.DrawLine(baseSphere + forward, endSphere + forward, color, drawDuration, drawDepth);
                Debug.DrawLine(baseSphere - forward, endSphere - forward, color, drawDuration, drawDepth);

                //Draw end caps
                for (int i = 1; i < 26; i++)
                {
                    //End endcap
                    Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + endSphere, Vector3.Slerp(right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + endSphere, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + endSphere, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + endSphere, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);

                    //Start endcap
                    Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + baseSphere, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                }
            }

            if (drawGame)
            {
                //Side lines
                GLDebug.DrawLine(baseSphere + right, endSphere + right, color, drawDuration, drawDepth);
                GLDebug.DrawLine(baseSphere - right, endSphere - right, color, drawDuration, drawDepth);

                GLDebug.DrawLine(baseSphere + forward, endSphere + forward, color, drawDuration, drawDepth);
                GLDebug.DrawLine(baseSphere - forward, endSphere - forward, color, drawDuration, drawDepth);

                //Draw end caps
                for (int i = 1; i < 26; i++)
                {
                    //End endcap
                    GLDebug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + endSphere, Vector3.Slerp(right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + endSphere, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + endSphere, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + endSphere, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + endSphere, color, drawDuration, drawDepth);

                    //Start endcap
                    GLDebug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + baseSphere, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                    GLDebug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + baseSphere, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + baseSphere, colorizeBase ? color : Color.red, drawDuration, drawDepth);
                }
            }
        }

        public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f,
                                       float drawDuration = 0, PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            Vector3 upDir = up.normalized * radius;
            Vector3 forwardDir = Vector3.Slerp(upDir, -upDir, 0.5f);
            Vector3 rightDir = Vector3.Cross(upDir, forwardDir).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = rightDir.x;
            matrix[1] = rightDir.y;
            matrix[2] = rightDir.z;

            matrix[4] = upDir.x;
            matrix[5] = upDir.y;
            matrix[6] = upDir.z;

            matrix[8] = forwardDir.x;
            matrix[9] = forwardDir.y;
            matrix[10] = forwardDir.z;

            Vector3 lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 nextPoint = Vector3.zero;

            color = (color == default(Color)) ? Color.white : color;

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            for (var i = 0; i < 91; i++)
            {
                nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                nextPoint.y = 0;

                nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);

                if (drawEditor)
                    Debug.DrawLine(lastPoint, nextPoint, color, drawDuration, drawDepth);
                    
                if (drawGame)
                    GLDebug.DrawLine(lastPoint, nextPoint, color, drawDuration, drawDepth);
                    
                lastPoint = nextPoint;
            }
        }

        public static void DebugPoint(Vector3 position, Color color, float scale = 0.5f, float drawDuration = 0,
                                      PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            color = (color == default(Color)) ? Color.white : color;

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            if (drawEditor)
            {
                Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, drawDuration, drawDepth);
                Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, drawDuration, drawDepth);
                Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, drawDuration, drawDepth);
             }

            if (drawGame)
            {
                GLDebug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, drawDuration, drawDepth);
                GLDebug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, drawDuration, drawDepth);
                GLDebug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, drawDuration, drawDepth);
             }   
        }

        public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f, float drawDuration = 0,
                                           PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            float angle = 10.0f;

            Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
            Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
            Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            for (int i = 1; i < 37; i++)
            {
                Vector3 new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
                Vector3 new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
                Vector3 new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

                if (drawEditor)
                {
                    Debug.DrawLine(x, new_x, color, drawDuration, drawDepth);
                    Debug.DrawLine(y, new_y, color, drawDuration, drawDepth);
                    Debug.DrawLine(z, new_z, color, drawDuration, drawDepth);
                }

                if (drawGame)
                {
                    GLDebug.DrawLine(x, new_x, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(y, new_y, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(z, new_z, color, drawDuration, drawDepth);
                }
                
                x = new_x;
                y = new_y;
                z = new_z;
            }
        }

        public static void DebugConeSight(Vector3 position, Vector3 direction, float length, Color color, float angle = 45,
                                          float drawDuration = 0, PreviewCondition preview = PreviewCondition.Editor, bool drawDepth = false)
        {
            if (angle > 0)
                angle = Mathf.Min(angle, 360);
            else
                angle = Mathf.Max(angle, -360);

            direction.Normalize();
            Vector3 forwardDir = direction * length;
            Vector3 upDir = Vector3.Slerp(forwardDir, -forwardDir, 0.5f);
            Vector3 rightDir = Vector3.Cross(forwardDir, upDir).normalized * length;

            Vector3 up;
            Vector3 rightUp;
            Vector3 right;
            Vector3 rightDown;
            Vector3 down;
            Vector3 leftDown;
            Vector3 left;
            Vector3 leftUp;

            if (angle <= 180)
            {
                float percentage = angle / 180f;

                up = position + Vector3.Slerp(forwardDir, -rightDir, percentage).normalized * length;
                rightUp = position + Vector3.Slerp(forwardDir, -upDir - rightDir, percentage).normalized * length;
                right = position + Vector3.Slerp(forwardDir, -upDir, percentage).normalized * length;
                rightDown = position + Vector3.Slerp(forwardDir, -upDir + rightDir, percentage).normalized * length;
                down = position + Vector3.Slerp(forwardDir, rightDir, percentage).normalized * length;
                leftDown = position + Vector3.Slerp(forwardDir, upDir + rightDir, percentage).normalized * length;
                left = position + Vector3.Slerp(forwardDir, upDir, percentage).normalized * length;
                leftUp = position + Vector3.Slerp(forwardDir, upDir - rightDir, percentage).normalized * length;
            }
            else
            {
                float percentage = (angle - 180) / 180f;

                up = position + Vector3.Slerp(-rightDir, -forwardDir, percentage).normalized * length;
                rightUp = position + Vector3.Slerp(-upDir - rightDir, -forwardDir, percentage).normalized * length;
                right = position + Vector3.Slerp(-upDir, -forwardDir, percentage).normalized * length;
                rightDown = position + Vector3.Slerp(-upDir + rightDir, -forwardDir, percentage).normalized * length;
                down = position + Vector3.Slerp(rightDir, -forwardDir, percentage).normalized * length;
                leftDown = position + Vector3.Slerp(upDir + rightDir, -forwardDir, percentage).normalized * length;
                left = position + Vector3.Slerp(upDir, -forwardDir, percentage).normalized * length;
                leftUp = position + Vector3.Slerp(upDir - rightDir, -forwardDir, percentage).normalized * length;
            }

            bool drawEditor = false;
            bool drawGame = false;
            
            switch (preview)
            {
                case PreviewCondition.Editor:
                    drawEditor = true;
                    break;

                case PreviewCondition.Game:
                    drawGame = true;
                    break;
                    
                case PreviewCondition.Both:
                    drawEditor = true;
                    drawGame = true;
                    break;
            }

            #region Rays Logic
            if (drawEditor)
            {
                //Forward
                Debug.DrawRay(position, forwardDir, color, drawDuration, drawDepth);

                //Left Down
                Debug.DrawLine(position, leftDown, color, drawDuration, drawDepth);
                //Left Up
                Debug.DrawLine(position, leftUp, color, drawDuration, drawDepth);
                //Right Down
                Debug.DrawLine(position, rightDown, color, drawDuration, drawDepth);
                //Right Up
                Debug.DrawLine(position, rightUp, color, drawDuration, drawDepth);

                //Left
                Debug.DrawLine(position, left, color, drawDuration, drawDepth);
                //Right
                Debug.DrawLine(position, right, color, drawDuration, drawDepth);
                //Down
                Debug.DrawLine(position, down, color, drawDuration, drawDepth);
                //Up
                Debug.DrawLine(position, up, color, drawDuration, drawDepth);
            }
            
            if (drawGame)
            {
                //Forward
                GLDebug.DrawRay(position, forwardDir, color, drawDuration, drawDepth);

                //Left Down
                GLDebug.DrawLine(position, leftDown, color, drawDuration, drawDepth);
                //Left Up
                GLDebug.DrawLine(position, leftUp, color, drawDuration, drawDepth);
                //Right Down
                GLDebug.DrawLine(position, rightDown, color, drawDuration, drawDepth);
                //Right Up
                GLDebug.DrawLine(position, rightUp, color, drawDuration, drawDepth);

                //Left
                GLDebug.DrawLine(position, left, color, drawDuration, drawDepth);
                //Right
                GLDebug.DrawLine(position, right, color, drawDuration, drawDepth);
                //Down
                GLDebug.DrawLine(position, down, color, drawDuration, drawDepth);
                //Up
                GLDebug.DrawLine(position, up, color, drawDuration, drawDepth);
            }
            #endregion

            #region Circles
            Vector3 midUp = (up + position) / 2;
            Vector3 midDown = (down + position) / 2;

            Vector3 endPoint = (up + down) / 2;
            Vector3 midPoint = (midUp + midDown) / 2;

            DebugCircle(endPoint, direction, color, Vector3.Distance(up, down) / 2, drawDuration, preview, drawDepth);
            DebugCircle(midPoint, direction, color, Vector3.Distance(midUp, midDown) / 2, drawDuration, preview, drawDepth);
            #endregion

            #region Cone base sphere logic
            Vector3 lastLdPosition = leftDown;
            Vector3 lastLuPosition = leftUp;
            Vector3 lastRdPosition = rightDown;
            Vector3 lastRuPosition = rightUp;

            Vector3 lastLPosition = left;
            Vector3 lastRPosition = right;
            Vector3 lastDPosition = down;
            Vector3 lastUPosition = up;
            
            int index = 1;

            for (int i = 0; i < 7; i++)
            {
                float tempAngle = angle - index;

                Vector3 nextLdPosition;
                Vector3 nextLuPosition;
                Vector3 nextRdPosition;
                Vector3 nextRuPosition;

                Vector3 nextLPosition;
                Vector3 nextRPosition;
                Vector3 nextDPosition;
                Vector3 nextUPosition;
                
                if (tempAngle <= 180)
                {
                    float percentage = tempAngle / 180f;

                    nextLdPosition = position + Vector3.Slerp(forwardDir, upDir + rightDir, percentage).normalized * length;
                    nextLuPosition = position + Vector3.Slerp(forwardDir, upDir - rightDir, percentage).normalized * length;
                    nextRdPosition = position + Vector3.Slerp(forwardDir, -upDir + rightDir, percentage).normalized * length;
                    nextRuPosition = position + Vector3.Slerp(forwardDir, -upDir - rightDir, percentage).normalized * length;

                    nextDPosition = position + Vector3.Slerp(forwardDir, rightDir, percentage).normalized * length;
                    nextUPosition = position + Vector3.Slerp(forwardDir, -rightDir, percentage).normalized * length;
                    nextRPosition = position + Vector3.Slerp(forwardDir, -upDir, percentage).normalized * length;
                    nextLPosition = position + Vector3.Slerp(forwardDir, upDir, percentage).normalized * length;
                }
                else
                {
                    float percentage = (tempAngle - 180) / 180f;

                    nextLdPosition = position + Vector3.Slerp(upDir + rightDir, -forwardDir, percentage).normalized * length;
                    nextLuPosition = position + Vector3.Slerp(upDir - rightDir, -forwardDir, percentage).normalized * length;
                    nextRdPosition = position + Vector3.Slerp(-upDir + rightDir, -forwardDir, percentage).normalized * length;
                    nextRuPosition = position + Vector3.Slerp(-upDir - rightDir, -forwardDir, percentage).normalized * length;

                    nextDPosition = position + Vector3.Slerp(rightDir, -forwardDir, percentage).normalized * length;
                    nextUPosition = position + Vector3.Slerp(-rightDir, -forwardDir, percentage).normalized * length;
                    nextRPosition = position + Vector3.Slerp(-upDir, -forwardDir, percentage).normalized * length;
                    nextLPosition = position + Vector3.Slerp(upDir, -forwardDir, percentage).normalized * length;
                }

                if (drawEditor)
                {
                    Debug.DrawLine(lastLdPosition, nextLdPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastLuPosition, nextLuPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastRdPosition, nextRdPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastRuPosition, nextRuPosition, color, drawDuration, drawDepth);

                    Debug.DrawLine(lastDPosition, nextDPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastUPosition, nextUPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastRPosition, nextRPosition, color, drawDuration, drawDepth);
                    Debug.DrawLine(lastLPosition, nextLPosition, color, drawDuration, drawDepth);
                }

                if (drawGame)
                {
                    GLDebug.DrawLine(lastLdPosition, nextLdPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastLuPosition, nextLuPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastRdPosition, nextRdPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastRuPosition, nextRuPosition, color, drawDuration, drawDepth);

                    GLDebug.DrawLine(lastDPosition, nextDPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastUPosition, nextUPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastRPosition, nextRPosition, color, drawDuration, drawDepth);
                    GLDebug.DrawLine(lastLPosition, nextLPosition, color, drawDuration, drawDepth);
                }

                lastLdPosition = nextLdPosition;
                lastLuPosition = nextLuPosition;
                lastRdPosition = nextRdPosition;
                lastRuPosition = nextRuPosition;

                lastDPosition = nextDPosition;
                lastUPosition = nextUPosition;
                lastRPosition = nextRPosition;
                lastLPosition = nextLPosition;
                index += 60;
            }
            #endregion
        }
    }
}