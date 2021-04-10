using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UPhysics = UnityEngine.Physics;

using RotaryHeart.Lib.PhysicsExtension;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

using ExtensibleCharacterController.Core.Utility;
using ExtensibleCharacterController.Characters.Behaviours;

namespace ExtensibleCharacterController.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class ECCCharacter : ECCBehaviour
    {
        private const float COLLIDER_OFFSET = 0.01f;

        [Header("Generic Settings")]
        [Tooltip("Enable/disable all character functionality, including movement, collisions, gravity, and character behaviours.")]
        [SerializeField]
        private bool m_MotorEnabled = true;
        [Tooltip(
            "Layers that collision tests will ignore." + " " +
            "Always make sure to create a specific layer for the character, assign it to this game object, and select that layer here." + " " +
            "If this is not done, the character could theoretically collide with itself. To be cautious, always use a character-specific layer."
        )]
        [SerializeField]
        private LayerMask m_CollisionIgnoreLayer;
        [Tooltip("Enable/disable gravity for the character.")]
        [SerializeField]
        private bool m_UseGravity = true;
        [Tooltip("Amount of force that is applied to the gravity direction.")]
        [SerializeField]
        private float m_Gravity = -UPhysics.gravity.y;
        [Tooltip("Maximum amount of collisions that character can detect per frame.")]
        [SerializeField]
        private int m_MaxCollisions = 10;

        [Header("Horizontal Collision Settings")]
        [Tooltip(
            "Distance that is used during horizontal collision detection to prevent the character from clipping through surfaces." + " " +
            "The higher the value, the farther away the character will be from collision points, and vice-versa." + " " +
            "The default value should be fine in most scenarios."
        )]
        [SerializeField]
        private float m_HorizontalSkinWidth = 0.1f;
        [Tooltip(
            "The minimum angle that the character must be, relative to a horizontal surface, before surface sliding begins." + " " +
            "If the angle is 0, the character will slide against every surface. If the angle is 90, the character will never slide against a surface."
        )]
        [SerializeField]
        private float m_HorizontalSurfaceSlideAngleMinimum = 15.0f;
        [Tooltip("Friction multiplier applied to surface sliding.")]
        [SerializeField]
        private float m_HorizontalFrictionFactor = 1.0f;

        [Header("Vertical Collision Settings")]
        [Tooltip(
            "Distance that is used during vertical collision detection to detect the ground surface." + " " +
            "The higher the value, the bigger the gap between the character and ground is required before considered grounded, and vice-versa." + " " +
            "The default value should be fine in most scenarios."
        )]
        [SerializeField]
        private float m_SkinWidth = 0.1f;
        [Tooltip("The maximum height that the character can step over.")]
        [SerializeField]
        private float m_MaxStep = 0.3f;
        [Tooltip("The maximum slope angle that the character can move over.")]
        [SerializeField]
        private float m_MaxSlopeAngle = 60.0f;

        [Header("Debug Settings")]
        [Tooltip("Change the global time scale. Useful for slowly visualizing changes to a fast character.")]
        [SerializeField]
        private float m_TimeScale = 1.0f;
        [Tooltip("Visually indicates the primary horizontal collision test. Drawn in the Editor and at Runtime.")]
        [SerializeField]
        private bool m_DebugHorizontalCollisionCast = false;
        [Tooltip("Visually indicates the horizontal collision test that happens when sliding on a surface. Drawn in the Editor and at Runtime.")]
        [SerializeField]
        private bool m_DebugHorizontalWallCast = false;

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
        private RaycastHit m_SingleRaycastHit;
        private Collider[] m_OverlappedColliders;

        private Vector3 m_MoveDirection = Vector3.zero;
        private Vector3 m_GravityDirection = -Vector3.up;
        private float m_GravityFactor = 1.0f;
        private bool m_IsGrounded = false;
        private IECCCharacterController m_Controller;
        private Vector2 m_Input = Vector2.zero;
        private Vector3 m_Motor = Vector3.zero;
        private Vector3 m_DeltaVelocity = Vector3.zero;
        private Vector3 m_HorizontalDeltaVelocity = Vector3.zero;
        private Vector3 m_VerticalDeltaVelocity = Vector3.zero;

        protected override void Initialize()
        {
            m_RaycastHits = new RaycastHit[m_MaxCollisions];
            m_OverlappedColliders = new Collider[m_MaxCollisions];

            // // TODO: Create custom inspector that adds through dropdown rather than manual string type names.
            // // TODO: Create custom inspector to drag and order the priority of each behaviour.
            // m_CharacterBehaviours = FindCharacterBehaviours();

            m_Rigidbody = GetComponent<Rigidbody>();
            SetupRigidbody();

            m_Collider = GetComponentInChildren<CapsuleCollider>();
            m_MoveDirection = Vector3.zero;
            m_GravityDirection = -transform.up;

            m_Controller = GetComponent<IECCCharacterController>();

            // // TODO: Does not work in Runtime build. Fix.
            // for (int i = 0; i < m_CharacterBehaviours.Count; i++)
            //     m_CharacterBehaviours[i].Initialize();
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
            Time.timeScale = m_TimeScale;

            // TODO: Input should not be handled in this class.
            m_Input = m_Controller != null ? m_Controller.GetInput() : Vector2.zero;
            Vector2 input = m_Input.normalized * Mathf.Max(Mathf.Abs(m_Input.x), Mathf.Abs(m_Input.y));
            m_Motor = transform.TransformDirection(input.x, 0.0f, input.y);
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177798
        private void FixedUpdate()
        {
            // TODO: Move elsewhere when ready.
            Vector3 eulerRot = transform.rotation.eulerAngles;
            eulerRot.y = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(eulerRot);

            // Set gravity direction every frame to account for rotation changes.
            m_GravityDirection = -transform.up;

            // Apply motor and gravity forces.
            m_GravityFactor = 0.0f;
            m_MoveDirection += (m_Motor + (m_UseGravity ? (m_GravityDirection * (m_Gravity * m_GravityFactor)) : Vector3.zero)) * Time.fixedDeltaTime;

            // // Adjusts the move direction to smoothly move over any vertical or horizontal surface.
            // SmoothMoveDirection();

            // Detect collisions and make adjustments to the move direction as needed.
            DetectHorizontalCollisions();
            // DetectVerticalCollisions();

            #if UNITY_EDITOR
            // Draw final movement direction.
            Debug.DrawRay(transform.position + m_DeltaVelocity, m_MoveDirection, Color.magenta);
            #endif

            // Move character after all calculations are completed.
            // Make sure move direction is multiplied by delta time as the direction vector is too large for per-frame movement.
            if (m_MotorEnabled)
            {
                transform.position += m_MoveDirection;
                // m_Rigidbody.MovePosition(m_Rigidbody.position + m_MoveDirection);
            }

            // m_DeltaVelocity = m_Rigidbody.velocity * Time.fixedDeltaTime;
            // m_HorizontalDeltaVelocity = Vector3.ProjectOnPlane(m_DeltaVelocity, transform.up);
            // m_VerticalDeltaVelocity = Vector3.ProjectOnPlane(
            //     Vector3.ProjectOnPlane(m_DeltaVelocity, transform.forward),
            //     transform.right
            // );
            m_MoveDirection = Vector3.zero;
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177803
        private void SmoothMoveDirection()
        {
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, -m_GravityDirection);
            if (!IsMovingHorizontal(horizontalMoveDirection)) return;

            #if UNITY_EDITOR
            // Draw horizontal direction.
            Debug.DrawRay(transform.position + m_DeltaVelocity, horizontalMoveDirection.normalized, Color.green);
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
                ((m_Collider.transform.position + m_DeltaVelocity) + (transform.up * COLLIDER_OFFSET)) + (m_GravityDirection * (m_Collider.height / 2.0f)),
                m_GravityDirection * m_SkinWidth,
                hitCount > 0 ? Color.red : Color.green
            );
            #endif

            if (hitCount > 0)
            {
                RaycastHit hit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                    m_Collider,
                    hitCount,
                    m_RaycastHits,
                    m_DeltaVelocity,
                    COLLIDER_OFFSET
                );

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
            // float horizontalOffset = 0.05f;
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, transform.up);
            Vector3 targetDirection = horizontalMoveDirection;

            Vector3 normalizedHorizontalDirection = horizontalMoveDirection.normalized;
            float horizontalDirectionMagnitude = horizontalMoveDirection.magnitude;
            if (horizontalDirectionMagnitude <= 0.001f)
            {
                m_MoveDirection -= horizontalMoveDirection;
                return;
            }

            // Perform capsule cast in horizontal direction.
            Vector3 horizontalOffset = -(normalizedHorizontalDirection / 2.0f) + m_HorizontalDeltaVelocity;
            int hitCount = NonAllocCapsuleCast(
                horizontalOffset,
                normalizedHorizontalDirection,
                ref m_RaycastHits,
                m_DebugHorizontalCollisionCast
            );

            if (hitCount > 0)
            {
                RaycastHit horizontalHit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                    m_Collider,
                    hitCount,
                    m_RaycastHits,
                    m_HorizontalDeltaVelocity,
                    COLLIDER_OFFSET
                );
                Vector3 hitPoint = horizontalHit.point;
                Vector3 hitNormal = horizontalHit.normal;
                Vector3 colliderPoint = ECCPhysicsHelper.GetClosestColliderPoint(m_Collider, m_HorizontalDeltaVelocity, hitPoint);

                float colliderDistance = (hitPoint - colliderPoint).magnitude - COLLIDER_OFFSET;
                if (colliderDistance <= m_HorizontalSkinWidth)
                {
                    // // TODO: Test this to ensure it still works.
                    // // Check if character can step over.
                    // Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
                    // if (localHitPoint.y <= m_MaxStep + COLLIDER_OFFSET)
                    // {
                    //     // If character can step over, do not continue.
                    //     // Prevents horizontal collision correction from running while on a slope.
                    //     Physics.Raycast(
                    //         hitPoint - horizontalOffset,
                    //         horizontalMoveDirection,
                    //         out m_SingleRaycastHit,
                    //         m_HorizontalSkinWidth,
                    //         ~m_CollisionIgnoreLayer.value
                    //     );
                    //     float slopeAngle = Vector3.Angle(transform.up, m_SingleRaycastHit.normal);
                    //     if (slopeAngle <= m_MaxSlopeAngle + COLLIDER_OFFSET) return;
                    // }

                    // // Find actual distance from skin width. Then apply the difference. Allows character to get as close to surface as possible.
                    // Vector3 correctionDirection = Vector3.zero;
                    // float correctionDistance = m_HorizontalSkinWidth - distanceFromCollider;
                    // if (correctionDistance < m_HorizontalSkinWidth)
                    // {
                    //     correctionDirection +=
                    //         normalizedHorizontalDirection * horizontalDirectionMagnitude
                    //         * ((m_HorizontalSkinWidth - distanceFromCollider) * Time.fixedDeltaTime * horizontalDirectionMagnitude);
                    // }

                    // Create direction that allows character to slide off surfaces by projecting the horizontal direction onto the hit surface.
                    // Uses same magnitude as horizontal direction, so make sure to remove horizontal direction before applying target direction.
                    Vector3 horizontalNormal = Vector3.ProjectOnPlane(hitNormal, transform.up);
                    Vector3 surfaceDirection = Vector3.ProjectOnPlane(horizontalMoveDirection, horizontalNormal).normalized;

                    float surfaceAngle = 90.0f - Vector3.Angle(horizontalMoveDirection, surfaceDirection);
                    if (surfaceAngle >= m_HorizontalSurfaceSlideAngleMinimum)
                    {
                        // Calculate the strength of the surface direction based on the direction of the horizontal move direction.
                        // The Dot product of the normalized horizontal direction and inverse horizontal normal is perfect, as it uses cosine.
                        float surfaceDirectionStrength = 1.0f - Vector3.Dot(normalizedHorizontalDirection, -horizontalNormal);

                        // If the horizontal magnitude is greater than the radius of the collider, than the collider point distance will be used.
                        // This will prevent overlap. Collisions behave odd if an object moves more than its radius and has another collision next frame.
                        float surfaceDirectionDistance = Mathf.Min(horizontalHit.distance - COLLIDER_OFFSET, horizontalDirectionMagnitude);

                        // Calculate the surface move direction by multiplying the surface direction by all of our factor values.
                        targetDirection = surfaceDirection * (surfaceDirectionDistance * surfaceDirectionStrength) * m_HorizontalFrictionFactor;
                    }

                    ClearRaycasts();
                }
            }

            // Do another cast in the target direction to make sure direction is correct.
            // hitCount = NonAllocCapsuleCast(
            //     (normalizedHorizontalDirection + normalizedTargetDirection) * COLLIDER_OFFSET + m_HorizontalDeltaVelocity,
            //     (horizontalMoveDirection + targetDirection).normalized,
            //     ref m_RaycastHits,
            //     m_DebugHorizontalWallCast
            // );
            Vector3 normalizedTargetDirection = targetDirection.normalized;
            float targetDirectionMagnitude = targetDirection.magnitude;
            Vector3 normalizedSlideOffsetDirection = (horizontalMoveDirection + targetDirection).normalized;
            hitCount = NonAllocCapsuleCast(
                -(normalizedSlideOffsetDirection / 2.0f) + m_HorizontalDeltaVelocity,
                normalizedSlideOffsetDirection,
                ref m_RaycastHits,
                m_DebugHorizontalWallCast
            );

            if (hitCount > 0)
            {
                // horizontalHit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                //     m_Collider,
                //     hitCount,
                //     m_RaycastHits,
                //     m_HorizontalDeltaVelocity,
                //     COLLIDER_OFFSET
                // );
                // hitPoint = horizontalHit.point;
                // hitNormal = horizontalHit.normal;
                // colliderPoint = ECCPhysicsHelper.GetClosestColliderPoint(m_Collider, m_HorizontalDeltaVelocity, hitPoint);

                // distanceFromCollider = (hitPoint - colliderPoint).magnitude - COLLIDER_OFFSET;
                // if (distanceFromCollider < m_HorizontalSkinWidth)
                // {
                //     // // Check if character can step over.
                //     // localHitPoint = transform.InverseTransformPoint(hitPoint);
                //     // if (localHitPoint.y <= m_MaxStep + COLLIDER_OFFSET)
                //     // {
                //     //     // If character can step over, do not continue.
                //     //     // Prevents horizontal collision correction from running while on a slope.
                //     //     bool hit = Physics.Raycast(
                //     //         hitPoint - normalizedSlideOffsetDirection,
                //     //         normalizedTargetDirection,
                //     //         out m_SingleRaycastHit,
                //     //         m_HorizontalSkinWidth,
                //     //         ~m_CharacterLayer.value,
                //     //         PreviewCondition.Both,
                //     //         0.0f,
                //     //         Color.red,
                //     //         Color.green,
                //     //         true
                //     //     );
                //     //     if (hit)
                //     //     {
                //     //         Debug.Break();
                //     //     }
                //     //     // float slopeAngle = Vector3.Angle(transform.up, m_SingleRaycastHit.normal);
                //     //     // if (slopeAngle <= m_MaxSlopeAngle + COLLIDER_OFFSET) return;
                //     // }

                //     // // Find actual distance from skin width. Then apply the difference. Allows character to get as close to surface as possible.
                //     // correctionDistance = m_HorizontalSkinWidth - distanceFromCollider;
                //     // if (correctionDistance < m_HorizontalSkinWidth)
                //     // {
                //     //     targetDirection +=
                //     //         normalizedTargetDirection * targetDirectionMagnitude
                //     //         * ((m_HorizontalSkinWidth - distanceFromCollider) * Time.fixedDeltaTime * targetDirectionMagnitude);
                //     // }

                //     // // Create direction that allows character to slide off surfaces.
                //     // // Uses same magnitude as horizontal direction, so make sure to remove horizontal direction before applying target direction.
                //     // horizontalNormal = Vector3.ProjectOnPlane(hitNormal, transform.up);
                //     // surfaceDirection = Vector3.ProjectOnPlane(targetDirection, horizontalNormal);

                //     // // Apply surface direction, corrected distance direction, and then remove original horizontal direction.
                //     // targetDirection +=
                //     //     surfaceDirection.normalized * targetDirectionMagnitude * m_HorizontalSlideFrictionFactor
                //     //     - (targetDirection);

                //     // // If something was hit and not stepped over, it is a real collision.
                //     // // Subtract all horizontal directions from target direction to stop movement.
                //     // targetDirection -= targetDirection + horizontalMoveDirection;
                // }
            }

            m_MoveDirection = targetDirection;
        }

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177807
        private void DetectVerticalCollisions()
        {
            float originalMoveDistance = m_MoveDirection.magnitude;
            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, -m_GravityDirection);
            Vector3 targetDirection = Vector3.zero;

            // Cast in downward position.
            int hitCount = NonAllocCapsuleCast(
                (horizontalMoveDirection.normalized * COLLIDER_OFFSET) + (transform.up * COLLIDER_OFFSET),
                m_GravityDirection * m_SkinWidth,
                ref m_RaycastHits
            );
            if (hitCount > 0)
            {
                m_IsGrounded = true;
                m_GravityFactor = 0.0f;

                RaycastHit hit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                    m_Collider,
                    hitCount,
                    m_RaycastHits,
                    m_VerticalDeltaVelocity,
                    COLLIDER_OFFSET
                );

                // If slope is too high, do not continue.
                float slopeAngle = Vector3.Angle(transform.up, hit.normal);
                if (slopeAngle >= m_MaxSlopeAngle + COLLIDER_OFFSET) return;

                // Vector3 hitPoint = hit.point + (hit.normal * COLLIDER_OFFSET);
                Vector3 hitPoint = hit.point + (hit.normal * COLLIDER_OFFSET);
                Vector3 closestPoint = m_Collider.ClosestPoint(hitPoint);

                // Get Y difference of hit point and closest collider point.
                float offset = (hitPoint - closestPoint).y + COLLIDER_OFFSET;
                if (Mathf.Abs(offset) <= 0.001f) // Account for floating point error.
                {
                    offset = 0.0f;
                }

                // Don't move down further than the inverse hit distance.
                targetDirection.y += offset;
                if (targetDirection.y < -hit.distance + COLLIDER_OFFSET)
                {
                    targetDirection.y = -hit.distance + COLLIDER_OFFSET;
                }

                // Check if character can step over.
                Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
                if (localHitPoint.y <= m_MaxStep + COLLIDER_OFFSET)
                {
                    // If character can step over, do not continue.
                    // Prevents horizontal collision correction from running while on a slope.
                    Physics.Raycast(
                        hitPoint - (horizontalMoveDirection.normalized * COLLIDER_OFFSET),
                        horizontalMoveDirection.normalized,
                        out m_SingleRaycastHit,
                        m_HorizontalSkinWidth,
                        ~m_CollisionIgnoreLayer.value
                    );
                    float angle = Vector3.Angle(transform.up, m_SingleRaycastHit.normal);
                    if (slopeAngle <= m_MaxSlopeAngle + COLLIDER_OFFSET)
                    {
                        targetDirection.y = 0.0f;
                    }
                }

                bool overlapped = IsOverlapping(
                    hit.collider,
                    (horizontalMoveDirection.normalized * COLLIDER_OFFSET) + (transform.up * COLLIDER_OFFSET),
                    out Vector3 dir,
                    out float dis
                );
                if (overlapped)
                {
                    targetDirection.y += dir.y;
                }
            }
            else
            {
                m_IsGrounded = false;
                m_GravityFactor = 1.0f;
            }

            ClearRaycasts();

            m_MoveDirection += targetDirection;
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
            float radiusMultiplier = ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider);
            float overlapRadius = (capsuleRadius * radiusMultiplier) + COLLIDER_OFFSET;

            // Set radius of capsule to account for scale and small offset.
            m_Collider.radius = overlapRadius;
            bool overlapped = UPhysics.ComputePenetration(
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
        private int NonAllocCapsuleCast(Vector3 offset, Vector3 direction, ref RaycastHit[] hits, bool debugDraw = false)
        {
            float radiusMultiplier = ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider);
            float radius = (m_Collider.radius * radiusMultiplier) + COLLIDER_OFFSET;
            ECCPhysicsHelper.CalculateCapsuleCaps(
                m_Collider,
                m_Collider.transform.position + offset,
                m_Collider.transform.rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );

            return Physics.CapsuleCastNonAlloc(
                capStart,
                capEnd,
                radius,
                direction.normalized,
                hits,
                direction.magnitude,
                ~m_CollisionIgnoreLayer.value,
                debugDraw ? PreviewCondition.Both : PreviewCondition.None,
                0.0f,
                Color.red,
                Color.green
            );
        }

        private int NonAllocCapsuleOverlap(Vector3 offset, ref Collider[] colliders, bool debugDraw = false)
        {
            float radiusMultiplier = ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider);
            float radius = (m_Collider.radius * radiusMultiplier) + COLLIDER_OFFSET;
            ECCPhysicsHelper.CalculateCapsuleCaps(
                m_Collider,
                m_Collider.transform.position + offset,
                m_Collider.transform.rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );

            return Physics.OverlapCapsuleNonAlloc(
                capStart,
                capEnd,
                radius,
                colliders,
                ~m_CollisionIgnoreLayer.value,
                debugDraw ? PreviewCondition.Both : PreviewCondition.None,
                0.0f,
                Color.red,
                Color.green
            );
        }

        private void ClearRaycasts()
        {
            for (int i = 0; i < m_RaycastHits.Length; i++)
            {
                m_RaycastHits[i] = default(RaycastHit);
            }
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
