using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        [Tooltip("The speed at which the character's collider will seperate from an overlapped collider.")]
        [SerializeField]
        private ECCFloatReference m_CollisionCorrectionSpeed = 50.0f;
        [SerializeField]
        private ECCIntReference m_MaxCollisions = 10;

        [Header("Ground Settings")]
        [SerializeField]
        private ECCFloatReference m_GroundRadius = 1.0f;
        [SerializeField]
        private ECCFloatReference m_SkinWidth = 0.1f;
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
        }

        private void Update()
        {
            m_Input = m_Controller != null ? m_Controller.GetInput() : Vector2.zero;

            Vector2 input = m_Input.normalized * Mathf.Max(Mathf.Abs(m_Input.x), Mathf.Abs(m_Input.y));
            m_Motor = transform.TransformDirection(input.x, 0.0f, input.y);
        }

        private void FixedUpdate()
        {
            // TODO: Move elsewhere when ready.
            Vector3 eulerRot = m_Rigidbody.rotation.eulerAngles;
            eulerRot.y = Camera.main.transform.eulerAngles.y;
            m_Rigidbody.rotation = Quaternion.Euler(eulerRot);

            // Apply forces.
            m_MoveDirection += m_Motor + (m_UseGravity ? (m_GravityDirection * (m_Gravity * m_GravityFactor)) : Vector3.zero);

            // Ensure movement over any collision is smooth and correct any overlaps.
            HandleHorizontalCollisions();
            HandleVerticalCollisions();

            // Move character after all calculations are completed.
            // Make sure move direction is multiplied by delta time as the direction vector is too large for per-frame movement.
            m_Rigidbody.MovePosition(m_Rigidbody.position + (m_MoveDirection * Time.fixedDeltaTime));
            m_MoveDirection = Vector3.zero;
        }

        private void HandleHorizontalCollisions()
        {
            // TODO: Implement...
        }

        private Vector3 CheckHorizontalCollisions(Vector3 moveDirection)
        {
            Vector3 direction = Vector3.zero;

            Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(moveDirection, transform.up);
            // Debug.Log(horizontalMoveDirection);
            int amountHit = NonAllocCapsuleCast(
                m_Collider,
                m_Collider.transform.position,
                m_Collider.transform.rotation,
                m_Collider.radius,
                horizontalMoveDirection,
                ref m_RaycastHits
            );
            Debug.DrawRay(m_Collider.transform.position, horizontalMoveDirection, Color.magenta);
            if (amountHit == 0)
            {
                return Vector3.zero;
            }

            for (byte i = 0; i < amountHit; i++)
            {
                RaycastHit hit = m_RaycastHits[i];

                Debug.DrawRay(hit.point, hit.normal, Color.cyan);
                Debug.DrawRay(m_Collider.transform.position, hit.point - m_Collider.transform.position, Color.red);
            }

            return direction;
        }

        private void HandleVerticalCollisions()
        {
            Vector3 verticalMoveDirection = CreateGroundMoveDirection(m_MoveDirection);
            m_MoveDirection += verticalMoveDirection;
        }

        private Vector3 CreateGroundMoveDirection(Vector3 moveDirection)
        {
            Vector3 direction = Vector3.zero;

            RaycastHit[] hits = PerformGroundCast(moveDirection * Time.fixedDeltaTime); // Multiply by delta time to find "next frame" direction.
            List<RaycastHit> validHits = new List<RaycastHit>();
            for (int i = 0; i < hits.Length; i++)
            {
                // If collider has any overlaps, find the direction to seperate the overlap and move the character accordingly.
                bool overlapped = IsOverlapped(hits[i].collider, Vector3.zero, out Vector3 dir, out float distance);
                direction += (dir * distance * m_CollisionCorrectionSpeed);

                // Calculate slope. If it's greater than the maximum slope allowed, than we are not grounded.
                float angle = Vector3.Angle(hits[i].normal, m_Collider.transform.up);
                if (angle > m_MaxSlopeAngle) continue;

                validHits.Add(hits[i]);
            }

            if (validHits.Count > 0)
            {
                m_IsGrounded = true;
                m_GravityFactor = 0.0f;

                RaycastHit closestHit = validHits[0];
                for (int i = 0; i < validHits.Count; ++i)
                {
                    if (hits[i].distance < closestHit.distance)
                    {
                        closestHit = hits[i];
                    }
                }

                /**
                  * TODO: Fix character "jumping" over top of slope.
                  * Since m_SkinWidth is not used here, sometimes the CapsuleCast will miss a collider and gravity will turn back on.
                  * This will make the character "jump" over the top of a slope.
                  * Only a concern for slopes with high angles.
                  */
                // Get the normal of the hit that will rotate with the character. Used to create a direction that is up or down any angle.
                Vector3 hitNormal = Vector3.ProjectOnPlane(closestHit.normal, transform.right).normalized;

                // Create a target direction that gets the orthogonal direction from the hitNormal and horizontalMoveDirection,
                // and then gets another orthogonal direction that is placed on the surface in the correct orientation.
                Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(moveDirection + direction, -m_GravityDirection);
                Vector3 targetDirection = -Vector3.Cross( // Make negative because this returns a backwards direction.
                    closestHit.normal,
                    Vector3.Cross(hitNormal, horizontalMoveDirection.normalized).normalized
                ).normalized;

                // Exclude horizontal move direction to prevent wrong speed or directions.
                direction += (targetDirection * horizontalMoveDirection.magnitude) - horizontalMoveDirection;
            }
            else
            {
                m_IsGrounded = false;
                m_GravityFactor = 1.0f;
            }

            return direction;
        }

        private bool IsOverlapped(Collider collider, Vector3 offset, out Vector3 direction, out float distance)
        {
            Vector3 dir = Vector3.zero;

            bool overlapped = Physics.ComputePenetration(
                m_Collider, m_Collider.transform.position + offset, m_Collider.transform.rotation,
                collider, collider.transform.position, collider.transform.rotation,
                out direction, out distance
            );

            return overlapped;
        }

        private RaycastHit[] PerformGroundCast(Vector3 moveDirection)
        {
            m_Collider.radius += COLLIDER_OFFSET;

            // Get direction of capsule and calculate half of the height, including scale and skin width.
            float heightScale = ECCColliderHelper.GetCapsuleHeightScale(m_Collider);
            float adjustedHalfHeight = (m_Collider.height / 2.0f) + m_SkinWidth;
            float scaledHalfHeight = adjustedHalfHeight * heightScale;
            Vector3 direction = ECCColliderHelper.GetCapsuleDirection(m_Collider);

            // Calculate a capsule position that covers the TOP half of the CapsuleCollider.
            // Since the adjusted half height is used as the full height, we need to half it one more time.
            Vector3 topHalfCapsulePos = m_Collider.transform.position
                + (direction * (scaledHalfHeight / 2.0f))
                + (m_Collider.transform.InverseTransformDirection(moveDirection));
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                topHalfCapsulePos,
                m_Collider.transform.rotation,
                out Vector3 topCapStart,
                out Vector3 topCapEnd,
                adjustedHalfHeight,
                (m_Collider.radius + COLLIDER_OFFSET) * m_GroundRadius
            );

            // Shoot the CapsuleCast in the inverse direction of the capsule from the top half position.
            RaycastHit[] hits = Physics.CapsuleCastAll(
                topCapStart,
                topCapEnd,
                (m_Collider.radius + COLLIDER_OFFSET) * m_GroundRadius,
                -direction,
                scaledHalfHeight,
                ~m_CharacterLayer.value
            );

            m_Collider.radius -= COLLIDER_OFFSET;

            return hits;
        }

        private int NonAllocCapsuleCast(
            CapsuleCollider collider,
            Vector3 position,
            Quaternion rotation,
            float radius,
            Vector3 direction,
            ref RaycastHit[] hits
        )
        {
            ECCColliderHelper.CalculateCapsuleCaps(
                collider,
                position,
                rotation,
                out Vector3 capStart,
                out Vector3 capEnd
            );

            return Physics.CapsuleCastNonAlloc(capStart, capEnd, radius, direction.normalized, hits, direction.magnitude, ~m_CharacterLayer.value);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            DrawHorizontalCheck();
            DrawGroundCheck();

            Gizmos.color = color;
        }

        private void DrawHorizontalCheck()
        {
            Vector3 moveDirection = Vector3.zero;
            m_Collider = m_Collider ? m_Collider : GetComponentInChildren<CapsuleCollider>();
            m_Collider.radius += COLLIDER_OFFSET;

            // TODO: Draw stuff here...

            m_Collider.radius -= COLLIDER_OFFSET;
        }

        private void DrawGroundCheck()
        {
            Color tempColor = Gizmos.color;

            Vector3 moveDirection = m_MoveDirection;
            m_Collider = m_Collider ? m_Collider : GetComponentInChildren<CapsuleCollider>();
            m_Collider.radius += COLLIDER_OFFSET;

            // Calculate half of the height, including scale and skin width.
            float heightScale = ECCColliderHelper.GetCapsuleHeightScale(m_Collider);
            float adjustedHalfHeight = (m_Collider.height / 2.0f) + m_SkinWidth;
            float scaledHalfHeight = adjustedHalfHeight * heightScale;
            Vector3 direction = ECCColliderHelper.GetCapsuleDirection(m_Collider);

            // Calculate a capsule position that covers the TOP half of the CapsuleCollider.
            // Since the adjusted half height is used as the full height, we need to half it one more time.
            Vector3 topHalfCapsulePos = m_Collider.transform.position
                + (direction * (scaledHalfHeight / 2.0f))
                + (m_Collider.transform.InverseTransformDirection(moveDirection));
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                topHalfCapsulePos,
                m_Collider.transform.rotation,
                out Vector3 topCapStart,
                out Vector3 topCapEnd,
                adjustedHalfHeight,
                m_Collider.radius * m_GroundRadius
            );

            RaycastHit[] hits = Physics.CapsuleCastAll(
                topCapStart,
                topCapEnd,
                m_Collider.radius * m_GroundRadius,
                -direction,
                scaledHalfHeight,
                ~m_CharacterLayer.value
            );
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                Gizmos.color = Color.cyan;
                Vector3 closestPoint = m_Collider.ClosestPoint(hit.point);
                Gizmos.DrawSphere(hit.point, 0.1f);
                Gizmos.DrawWireSphere(closestPoint, 0.1f);
            }

            // Draw bottom half.
            DrawWireCapsule(
                topHalfCapsulePos,
                m_Collider.transform.rotation,
                m_Collider.radius * m_GroundRadius,
                scaledHalfHeight,
                Color.green
            );
            DrawWireCapsule(
                topHalfCapsulePos - (direction * (1.0f + m_SkinWidth)),
                m_Collider.transform.rotation,
                m_Collider.radius * m_GroundRadius,
                scaledHalfHeight,
                Color.green
            );

            m_Collider.radius -= COLLIDER_OFFSET;
        }

        private static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;

                // Draw sideways.
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);

                // Draw frontways.
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);

                // Draw center.
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
        }
        #endif
    }
}

