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

        // TODO: Move ground checking to a custom behaviour?
        [Header("Ground Settings")]
        [SerializeField]
        private ECCFloatReference m_GroundRadius = 1.0f;
        [SerializeField]
        private ECCVector3Reference m_GroundOffset = Vector3.zero;
        [SerializeField]
        private ECCFloatReference m_GroundDistance = 1.0f;
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
        private Vector3 m_MovementDirection = Vector3.zero;
        private Vector3 m_PrevPosition = Vector3.zero;
        private Vector3 m_UpdatePosition = Vector3.zero;
        private Vector3 m_GravityDirection = -Vector3.up;
        private bool m_IsGrounded = false;

        protected override void Initialize()
        {
            // TODO: Create custom inspector that adds through dropdown rather than manual string type names.
            // TODO: Create custom inspector to drag and order the priority of each behaviour.
            m_CharacterBehaviours = FindCharacterBehaviours();

            m_Rigidbody = GetComponent<Rigidbody>();
            SetupRigidbody();

            m_Collider = GetComponentInChildren<CapsuleCollider>();
            m_UpdatePosition = m_PrevPosition = m_Rigidbody.position;
            m_MovementDirection = Vector3.zero;
            m_GravityDirection = -transform.up;

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

        float horizontal;
        float vertical;
        float mouseX;
        float mouseY;
        private void Update()
        {
            // TODO: Test input. Move elsewhere sometime.
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        private void DrawGroundCheck()
        {
            Color tempColor = Gizmos.color;

            Vector3 moveDirection = m_MovementDirection;
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
                Gizmos.DrawSphere(hit.point, 0.1f);
                Vector3 closestPoint = m_Collider.ClosestPoint(hit.point);
                Gizmos.DrawWireSphere(closestPoint, 0.1f);
                Gizmos.DrawWireSphere(transform.TransformPoint(transform.position - closestPoint), 0.1f);
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


            // if (CheckForGround(moveDirection, out RaycastHit closestHit, out Vector3 bottomHalfCapsulePos, out Vector3 topHalfCapsulePos))
            // {
            //     // Correct collision.
            //     bool overlapped = Physics.ComputePenetration(
            //         m_Collider, m_Collider.transform.position, m_Collider.transform.rotation,
            //         closestHit.collider, closestHit.transform.position, closestHit.transform.rotation,
            //         out Vector3 dir, out float dis
            //     );
            //     if (overlapped && dis > 0.001f)
            //     {
            //         moveDirection += (direction * (1.0f - m_SkinWidth)) * dis;
            //     }

            //     // Draw normal hit.
            //     tempColor = Gizmos.color;
            //     Gizmos.color = Color.cyan;
            //     Gizmos.DrawRay(closestHit.point, closestHit.normal * 3.0f);

            //     // Draw ground cast collision points.
            //     tempColor = Gizmos.color;
            //     Gizmos.color = Color.cyan;
            //     Vector3 closestPoint = m_Collider.ClosestPoint(closestHit.point);
            //     Gizmos.DrawWireSphere(closestPoint, 0.1f);
            //     Gizmos.DrawSphere(closestHit.point, 0.1f);
            //     Gizmos.color = tempColor;

            //     // Draw top half.
            //     DrawWireCapsule(
            //         topHalfCapsulePos,
            //         m_Collider.transform.rotation,
            //         m_Collider.radius,
            //         scaledHalfHeight,
            //         Color.cyan
            //     );

            //     // Draw bottom half.
            //     DrawWireCapsule(
            //         bottomHalfCapsulePos,
            //         m_Collider.transform.rotation,
            //         m_Collider.radius,
            //         scaledHalfHeight,
            //         Color.red
            //     );
            // }
            // else
            // {
            //     // Draw bottom half.
            //     DrawWireCapsule(
            //         bottomHalfCapsulePos,
            //         m_Collider.transform.rotation,
            //         m_Collider.radius,
            //         scaledHalfHeight,
            //         Color.green
            //     );
            // }

            m_Collider.radius -= COLLIDER_OFFSET;
        }

        private void HandleVerticalCollisions(ref Vector3 moveDirection)
        {
            m_Collider.radius += COLLIDER_OFFSET;

            // Check for the ground before doing anything else.
            m_IsGrounded = CheckForGround(moveDirection, out RaycastHit closestHit);
            Vector3 offset = Vector3.zero;
            if (m_IsGrounded)
            {
                // Correct collision.
                bool overlapped = Physics.ComputePenetration(
                    m_Collider, m_Collider.transform.position, m_Collider.transform.rotation,
                    closestHit.collider, closestHit.transform.position, closestHit.transform.rotation,
                    out Vector3 direction, out float distance
                );
                if (overlapped && distance > 0.001f)
                {
                    offset += (direction * (1.0f + m_SkinWidth)) * distance;
                }
            }

            // Vector3 closestPoint = m_Collider.ClosestPoint(closestHit.point);
            // Vector3 v = Vector3.ProjectOnPlane((m_Rigidbody.position - closestPoint) + (moveDirection + offset), closestHit.normal).normalized;
            // Debug.DrawRay(transform.TransformPoint((m_Rigidbody.position - closestPoint) + (moveDirection + offset)), v * 3.0f, Color.cyan);

            Vector3 directionNormal = Vector3.ProjectOnPlane(moveDirection, closestHit.normal);
            moveDirection = directionNormal.normalized * moveDirection.magnitude;
            // moveDirection += offset + (directionNormal - directionNormal);
            Debug.DrawRay(m_Rigidbody.position + moveDirection, directionNormal.normalized * 3.0f, Color.cyan);

            m_Collider.radius -= COLLIDER_OFFSET;
        }

        private bool CheckForGround(Vector3 moveDirection, out RaycastHit closestHit, out Vector3 bottomHalfCapsulePos, out Vector3 topHalfCapsulePos)
        {
            closestHit = default(RaycastHit);
            topHalfCapsulePos = Vector3.zero;

            // Get direction of capsule and calculate half of the height, including scale and skin width.
            float heightScale = ECCColliderHelper.GetCapsuleHeightScale(m_Collider);
            float adjustedHalfHeight = (m_Collider.height / 2.0f) + m_SkinWidth;
            float scaledHalfHeight = adjustedHalfHeight * heightScale;
            Vector3 direction = ECCColliderHelper.GetCapsuleDirection(m_Collider);

            // Calculate a position for the ground check at the BOTTOM half of the CapsuleCollider.
            // Since the adjusted half height is used as the full height, we need to half it one more time.
            bottomHalfCapsulePos = m_Collider.transform.position
                - (direction * (scaledHalfHeight / 2.0f))
                + (m_Collider.transform.InverseTransformDirection(moveDirection));
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                bottomHalfCapsulePos,
                m_Collider.transform.rotation,
                out Vector3 bottomCapStart,
                out Vector3 bottomCapEnd,
                adjustedHalfHeight
            );

            // Use CheckCapsule to detect a collision. Better than always performing a CapsuleCast.
            bool grounded = Physics.CheckCapsule(bottomCapStart, bottomCapEnd, m_Collider.radius, ~m_CharacterLayer.value);
            if (grounded)
            {
                // Perform CapsuleCast to check if the collision is actually the ground.
                RaycastHit[] hits = PerformGroundCast(moveDirection, out topHalfCapsulePos);
                if (hits.Length > 0)
                {
                    closestHit.distance = 999.0f;

                    // Find the closest RaycastHit.
                    bool isValid = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        // Calculate slope. If it's greater than the maximum slope allowed, than we are not grounded.
                        float angle = Vector3.Angle(hits[i].normal, m_Collider.transform.up);
                        if (angle > m_MaxSlopeAngle) continue;

                        if (hits[i].distance < closestHit.distance)
                        {
                            closestHit = hits[i];
                            isValid = true;
                        }
                    }

                    // If a closest point was found, that means it also passed the slope check, meaning the ground is valid.
                    grounded = isValid;
                }
                else
                {
                    grounded = false;
                }
            }

            return grounded;
        }

        private bool CheckForGround(Vector3 moveDirection, out RaycastHit closestHit)
        {
            return CheckForGround(moveDirection, out closestHit, out Vector3 bottomHalfCapsulePos, out Vector3 topHalfCapsulePos);
        }

        private RaycastHit[] PerformGroundCast(Vector3 moveDirection, out Vector3 topHalfCapsulePos)
        {
            // Get direction of capsule and calculate half of the height, including scale and skin width.
            float heightScale = ECCColliderHelper.GetCapsuleHeightScale(m_Collider);
            float adjustedHalfHeight = (m_Collider.height / 2.0f) + m_SkinWidth;
            float scaledHalfHeight = adjustedHalfHeight * heightScale;
            Vector3 direction = ECCColliderHelper.GetCapsuleDirection(m_Collider);

            // Calculate a capsule position that covers the TOP half of the CapsuleCollider.
            // Since the adjusted half height is used as the full height, we need to half it one more time.
            topHalfCapsulePos = m_Collider.transform.position
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
            return Physics.CapsuleCastAll(
                topCapStart,
                topCapEnd,
                (m_Collider.radius + COLLIDER_OFFSET) * m_GroundRadius,
                -direction,
                scaledHalfHeight,
                ~m_CharacterLayer.value
            );
        }

        private RaycastHit[] PerformGroundCast(Vector3 moveDirection)
        {
            return PerformGroundCast(moveDirection, out Vector3 topHalfCapsulePos);
        }

        private void FixedUpdate()
        {
            m_MovementDirection = !m_IsGrounded && m_UseGravity ? GetGravityPosition(m_MovementDirection, m_GravityDirection) : m_MovementDirection;

            // TODO: Test movement and rotation. Move elsewhere sometime.
            m_Rigidbody.rotation = GetMovementRotation(m_Rigidbody.rotation.eulerAngles);
            m_MovementDirection = GetHorizontalPosition(m_MovementDirection);

            // HandleVerticalCollisions(ref m_MovementDirection);

            // TODO: Move ground stuff...
            RaycastHit[] hits = PerformGroundCast(m_MovementDirection);
            List<RaycastHit> validHits = new List<RaycastHit>();
            for (int i = 0; i < hits.Length; i++)
            {
                // Calculate slope. If it's greater than the maximum slope allowed, than we are not grounded.
                float angle = Vector3.Angle(hits[i].normal, m_Collider.transform.up);
                if (angle > m_MaxSlopeAngle) continue;

                validHits.Add(hits[i]);
            }

            if (validHits.Count > 0)
            {
                m_IsGrounded = true;

                RaycastHit closestHit = validHits[0];
                for (int i = 0; i < validHits.Count; ++i)
                {
                    if (hits[i].distance < closestHit.distance)
                    {
                        closestHit = hits[i];
                    }
                }

                // // NON-NORMALIZED
                // float dis = (closestHit.point - m_Rigidbody.position).y - COLLIDER_OFFSET;
                // dis = Mathf.Abs(dis) <= COLLIDER_OFFSET ? 0.0f : dis;

                // Vector3 moveDirectionNormal = Vector3.ProjectOnPlane(m_MovementDirection, closestHit.normal);
                // m_MovementDirection.y += (dis - moveDirectionNormal.y) - (COLLIDER_OFFSET * 2.0f);  // Acount for both offsets.

                // NORMALIZED
                // Vector3 projectedMoveDirection = Vector3.ProjectOnPlane(m_MovementDirection, closestHit.normal);
                // m_MovementDirection += (projectedMoveDirection - m_MovementDirection);

                // Calculate the movement direction based on angles of collision. Could be a small slope or a right degree wall.
                Vector3 horizontalMoveDirection = Vector3.ProjectOnPlane(m_MovementDirection, transform.up); // Convert move direction to horizontal.
                Debug.Log(horizontalMoveDirection);
                Vector3 orthoHitNormal = Vector3.ProjectOnPlane(closestHit.normal, transform.up).normalized; // Return an orthogonal version of hit.normal.
                Vector3 targetDirection = Vector3.Cross(orthoHitNormal, transform.up).normalized; // Return a orthogonal Vector based on the orthHitNormal and transform.up.

                // Returns an orthogonal Vector that is pointing up or down. This is used to check if we are moving up or down a slope and/or angle.
                Vector3 slopeDirection = Vector3.Cross(orthoHitNormal, horizontalMoveDirection).normalized;

                bool movingDown = Vector3.Dot(slopeDirection, transform.up) > 0.0f;
                if (movingDown)
                {
                    targetDirection = -targetDirection;
                }

                // Draw hit normals.
                Debug.DrawRay(closestHit.point, transform.up, Color.green); // Up direction "normal"
                Debug.DrawRay(closestHit.point, orthoHitNormal, Color.green); // Orthogonal normal of regular hit.normal
                Debug.DrawRay(closestHit.point, closestHit.normal, Color.green); // Regular hit.normal
                Debug.DrawRay(transform.position, targetDirection, Color.magenta); // Target direction based on normals
            }
            else
            {
                m_IsGrounded = false;
            }

            // // Debug.DrawRay(transform.position, m_MovementDirection.normalized * 3.0f, Color.cyan);
            // Debug.Log(m_MovementDirection.y);
            // Debug.Log(m_MovementDirection.x);

            // Apply new updated position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_MovementDirection);
            m_MovementDirection = Vector3.zero;
        }

        private Quaternion GetMovementRotation(Vector3 rotation)
        {
            rotation.y = Camera.main.transform.eulerAngles.y;
            return Quaternion.Euler(rotation);
        }

        private Vector3 GetGravityPosition(Vector3 moveDirection, Vector3 direction)
        {
            return moveDirection + (direction * m_Gravity * Time.fixedDeltaTime);
        }

        private Vector3 GetHorizontalPosition(Vector3 moveDirection)
        {
            // TODO: Test input. Again, put elsewhere.
            Vector3 input = new Vector3(horizontal, 0.0f, vertical);
            moveDirection += Vector3.ProjectOnPlane(
                (m_Rigidbody.rotation * input) * Time.fixedDeltaTime * 5.0f,
                transform.up
            );

            // m_Collider.radius += COLLIDER_OFFSET;

            // Vector3 center, capStart, capEnd;
            // ECCColliderHelper.CalculateCapsuleCaps(
            //     m_Collider,
            //     m_Collider.transform.position + (transform.up * COLLIDER_OFFSET),
            //     m_Collider.transform.rotation,
            //     out center,
            //     out capStart,
            //     out capEnd
            // );

            // m_Collider.radius -= COLLIDER_OFFSET;

            return moveDirection;
        }

        private Vector3 GetGroundCheckPosition(Vector3 moveDirection, out bool grounded)
        {
            m_Collider.radius += COLLIDER_OFFSET;

            Vector3 center, capStart, capEnd;
            ECCColliderHelper.CalculateCapsuleCaps(
                m_Collider,
                m_Collider.transform.position + (transform.up * COLLIDER_OFFSET),
                m_Collider.transform.rotation,
                out center,
                out capStart,
                out capEnd
            );

            // If any collider is overlapped, get the direction it needs to move to not be overlapped.
            RaycastHit[] hits = ECCColliderHelper.CapsuleCastAll(
                m_Collider,
                Vector3.zero,
                -transform.up,
                m_CharacterLayer,
                true
            );
            grounded = false;
            Vector3 overlapCorrectionOffset = Vector3.zero;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                // If slope is too high, don't try to correct vertical position.
                float angle = Vector3.Angle(hit.normal, transform.up);
                if (angle > m_MaxSlopeAngle) continue;

                Collider hitCollider = hit.collider;
                if (hit.distance == 0)
                {
                    Vector3 direction;
                    float distance;
                    bool overlapped = Physics.ComputePenetration(
                        m_Collider, m_Collider.transform.position, m_Collider.transform.rotation,
                        hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
                        out direction, out distance
                    );
                    if (overlapped && distance >= 0.001f) // Account for float precision errors.
                    {
                        overlapCorrectionOffset += direction * distance;
                    }
                }
            }

            // Check again for ground to ensure actually grounded.
            float verticalOffset = 0.0f;
            RaycastHit groundHit;
            grounded = ECCColliderHelper.CapsuleCast(
                m_Collider,
                transform.up,
                -transform.up * (1.0f + (m_SkinWidth * 2.0f)),
                m_CharacterLayer,
                out groundHit,
                true
            );
            if (grounded)
            {
                Vector3 closestPoint = m_Collider.ClosestPoint(groundHit.point);
                Vector3 distance = (groundHit.point - closestPoint);
                verticalOffset = distance.y + m_SkinWidth + COLLIDER_OFFSET;

                // If slope is too high, don't use the vertical offset.
                float groundAngle = Vector3.Angle(groundHit.normal, transform.up);
                if (groundAngle > m_MaxSlopeAngle) verticalOffset = 0.0f;
            }

            // Apply positional corrections.
            moveDirection += overlapCorrectionOffset;
            moveDirection.y += verticalOffset;

            Debug.Log("Move Dir: " + moveDirection);

            m_Collider.radius -= COLLIDER_OFFSET;

            return moveDirection;
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

        // private void DrawGroundCheck()
        // {
        //     Vector3 moveDirection = Vector3.zero;
        //     m_Collider = m_Collider ? m_Collider : GetComponentInChildren<CapsuleCollider>();
        //     m_Collider.radius += COLLIDER_OFFSET;

        //     Vector3 center, capStart, capEnd;
        //     ECCColliderHelper.CalculateCapsuleCaps(
        //         m_Collider,
        //         m_Collider.transform.position + (transform.up * COLLIDER_OFFSET),
        //         m_Collider.transform.rotation,
        //         out center,
        //         out capStart,
        //         out capEnd
        //     );
        //     RaycastHit[] hits = ECCColliderHelper.CapsuleCastAll(
        //         m_Collider,
        //         Vector3.zero,
        //         -transform.up,
        //         m_CharacterLayer,
        //         true
        //     );

        //     Vector3 overlapCorrectionOffset = Vector3.zero;
        //     for (int i = 0; i < hits.Length; i++)
        //     {
        //         RaycastHit hit = hits[i];

        //         // Draw hit point normal.
        //         // Gizmos.color = Color.cyan;
        //         // RaycastHit normalHit;
        //         // if (Physics.Raycast(center, -transform.up, out normalHit, m_GroundDistance, ~m_CharacterLayer.value))
        //         // {
        //         //     Gizmos.DrawRay(normalHit.point, normalHit.normal * 3.0f);
        //         // }

        //         Gizmos.color = Color.cyan;
        //         Gizmos.DrawRay(hit.point, hit.normal * 3.0f);
        //         float angle = Vector3.Angle(hit.normal, transform.up);
        //         if (angle > m_MaxSlopeAngle) continue;
        //         // Debug.Log("Angle: " + angle);
        //         // Debug.Log("Angle - 90: " + (1 + (1 - (angle / 90.0f))));
        //         Gizmos.DrawRay(hit.point, (Quaternion.AngleAxis(angle, transform.up) * transform.up) * 3.0f);

        //         float dotAngle = Vector3.Dot(hit.normal, transform.up) * Mathf.Rad2Deg;
        //         // Debug.Log("Dot Angle:" + dotAngle);
        //         // Debug.Log("2: " + Vector3.Dot(transform.up, hit.normal) * Mathf.Rad2Deg);

        //         Vector3 cross = Vector3.Cross(hit.normal, transform.up);
        //         // Debug.Log("Cross: " + cross);
        //         // Debug.Log("Cross 2: " + Vector3.Dot(transform.TransformDirection(Vector3.Cross(transform.up, hit.normal)), transform.up));

        //         Collider hitCollider = hit.collider;
        //         if (hit.distance == 0)
        //         {
        //             Vector3 direction;
        //             float distance;
        //             bool overlapped = Physics.ComputePenetration(
        //                 m_Collider, m_Collider.transform.position, m_Collider.transform.rotation,
        //                 hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
        //                 out direction, out distance
        //             );
        //             distance = distance <= COLLIDER_OFFSET ? 0.0f : distance; // Account for float precision errors.
        //             if (overlapped && distance > 0.0f)
        //             {
        //                 overlapCorrectionOffset += direction * distance;
        //             }
        //         }

        //         // Draw closest point.
        //         Gizmos.color = Color.cyan;
        //         Vector3 closestPoint = m_Collider.ClosestPoint(hit.point);
        //         Vector3 localClosestPoint = transform.InverseTransformPoint(closestPoint);
        //         // if (localClosestPoint.y > 0)
        //         // {
        //         //     Debug.Log("Up!");
        //         // }
        //         // else if (localClosestPoint.y < 0)
        //         // {
        //         //     Debug.Log("Down!");
        //         // }
        //         // else
        //         // {
        //         //     Debug.Log("Flat!");
        //         // }
        //         Gizmos.DrawSphere(hit.point, 0.1f);
        //         Gizmos.DrawWireSphere(closestPoint, 0.1f);
        //     }

        //     // Check again for ground to ensure actually grounded.
        //     float verticalOffset = 0.0f;
        //     RaycastHit groundHit;
        //     bool grounded = ECCColliderHelper.CapsuleCast(
        //         m_Collider,
        //         transform.up,
        //         -transform.up * (1.0f + (m_SkinWidth * 2.0f)),
        //         m_CharacterLayer,
        //         out groundHit,
        //         true
        //     );
        //     if (grounded)
        //     {
        //         Vector3 closestPoint = m_Collider.ClosestPoint(groundHit.point);
        //         Vector3 distance = (groundHit.point - closestPoint);
        //         verticalOffset = distance.y + m_SkinWidth + COLLIDER_OFFSET;

        //         float groundAngle = Vector3.Angle(groundHit.normal, transform.up);
        //         if (groundAngle > m_MaxSlopeAngle) verticalOffset = 0.0f;
        //     }

        //     moveDirection += overlapCorrectionOffset;
        //     moveDirection.y += verticalOffset;

        //     Gizmos.color = Color.yellow;
        //     DrawWireCapsule(
        //         (m_Collider.transform.position +
        //         (center - m_Collider.transform.position - (transform.up * COLLIDER_OFFSET))) +
        //         moveDirection,
        //         transform.rotation,
        //         m_Collider.radius,
        //         m_Collider.height,
        //         Color.yellow
        //     );

        //     m_Collider.radius -= COLLIDER_OFFSET;
        // }

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
