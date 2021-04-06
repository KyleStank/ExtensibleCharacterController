#if UNITY_EDITOR
using UnityEditor;

using ExtensibleCharacterController.Editor;
#endif

using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using ExtensibleCharacterController.Core.Variables;
using ExtensibleCharacterController.Core.Utility;
using ExtensibleCharacterController.Characters.Behaviours;

namespace ExtensibleCharacterController.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class ECCCharacter : ECCBehaviour
    {
        private const float COLLIDER_OFFSET = 0.01f;

        [Header("Generic Settings")]
        [SerializeField]
        private LayerMask m_CharacterLayer;
        [SerializeField]
        private ECCBoolReference m_UseGravity = true;
        [SerializeField]
        private ECCFloatReference m_Gravity = -Physics.gravity.y;

        [Header("Collision Settings")]
        [SerializeField]
        private ECCIntReference m_MaxCollisions = 10;

        [Header("Ground Settings")]
        [SerializeField]
        private ECCFloatReference m_SkinWidth = 0.1f;
        [SerializeField]
        private ECCFloatReference m_MaxStep = 0.3f;
        [SerializeField]
        private ECCFloatReference m_MaxSlopeAngle = 60.0f;

        [Header("Behaviours")]
        [SerializeField]
        private List<string> m_CharacterBehaviourNames = new List<string>();

        private List<ECCBaseCharacterBehaviour> m_CharacterBehaviours = new List<ECCBaseCharacterBehaviour>();

        private Rigidbody m_Rigidbody = null;
        /// <summary>
        /// Reference to Rigidbody component.
        /// Handle with care. Use API methods as much as possible before using Rigidbody component.
        /// </summary>
        public Rigidbody Rigidbody
        {
            get => m_Rigidbody;
        }

        private CapsuleCollider m_Collider = null;
        private RaycastHit[] m_RaycastHits;
        private Collider[] m_OverlappedColliders;

        private Vector3 m_MoveDirection = Vector3.zero;
        private Vector3 m_GravityDirection = -Vector3.up;
        private float m_GravityFactor = 1.0f;
        private bool m_IsGrounded = false;
        private IECCCharacterController m_Controller;
        private Vector2 m_Input = Vector2.zero;
        private Vector3 m_Motor = Vector3.zero;

        protected override void Initialize()
        {
            m_RaycastHits = new RaycastHit[m_MaxCollisions];
            m_OverlappedColliders = new Collider[m_MaxCollisions];

            // TODO: Create custom inspector that adds through dropdown rather than manual string type names.
            // TODO: Create custom inspector to drag and order the priority of each behaviour.
            m_CharacterBehaviours = FindCharacterBehaviours();

            m_Rigidbody = GetComponent<Rigidbody>();
            SetupRigidbody();

            m_Collider = GetComponentInChildren<CapsuleCollider>();
            m_MoveDirection = Vector3.zero;
            m_GravityDirection = -transform.up;

            m_Controller = GetComponent<IECCCharacterController>();

            // TODO: Does not work in Runtime build. Fix.
            for (int i = 0; i < m_CharacterBehaviours.Count; i++)
                m_CharacterBehaviours[i].Initialize();
        }

        /// <summary>
        /// Finds all character behaviours, filters based on name array, and creates an instance of remaining results.
        /// </summary>
        private List<ECCBaseCharacterBehaviour> FindCharacterBehaviours()
        {
            List<ECCBaseCharacterBehaviour> behaviours = CreateAllSubClassesOf<ECCBaseCharacterBehaviour>(this);
            List<ECCBaseCharacterBehaviour> toRemove = new List<ECCBaseCharacterBehaviour>();
            for (int i = 0; i < behaviours.Count; i++)
            {
                string behaviourName = behaviours[i].GetType().Name;
                if (!m_CharacterBehaviourNames.Contains(behaviourName))
                    toRemove.Add(behaviours[i]);
            }

            behaviours.RemoveAll(x => toRemove.Contains(x));

            // Sort behaviors based on order of names.
            // TODO: Temporary. Create actual "Queue" class that will handle this in a better way.
            List<ECCBaseCharacterBehaviour> orderedBehaviours = new List<ECCBaseCharacterBehaviour>();
            for (int i = 0; i < m_CharacterBehaviourNames.Count; i++)
            {
                ECCBaseCharacterBehaviour behaviour = behaviours.Find(x => x.GetType().Name == m_CharacterBehaviourNames[i]);
                orderedBehaviours.Add(behaviour);
            }

            return orderedBehaviours;
        }

        /// <summary>
        /// Finds all sub-classes of a type, creates an instance of each, and returns a list of new objects.
        /// </summary>
        /// <param name="args">Arguments to pass to contructor.</param>
        /// <typeparam name="T">Base type that all sub-classes must inherit.</typeparam>
        /// <returns>List of type T with newly created objects.</returns>
        private List<T> CreateAllSubClassesOf<T>(params object[] args)
        {
            List<T> objects = new List<T>();
            System.Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || !types[i].IsSubclassOf(typeof(T)) || types[i].IsAbstract) continue;
                objects.Add((T)System.Activator.CreateInstance(types[i], args));
            }

            return objects;
        }

        /// <summary>
        /// Ensures that the Rigidbody has all proper settings applied.
        /// </summary>
        private void SetupRigidbody()
        {
            m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.constraints = RigidbodyConstraints.None;
        }

        private void Update()
        {
            // TODO: Input should not be handled in this class.
            m_Input = m_Controller != null ? m_Controller.GetInput() : Vector2.zero;
            Vector2 input = m_Input.normalized * Mathf.Max(Mathf.Abs(m_Input.x), Mathf.Abs(m_Input.y)) * Time.fixedDeltaTime;
            m_Motor = transform.TransformDirection(input.x, 0.0f, input.y);
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177798
        private void FixedUpdate()
        {
            // TODO: Move elsewhere when ready.
            Vector3 eulerRot = m_Rigidbody.rotation.eulerAngles;
            eulerRot.y = Camera.main.transform.eulerAngles.y;
            m_Rigidbody.rotation = Quaternion.Euler(eulerRot);

            // Set gravity direction every frame to account for rotation changes.
            m_GravityDirection = -transform.up;

            // Apply motor and gravity forces.
            m_GravityFactor = 0.0f;
            m_MoveDirection += m_Motor + (m_UseGravity ? (m_GravityDirection * (m_Gravity * m_GravityFactor) * Time.fixedDeltaTime) : Vector3.zero);

            // // Adjusts the move direction to smoothly move over any vertical or horizontal surface.
            // SmoothMoveDirection();

            // Detect collisions and make adjustments to the move direction as needed.
            DetectHorizontalCollisions();
            // DetectVerticalCollisions();

            #if UNITY_EDITOR
            // Draw final movement direction.
            Debug.DrawRay(transform.position, m_MoveDirection.normalized, Color.magenta);
            #endif

            // Move character after all calculations are completed.
            // Make sure move direction is multiplied by delta time as the direction vector is too large for per-frame movement.
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_MoveDirection);
            m_MoveDirection = Vector3.zero;
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177803
        private void SmoothMoveDirection()
        {
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, -m_GravityDirection);
            if (!IsMovingHorizontal(horizontalMoveDirection)) return;

            #if UNITY_EDITOR
            // Draw horizontal direction.
            Debug.DrawRay(transform.position, horizontalMoveDirection.normalized, Color.green);
            #endif

            // Cast in downward position.
            int hitCount = NonAllocCapsuleCast(
                transform.up * COLLIDER_OFFSET,
                m_GravityDirection * m_SkinWidth,
                ref m_RaycastHits
            );

            #if UNITY_EDITOR
            // Draw bottom of capsule cast ray.
            Debug.DrawRay(
                (m_Collider.transform.position + (transform.up * COLLIDER_OFFSET)) + (m_GravityDirection * (m_Collider.height / 2.0f)),
                m_GravityDirection * m_SkinWidth,
                hitCount > 0 ? Color.red : Color.green
            );
            #endif

            if (hitCount > 0)
            {
                RaycastHit hit = GetClosestRaycastHitRecursive(hitCount, m_RaycastHits);

                m_MoveDirection += CreateSlopeDirection(horizontalMoveDirection, hit.normal);
            }

            ClearRaycasts();
        }

        private bool IsMovingHorizontal(Vector3 horizontalMoveDirection)
        {
            return horizontalMoveDirection.magnitude >= 0.001f;
        }

        // Creates a direction based a sloped surface. If no slope is detected, returns provided direction.
        // Only works for ground surfaces.
        private Vector3 CreateSlopeDirection(Vector3 horizontalMoveDirection, Vector3 hitNormal)
        {
            if (!IsWalkableNormal(hitNormal)) return Vector3.zero;

            // Creates an up direction normal based on the hit normal and the right direction.
            // Using the right direction affects the upNormal by rotation, which is useful for the forward direction below.
            Vector3 upNormal = Vector3.ProjectOnPlane(hitNormal, transform.right).normalized;

            // Invert because the default value is a backwards direction.
            // Direction is created by crossing an up direction (hitNormal) and a right direction to get a forward direction.
            Vector3 forwardDirection = -Vector3.Cross(
                hitNormal,
                Vector3.Cross(upNormal, horizontalMoveDirection) // Creates a right direction based on rotation and horizontal move direction.
            ).normalized * horizontalMoveDirection.magnitude;

            return forwardDirection - horizontalMoveDirection;
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177805
        private void DetectHorizontalCollisions()
        {
            float horizontalOffset = 0.05f;
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, transform.up);
            Vector3 targetDirection = Vector3.zero;

            Vector3 offset = horizontalMoveDirection.normalized * horizontalOffset;
            int overlapCount = NonAllocCapsuleOverlap(
                offset,
                ref m_OverlappedColliders
            );
            for (int i = 0; i < overlapCount; i++)
            {
                bool overlapped = IsOverlapping(
                    m_OverlappedColliders[i],
                    targetDirection,
                    out Vector3 direction,
                    out float distance
                );
                if (overlapped)
                {
                    targetDirection += direction;
                }
            }

            targetDirection = Vector3.ProjectOnPlane(targetDirection, transform.up);

            // Perform cast in horizontal direction plus corrected overlap direction.
            Vector3 castDir = horizontalMoveDirection + targetDirection;
            Vector3 castOffset = castDir.normalized * horizontalOffset;
            int hitCount = NonAllocCapsuleCast(
                castOffset,
                castDir,
                ref m_RaycastHits
            );
            if (hitCount > 0)
            {
                RaycastHit closestRaycastHit = GetClosestRaycastHitRecursive(hitCount, m_RaycastHits);

                if (closestRaycastHit.distance == 0)
                {
                    overlapCount = NonAllocCapsuleOverlap(
                        castOffset,
                        ref m_OverlappedColliders
                    );
                    for (int i = 0; i < overlapCount; i++)
                    {
                        bool overlapped = IsOverlapping(
                            m_OverlappedColliders[i],
                            targetDirection,
                            out Vector3 direction,
                            out float distance
                        );
                        if (overlapped)
                        {
                            targetDirection += direction;
                        }
                    }

                    targetDirection -= horizontalMoveDirection;
                    targetDirection = Vector3.ProjectOnPlane(targetDirection, transform.up);

                    // float radiusMultiplier = ECCColliderHelper.GetCapsuleRadiusScale(m_Collider);
                    // float radius = (m_Collider.radius * radiusMultiplier) + COLLIDER_OFFSET;
                    // ECCColliderHelper.CalculateCapsuleCaps(
                    //     m_Collider,
                    //     m_Collider.transform.position + targetDirection,
                    //     m_Collider.transform.rotation,
                    //     out Vector3 capStart,
                    //     out Vector3 capEnd
                    // );

                    // RaycastHit hit;
                    // bool wasHit = Physics.CapsuleCast(
                    //     capStart,
                    //     capEnd,
                    //     radius,
                    //     horizontalMoveDirection.normalized,
                    //     out hit,
                    //     horizontalMoveDirection.magnitude,
                    //     ~m_CharacterLayer.value
                    // );
                    // if (wasHit)
                    // {
                    //     // Vector3 horizontalNormal = Vector3.ProjectOnPlane(closestRaycastHit.normal, transform.up);
                    //     // Vector3 wallDirection = Vector3.ProjectOnPlane(targetDirection, horizontalNormal);

                    //     // // Create direction to slide against surfaces.
                    //     // targetDirection = targetDirection + (targetDirection - (wallDirection.normalized * targetDirection.magnitude));
                    // }
                }
                else
                {
                    Vector3 horizontalNormal = Vector3.ProjectOnPlane(closestRaycastHit.normal, transform.up);
                    Vector3 wallDirection = Vector3.ProjectOnPlane(horizontalMoveDirection, horizontalNormal);

                    // Create direction to slide against surfaces.
                    targetDirection += wallDirection.normalized * horizontalMoveDirection.magnitude - horizontalMoveDirection;
                }
            }

            ClearRaycasts();

            m_MoveDirection += targetDirection;
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177807
        private void DetectVerticalCollisions()
        {
            float originalMoveDistance = m_MoveDirection.magnitude;

            // Get the horizontal move direction while ignoring the rotation of the character.
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, -m_GravityDirection);

            // Cast in downward position.
            int hitCount = NonAllocCapsuleCast(
                transform.up * COLLIDER_OFFSET,
                m_GravityDirection * m_SkinWidth,
                ref m_RaycastHits
            );

            if (hitCount > 0)
            {
                RaycastHit hit = GetClosestRaycastHitRecursive(hitCount, m_RaycastHits);

                Vector3 hitPoint = hit.point + (hit.normal * COLLIDER_OFFSET);
                Vector3 closestPoint = m_Collider.ClosestPoint(hitPoint);

                // Get Y difference of closest collider point and raycast hit, then apply a small extra offset.
                float offset = (hitPoint - closestPoint).y;
                if (Mathf.Abs(offset) <= 0.001f) // Account for floating point error.
                {
                    offset = 0.0f;
                }

                // Adjust move direction and if vertical offset is too low, use negative hit distance to prevent ground clipping.
                m_MoveDirection.y += offset;
                if (m_MoveDirection.y < -hit.distance)
                {
                    m_MoveDirection.y = -hit.distance;
                }
            }

            ClearRaycasts();

            // Fix any collision overlaps.
            hitCount = NonAllocCapsuleCast(
                transform.up * COLLIDER_OFFSET,
                m_GravityDirection * m_SkinWidth,
                ref m_RaycastHits
            );

            if (hitCount > 0)
            {
                RaycastHit hit = GetClosestRaycastHitRecursive(hitCount, m_RaycastHits);

                if (!IsWalkableNormal(hit.normal)) return;

                bool overlapped = IsOverlapping(hit.collider, m_MoveDirection, out Vector3 offset, out float distance);
                if (overlapped)
                {
                    m_MoveDirection += offset.normalized * (distance + COLLIDER_OFFSET);
                    // m_MoveDirection.y += offset.normalized.y * (distance + COLLIDER_OFFSET);
                }
            }

            // Multiply by original move direction magnitude to prevent character from moving up surfaces too quickly.
            m_MoveDirection = m_MoveDirection.normalized * originalMoveDistance;

            ClearRaycasts();
        }

        private bool IsWalkableNormal(Vector3 normal)
        {
            // Calculate angle of slope based on normal.
            float angle = Vector3.Angle(normal, transform.up);
            return angle < m_MaxSlopeAngle;
        }

        private bool IsOverlapping(Collider collider, Vector3 offset, out Vector3 direction, out float distance)
        {
            float capsuleRadius = m_Collider.radius;
            float radiusMultiplier = ECCColliderHelper.GetCapsuleRadiusScale(m_Collider);
            float overlapRadius = (capsuleRadius * radiusMultiplier) + COLLIDER_OFFSET;

            // Set radius of capsule to account for scale and small offset.
            m_Collider.radius = overlapRadius;
            bool overlapped = Physics.ComputePenetration(
                m_Collider, m_Collider.transform.position + offset, m_Collider.transform.rotation,
                collider, collider.transform.position, collider.transform.rotation,
                out direction, out distance
            );
            if (overlapped)
            {
                direction = direction.normalized * (distance + COLLIDER_OFFSET);
            }

            // Reset radius.
            m_Collider.radius = capsuleRadius;

            return overlapped;
        }

        private int NonAllocCapsuleCast(Vector3 offset, Vector3 direction, ref RaycastHit[] hits)
        {
            float radiusMultiplier = ECCColliderHelper.GetCapsuleRadiusScale(m_Collider);
            float radius = (m_Collider.radius * radiusMultiplier) + COLLIDER_OFFSET;
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                m_Collider.transform.position + offset,
                m_Collider.transform.rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );

            return Physics.CapsuleCastNonAlloc(capStart, capEnd, radius, direction.normalized, hits, direction.magnitude, ~m_CharacterLayer.value);
        }

        private int NonAllocCapsuleOverlap(Vector3 offset, ref Collider[] colliders)
        {
            float radiusMultiplier = ECCColliderHelper.GetCapsuleRadiusScale(m_Collider);
            float radius = (m_Collider.radius * radiusMultiplier) + COLLIDER_OFFSET;
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                m_Collider.transform.position + offset,
                m_Collider.transform.rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );

            return Physics.OverlapCapsuleNonAlloc(capStart, capEnd, radius, colliders, ~m_CharacterLayer.value);
        }

        private void ClearRaycasts()
        {
            for (int i = 0; i < m_RaycastHits.Length; i++)
            {
                m_RaycastHits[i] = default(RaycastHit);
            }
        }

        // TODO: Placeholder for now. Eventually expand this method to require the index field and grab the closest point based on that.
        private RaycastHit GetClosestRaycastHitRecursive(int hitCount, RaycastHit[] hits, int index = 0, RaycastHit closestHit = default(RaycastHit))
        {
            // Get next index, current hit, and the next hit.
            int nextIndex = index + 1;
            RaycastHit currentHit = hits[index];
            RaycastHit nextHit = nextIndex < hitCount ? hits[nextIndex] : currentHit;

            // Set closest hit to current hit if it hasn't been assigned anything.
            closestHit = closestHit.Equals(default(RaycastHit)) ? currentHit : closestHit;

            // If the next hit is closer, then it is the new closest hit.
            if (nextHit.distance + COLLIDER_OFFSET < closestHit.distance + COLLIDER_OFFSET)
            {
                closestHit = nextHit;
            }

            return nextIndex < hitCount ? GetClosestRaycastHitRecursive(hitCount, hits, nextIndex, closestHit) : closestHit;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color originalGizmosColor = Gizmos.color;

            // Draw custom gizmos...

            Gizmos.color = originalGizmosColor;
        }
        #endif
    }
}