// TODO: Old reference code that may be useful.
// *************************************************************************
// * Creates a horizontal target direction from a hit surface.             *
// * The direction is oriented from the surface normal, not the character. *
// * If used, make sure to account for that oritentation difference.       *
// *************************************************************************
// Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MovementDirection, transform.up); // Convert move direction to horizontal.
// Vector3 orthoHitNormal = Vector3.ProjectOnPlane(closestHit.normal, transform.up).normalized; // Return an orthogonal version of hit.normal.
// Vector3 targetDirection = Vector3.Cross(orthoHitNormal, transform.up).normalized; // Return a orthogonal Vector based on the orthHitNormal and transform.up.

// Vector3 slopeDirection = Vector3.Cross(orthoHitNormal, horizontalMoveDirection).normalized;
// bool movingDown = Vector3.Dot(slopeDirection, transform.up) > 0.0f;
// if (movingDown)
// {
//     targetDirection = -targetDirection;
// }

// Debug.DrawRay(transform.position, horizontalMoveDirection.normalized, Color.cyan);

// // Draw hit normals.
// Debug.DrawRay(closestHit.point, transform.up, Color.magenta); // Right direction "normal"
// Debug.DrawRay(closestHit.point, orthoHitNormal, Color.green); // Orthogonal normal of regular hit.normal
// Debug.DrawRay(closestHit.point, closestHit.normal, Color.green); // Regular hit.normal
// Debug.DrawRay(transform.position, targetDirection, Color.magenta); // Target direction based on normals
