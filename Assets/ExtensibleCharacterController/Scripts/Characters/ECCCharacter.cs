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
        // Small offset applied to Physics casting and collider position checking. Helps to prevent overlap.
        private const float COLLIDER_OFFSET = 0.01f;

        // Small offset that is used in a raycast when checking if character should step over an object.
        private const float STEP_CHECK_OFFSET = 0.1f;

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
        [Tooltip("Maximum number of collisions that character can detect, per frame.")]
        [SerializeField]
        private int m_MaxCollisions = 30;
        [Tooltip("Maximum number of overlapped colliders that character will seperate from, per frame.")]
        [SerializeField]
        private int m_MaxOverlapCorrections = 10;
        [Tooltip("Enable/disable functionality to automatically ensure that the character will seperate itself from overlapping collisions.")]
        [SerializeField]
        private bool m_FixCollisionOverlaps = true;

        [Header("Horizontal Collision Settings")]
        [Tooltip("Enable/disable horizontal collision detection.")]
        [SerializeField]
        private bool m_HorizontalCollisionsEnabled = true;
        [Tooltip("Friction multiplier applied to surface sliding.")]
        [SerializeField]
        private float m_HorizontalFrictionFactor = 1.0f;

        [Header("Vertical Collision Settings")]
        [Tooltip("Enable/disable vertical collision detection.")]
        [SerializeField]
        private bool m_VerticalCollisionsEnabled = true;
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
        [Tooltip(
            "The amount of leeway that is allowed when checking slope angles." + " " +
            "This means the angle of sloped objects does not have be 100% exactly equal to the max slope angle." + " " +
            "For example, if the maximum slope angle is 60 and the slope leeway angle is 0.5," + " " +
            "then a sloped object with an angle of 60.5 and under will be walkable."
        )]
        [SerializeField]
        private float m_SlopeLeewayAngle = 0.5f;

        [Header("Debug Settings")]
        [Tooltip("Change the global time scale. Useful for slowly visualizing changes to a fast character.")]
        [SerializeField]
        private float m_TimeScale = 1.0f;
        [Tooltip("Visually indicates the primary horizontal collision test.")]
        [SerializeField]
        private bool m_DebugHorizontalCollisionCast = false;
        [Tooltip("Visually indicates the horizontal collision test that happens when sliding on a surface.")]
        [SerializeField]
        private bool m_DebugHorizontalWallCast = false;
        [Tooltip("Draws the casts/overlap checks that occur when checking for steps/slopes during horizontal collision detection.")]
        [SerializeField]
        private bool m_DebugHorizontalStepCast = false;
        [Tooltip("Draws the move direction at the character's position.")]
        [SerializeField]
        private bool m_DebugMoveDirection = false;
        [Tooltip("Draws the normalized move direction at the character's position.")]
        [SerializeField]
        private bool m_DebugNormalizedMoveDirection = false;

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
        private Collider[] m_OverlappingColliders;

        private Vector3 m_MoveDirection = Vector3.zero;
        private Vector3 m_GravityDirection = -Vector3.up;
        private float m_GravityFactor = 1.0f;
        private bool m_IsGrounded = false;
        private IECCCharacterController m_Controller;
        private Vector2 m_Input = Vector2.zero;
        private Vector3 m_Motor = Vector3.zero;

        private bool m_CanValidateHorizontalDirection = true;
        private bool m_CanValidateVerticalDirection = true;

        protected override void Initialize()
        {
            m_RaycastHits = new RaycastHit[m_MaxCollisions];
            m_OverlappingColliders = new Collider[m_MaxCollisions];

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
            m_Rigidbody.interpolation = RigidbodyInterpolation.None;
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

            // Perform one last check to make sure character is not overlapping any colliders.
            ValidateMoveDirection();

            // Move character after all calculations are completed.
            // Make sure move direction is multiplied by delta time as the direction vector is too large for per-frame movement.
            if (m_MotorEnabled)
            {
                transform.position += m_MoveDirection;
            }

            #if UNITY_EDITOR
            // Draw final movement direction.
            if (m_DebugMoveDirection)
            {
                DebugExtension.DebugArrow(
                    m_Rigidbody.position,
                    m_DebugNormalizedMoveDirection ? m_MoveDirection.normalized : m_MoveDirection,
                    Color.magenta
                );
            }
            #endif

            m_MoveDirection = Vector3.zero;
        }

        #region Smooth Move Direction

        // // TODO: https://app.asana.com/0/1200147678177766/1200147678177803
        // private void SmoothMoveDirection()
        // {
        //     Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MoveDirection, -m_GravityDirection);
        //     if (!IsMovingHorizontal(horizontalMoveDirection)) return;

        //     #if UNITY_EDITOR
        //     // Draw horizontal direction.
        //     Debug.DrawRay(transform.position, horizontalMoveDirection.normalized, Color.green);
        //     #endif

        //     // Cast in downward position.
        //     int hitCount = NonAllocCapsuleCast(
        //         transform.up * COLLIDER_OFFSET,
        //         m_GravityDirection * m_SkinWidth,
        //         ref m_RaycastHits
        //     );

        //     #if UNITY_EDITOR
        //     // Draw bottom of capsule cast ray.
        //     Debug.DrawRay(
        //         ((m_Collider.transform.position) + (transform.up * COLLIDER_OFFSET)) + (m_GravityDirection * (m_Collider.height / 2.0f)),
        //         m_GravityDirection * m_SkinWidth,
        //         hitCount > 0 ? Color.red : Color.green
        //     );
        //     #endif

        //     if (hitCount > 0)
        //     {
        //         RaycastHit hit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
        //             m_Collider,
        //             hitCount,
        //             m_RaycastHits,
        //             transform.up * COLLIDER_OFFSET,
        //             COLLIDER_OFFSET
        //         );

        //         m_MoveDirection += CreateSlopeDirection(horizontalMoveDirection, hit.normal);
        //     }

        //     ClearRaycasts();
        // }

        // private bool IsMovingHorizontal(Vector3 horizontalMoveDirection)
        // {
        //     return horizontalMoveDirection.magnitude >= 0.001f;
        // }

        // // Creates a direction based a sloped surface. If no slope is detected, returns provided direction.
        // // Only works for ground surfaces.
        // private Vector3 CreateSlopeDirection(Vector3 horizontalMoveDirection, Vector3 hitNormal)
        // {
        //     if (!IsValidSlopeAngle(hitNormal)) return Vector3.zero;

        //     // Creates an up direction normal based on the hit normal and the right direction.
        //     // Using the right direction affects the upNormal by rotation, which is useful for the forward direction below.
        //     Vector3 upNormal = Vector3.ProjectOnPlane(hitNormal, transform.right).normalized;

        //     // Invert because the default value is a backwards direction.
        //     // Direction is created by crossing an up direction (hitNormal) and a right direction to get a forward direction.
        //     Vector3 forwardDirection = -Vector3.Cross(
        //         hitNormal,
        //         Vector3.Cross(upNormal, horizontalMoveDirection) // Creates a right direction based on rotation and horizontal move direction.
        //     ).normalized * horizontalMoveDirection.magnitude;

        //     return forwardDirection - horizontalMoveDirection;
        // }

        #endregion

        #region Horizontal Collision Detection

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177805
        private void DetectHorizontalCollisions()
        {
            if (!m_HorizontalCollisionsEnabled) return;

            // Turn move direction into horizontal direction only.
            Vector3 horizontalMoveDirection = ConvertToHorizontalDirection(m_MoveDirection);
            Vector3 targetDirection = horizontalMoveDirection;

            // If no movement is occuring, no need to check for collisions.
            Vector3 normalizedHorizontalDirection = horizontalMoveDirection.normalized;
            float horizontalDirectionMagnitude = horizontalMoveDirection.magnitude;
            if (horizontalDirectionMagnitude <= 0.001f)
            {
                m_MoveDirection -= horizontalMoveDirection;
                return;
            }

            // Store the capsule radius for future use below.
            float radius = m_Collider.radius * ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider);

            // Check for collisions in the horizontal direction.
            int hitCount = NonAllocCapsuleCast(
                horizontalMoveDirection,
                horizontalMoveDirection,
                ref m_RaycastHits,
                m_DebugHorizontalCollisionCast
            );

            // // TODO: Re-enable and improve GetClosestRaycastHitRecursive() to support loops and always return the actual nearest hit.
            // for (int i = 0; i < hitCount; i++)
            // {
            //     RaycastHit horizontalHit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
            //         m_Collider,
            //         hitCount,
            //         m_RaycastHits,
            //         horizontalMoveDirection,
            //         COLLIDER_OFFSET
            //     );
            //     Vector3 hitPoint = horizontalHit.point;

            //     // If the hit point can be stepped over, do not continue. Vertical collision detection will handle the actual step-up logic.
            //     bool canStepOver = CanStepOver(hitPoint, horizontalMoveDirection, normalizedHorizontalDirection, horizontalDirectionMagnitude);
            //     ClearOverlapColliders();

            //     if (canStepOver) continue;

            //     // Create direction that allows character to slide off surfaces by projecting the horizontal direction onto the hit surface.
            //     // Uses same magnitude as horizontal direction, so make sure to remove horizontal direction before applying target direction.
            //     Vector3 horizontalNormal = Vector3.ProjectOnPlane(horizontalHit.normal, transform.up);
            //     Vector3 surfaceDirection = Vector3.ProjectOnPlane(horizontalMoveDirection, horizontalNormal).normalized;

            //     // Calculate the strength of the surface direction based on the direction of the horizontal move direction.
            //     // The Dot product of the normalized horizontal direction and inverse horizontal normal is perfect, as it uses cosine.
            //     float surfaceDirectionStrength = 1.0f - Vector3.Dot(normalizedHorizontalDirection, -horizontalNormal);

            //     // If the horizontal magnitude is greater than the radius of the collider, than the collider distance will be used.
            //     // This will prevent overlap. Collisions behave odd if an object moves more than its radius and has another collision next frame.
            //     Vector3 colliderPoint = ECCPhysicsHelper.GetClosestColliderPoint(m_Collider, horizontalMoveDirection, hitPoint);
            //     float colliderDistance = (hitPoint - colliderPoint).magnitude - COLLIDER_OFFSET;
            //     float surfaceDirectionDistance = Mathf.Min(colliderDistance, horizontalDirectionMagnitude);

            //     // Calculate the target move direction by multiplying the surface direction by all of our factor values.
            //     targetDirection = surfaceDirection *
            //         (horizontalDirectionMagnitude - surfaceDirectionDistance) * surfaceDirectionStrength * m_HorizontalFrictionFactor;

            //     ClearRaycasts();
            // }

            if (hitCount > 0)
            {
                RaycastHit horizontalHit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                    m_Collider,
                    hitCount,
                    m_RaycastHits,
                    horizontalMoveDirection,
                    COLLIDER_OFFSET
                );
                Vector3 hitPoint = horizontalHit.point;

                // If the hit point can be stepped over, do not continue. Vertical collision detection will handle the actual step-up logic.
                bool canStepOver = CanStepOver(hitPoint, horizontalMoveDirection, normalizedHorizontalDirection, horizontalDirectionMagnitude);
                m_CanValidateHorizontalDirection = !canStepOver; // If a step over happened, ignore horizontal validation this frame.
                ClearOverlapColliders();

                if (canStepOver) return;

                // Create direction that allows character to slide off surfaces by projecting the horizontal direction onto the hit surface.
                // Uses same magnitude as horizontal direction, so make sure to remove horizontal direction before applying target direction.
                Vector3 horizontalNormal = ConvertToHorizontalDirection(horizontalHit.normal);
                Vector3 surfaceDirection = ConvertToHorizontalDirection(horizontalMoveDirection, horizontalNormal).normalized;

                // Calculate the strength of the surface direction based on the direction of the horizontal move direction.
                // The Dot product of the normalized horizontal direction and inverse horizontal normal is perfect, as it uses cosine.
                float surfaceDirectionStrength = 1.0f - Vector3.Dot(normalizedHorizontalDirection, -horizontalNormal);

                // If the horizontal magnitude is greater than the radius of the collider, than the collider distance will be used.
                // This will prevent overlap. Collisions behave odd if an object moves more than its radius and has another collision next frame.
                Vector3 colliderPoint = ECCPhysicsHelper.GetClosestColliderPoint(m_Collider, horizontalMoveDirection, hitPoint);
                float colliderDistance = (hitPoint - colliderPoint).magnitude - COLLIDER_OFFSET;
                float surfaceDirectionDistance = Mathf.Min(colliderDistance, horizontalDirectionMagnitude);

                // Calculate the target move direction by multiplying the surface direction by all of our factor values.
                targetDirection = surfaceDirection *
                    (horizontalDirectionMagnitude - surfaceDirectionDistance) * surfaceDirectionStrength * m_HorizontalFrictionFactor;

                ClearRaycasts();
            }

            // Do another cast in the target direction and make sure the final direction is valid.
            Vector3 normalizedTargetDirection = targetDirection.normalized;
            float targetDirectionMagnitude = targetDirection.magnitude;
            hitCount = NonAllocCapsuleCast(
                targetDirection,
                targetDirection,
                ref m_RaycastHits,
                m_DebugHorizontalWallCast
            );

            if (hitCount > 0)
            {
                RaycastHit horizontalHit = ECCPhysicsHelper.GetClosestRaycastHitRecursive(
                    m_Collider,
                    hitCount,
                    m_RaycastHits,
                    horizontalMoveDirection,
                    COLLIDER_OFFSET
                );
                Vector3 hitPoint = horizontalHit.point;

                // If the hit point can be stepped over, do not continue. Vertical collision detection will handle the actual step-up logic.
                bool canStepOver = CanStepOver(hitPoint, targetDirection, normalizedTargetDirection, targetDirectionMagnitude);
                Debug.Log("Can Step Over? " + canStepOver);
                // m_CanValidateHorizontalDirection = !canStepOver; // If a step over happened, ignore horizontal validation this frame.
                // ClearOverlapColliders();

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
                //     //     // if (IsValidSlopeAngle(m_SingleRaycastHit.normal)) return;
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

        // Returns true if the character can step over a point. Otherwise, returns false.
        private bool CanStepOver(Vector3 point, Vector3 horizontalMoveDirection, Vector3 castDirection, float? horizontalMagnitude = null)
        {
            // The passed in directions must be a horizontal direction.
            if (horizontalMoveDirection.y != 0.0f)
            {
                horizontalMoveDirection = ConvertToHorizontalDirection(horizontalMoveDirection);
            }

            if (castDirection.y != 0.0f)
            {
                castDirection = ConvertToHorizontalDirection(castDirection);
            }

            // Make sure the horizontal magnitude is not null.
            horizontalMagnitude = horizontalMagnitude != null ? horizontalMagnitude : horizontalMoveDirection.magnitude;

            // Check if character can step over. This can only happen if the hit point is less than the maximum step height.
            Vector3 localPoint = transform.InverseTransformPoint(point);
            if (localPoint.y <= m_MaxStep + COLLIDER_OFFSET)
            {
                // Check if the point is on a valid slope angle.
                float maxDistance = STEP_CHECK_OFFSET + COLLIDER_OFFSET;
                Vector3 origin = point - (horizontalMoveDirection * STEP_CHECK_OFFSET);
                if (RaycastForValidSlope(origin, castDirection, maxDistance)) return true;

                // Do another capsule cast that is vertically offset by the step height and check for slope one last time.
                if (SimpleCapsuleCast(
                    horizontalMoveDirection + (transform.up * m_MaxStep),
                    horizontalMoveDirection,
                    out m_SingleRaycastHit,
                    m_DebugHorizontalStepCast
                ))
                {
                    // Make sure distance is within magnitude of move direction. Otherwise, next frame or two can handle this.
                    if (m_SingleRaycastHit.distance - COLLIDER_OFFSET < (float)horizontalMagnitude)
                    {
                        // Once again, check if the point is on a valid slope angle.
                        if (RaycastForValidSlope(origin, castDirection, maxDistance)) return true;
                    }
                }

                // Perform an overlap test that is vertically offset by the step height and offseted in the horizontal direction times the radius.
                // If nothing is overlapping, then we can step over.
                return NonAllocCapsuleOverlap(
                    horizontalMoveDirection + (transform.up * m_MaxStep) + horizontalMoveDirection.normalized
                        * (ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider) * m_Collider.radius + COLLIDER_OFFSET),
                    ref m_OverlappingColliders,
                    m_DebugHorizontalStepCast
                ) == 0;
            }

            return false;
        }

        // Performs a raycast from an origin in a direction and then checks the slope angle.
        // If the angle is less than the maximum slope angle, returns true. Otherwise, returns false.
        private bool RaycastForValidSlope(Vector3 origin, Vector3 direction, float? maxDistance = null)
        {
            SimpleRaycast(
                origin,
                direction,
                out m_SingleRaycastHit,
                maxDistance != null ? (float)maxDistance : direction.magnitude
            );

            return IsValidSlopeAngle(m_SingleRaycastHit.normal);
        }

        // Returns true if the normal angle is less than the maximum slope angle. Otherwise, returns false.
        private bool IsValidSlopeAngle(Vector3 normal)
        {
            float angle = Vector3.Angle(normal, transform.up);
            return IsValidSlopeAngle(angle);
        }

        // Returns true if the provided angle is less than the maximum slope angle. Otherwise, return false.
        private bool IsValidSlopeAngle(float angle)
        {
            return angle <= m_MaxSlopeAngle + m_SlopeLeewayAngle;
        }

        #endregion

        #region Vertical Collision Detection

        // TODO: https://app.asana.com/0/1200147678177766/1200147678177807
        private void DetectVerticalCollisions()
        {
            if (!m_VerticalCollisionsEnabled) return;

            float originalMoveDistance = m_MoveDirection.magnitude;
            Vector3 horizontalMoveDirection = ConvertToHorizontalDirection(m_MoveDirection, -m_GravityDirection);
            Vector3 targetDirection = Vector3.zero;

            // Cast in downward position.
            Vector3 verticalOffset = (horizontalMoveDirection.normalized * COLLIDER_OFFSET) + (transform.up * COLLIDER_OFFSET);
            int hitCount = NonAllocCapsuleCast(
                verticalOffset,
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
                    verticalOffset,
                    COLLIDER_OFFSET
                );

                // If slope is invalid, do not continue.
                if (!IsValidSlopeAngle(hit.normal)) return;

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
                        COLLIDER_OFFSET,
                        ~m_CollisionIgnoreLayer.value
                    );
                    if (IsValidSlopeAngle(m_SingleRaycastHit.normal))
                    {
                        targetDirection.y = 0.0f;
                    }
                }

                CalculateOverlapCorrection(
                    hit.collider,
                    (horizontalMoveDirection.normalized * COLLIDER_OFFSET) + (transform.up * COLLIDER_OFFSET),
                    out Vector3 dir
                );
                targetDirection.y += dir.y;
            }
            else
            {
                m_IsGrounded = false;
                m_GravityFactor = 1.0f;
            }

            ClearRaycasts();

            m_MoveDirection += targetDirection;
        }

        #endregion

        #region Move Direction Correction

        // Checks for any overlapping colliders and adjusts accordingly. Should be invoked after all final move direction has been calculated.
        private void ValidateMoveDirection()
        {
            if (!m_FixCollisionOverlaps || (!m_HorizontalCollisionsEnabled && !m_VerticalCollisionsEnabled)) return;

            int overlapCount = NonAllocCapsuleOverlap(
                m_MoveDirection,
                ref m_OverlappingColliders,
                true
            );

            // Calculate direction character must move to seperate from all overlapped colliders.
            Vector3 direction = Vector3.zero;
            Vector3 correctionDirection = Vector3.zero;
            for (int i = 0; i < overlapCount; i++)
            {
                CalculateOverlapCorrection(m_OverlappingColliders[i], m_MoveDirection, out direction);
                correctionDirection += direction;
            }

            // Seperate vertical and horizontal directions.
            Vector3 horizontalDirection = ConvertToHorizontalDirection(correctionDirection);
            Vector3 verticalDirection = ConvertToVerticalDirection(correctionDirection);

            // Apply horizontal and/or vertical directions based on enabled status.
            Vector3 targetDirection = Vector3.zero;
            if (m_HorizontalCollisionsEnabled && m_CanValidateHorizontalDirection)
            {
                targetDirection += horizontalDirection;
            }

            if (m_VerticalCollisionsEnabled && m_CanValidateVerticalDirection)
            {
                targetDirection += verticalDirection;
            }

            // Apply current direction magnitude to keep speed while correcting direction.
            float magnitude = Mathf.Max(m_MoveDirection.magnitude, targetDirection.magnitude);
            m_MoveDirection = (targetDirection + m_MoveDirection).normalized * magnitude;
        }

        #endregion

        #region Math Helpers

        // Converts a direction to a horizontal (X & Z only) direction.
        private Vector3 ConvertToHorizontalDirection(Vector3 direction, Vector3? upNormal = null)
        {
            return Vector3.ProjectOnPlane(direction, upNormal != null ? (Vector3)upNormal : transform.up);
        }

        // Converts a direction to a vertical (Y only) direction.
        private Vector3 ConvertToVerticalDirection(Vector3 direction, Vector3? forwardNormal = null, Vector3? rightNormal = null)
        {
            return Vector3.ProjectOnPlane(
                Vector3.ProjectOnPlane(direction, forwardNormal != null ? (Vector3)forwardNormal : transform.forward),
                rightNormal != null ? (Vector3)rightNormal : transform.right
            );
        }

        #endregion

        #region Collision Helpers

        private void CalculateOverlapCorrection(Collider collider, Vector3 offset, out Vector3 direction)
        {
            float radius = (m_Collider.radius * ECCPhysicsHelper.GetCapsuleRadiusScale(m_Collider)) + COLLIDER_OFFSET;

            // Set radius of capsule to account for scale and small offset.
            m_Collider.radius += radius;
            if (UPhysics.ComputePenetration(
                m_Collider, m_Collider.transform.position + offset, m_Collider.transform.rotation,
                collider, collider.transform.position, collider.transform.rotation,
                out direction, out float distance
            ))
            {
                direction = direction.normalized * (distance + COLLIDER_OFFSET);
            }

            // Reset radius.
            m_Collider.radius -= radius;
        }

        // Peforms a raycast that returns true when any one collider is hit; otherwise, returns false.
        private bool SimpleRaycast(Vector3 origin, Vector3 direction, out RaycastHit raycastHit, float distance, bool debugDraw = false)
        {
            return Physics.Raycast(
                origin,
                direction,
                out raycastHit,
                distance,
                ~m_CollisionIgnoreLayer.value,
                debugDraw && Application.isEditor ? PreviewCondition.Both : PreviewCondition.None,
                0.0f,
                Color.red,
                Color.green
            );
        }

        // Performs a capsule cast that can hit multiple colliders. Returns the number of items hit and stores the raycast hits in an array.
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
                debugDraw && Application.isEditor ? PreviewCondition.Both : PreviewCondition.None,
                0.0f,
                Color.red,
                Color.green
            );
        }

        // Performs a capsule cast that returns true when any one collider is hit; otherwise, returns false.
        private bool SimpleCapsuleCast(Vector3 offset, Vector3 direction, out RaycastHit raycastHit, bool debugDraw = false)
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

            return Physics.CapsuleCast(
                capStart,
                capEnd,
                radius,
                direction.normalized,
                out raycastHit,
                direction.magnitude,
                ~m_CollisionIgnoreLayer.value,
                debugDraw && Application.isEditor ? PreviewCondition.Both : PreviewCondition.None,
                0.0f,
                Color.red,
                Color.green
            );
        }

        // Performs a capsule overlap. Returns the number of items overlapping and stores the overlapped colliders in an array.
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
                debugDraw && Application.isEditor ? PreviewCondition.Both : PreviewCondition.None,
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

        private void ClearOverlapColliders()
        {
            for (int i = 0; i < m_OverlappingColliders.Length; i++)
            {
                m_OverlappingColliders[i] = null;
            }
        }

        #endregion

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
