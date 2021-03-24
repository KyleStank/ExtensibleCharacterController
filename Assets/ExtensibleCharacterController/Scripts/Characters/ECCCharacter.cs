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
        [SerializeField]
        private ECCFloatReference m_SkinWidth = 0.1f;

        // TODO: Move ground checking to a custom behaviour?
        [Header("Ground Settings")]
        [SerializeField]
        private ECCFloatReference m_GroundRadius = 1.0f;
        [SerializeField]
        private ECCVector3Reference m_GroundOffset = Vector3.zero;

        [SerializeField]
        private ECCFloatReference m_GroundDistance = 1.0f;

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
        private void Update()
        {
            // TODO: Test input. Move elsewhere sometime.
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        private void FixedUpdate()
        {
            // TODO: Test movement. Move elsewhere sometime.
            // Add input and gravity to movement direction.
            Vector3 input = new Vector3(horizontal, 0.0f, vertical);
            m_MovementDirection += (input * 5.0f) * Time.fixedDeltaTime;
            m_MovementDirection = !m_IsGrounded && m_UseGravity ? GetGravityPosition(m_MovementDirection, m_GravityDirection) : m_MovementDirection;

            // m_MovementDirection = GetGroundCheckPosition(m_MovementDirection, out m_IsGrounded);
            m_MovementDirection = GetGroundCheckPosition(m_MovementDirection, out m_IsGrounded);

            // Apply new updated position.
            m_Rigidbody.position += m_MovementDirection;
            m_MovementDirection = Vector3.zero;
        }

        private Vector3 GetGravityPosition(Vector3 moveDirection, Vector3 direction)
        {
            return moveDirection + (direction * m_Gravity * Time.fixedDeltaTime);
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
            }

            // Apply positional corrections.
            moveDirection += overlapCorrectionOffset;
            moveDirection.y += verticalOffset;

            m_Collider.radius -= COLLIDER_OFFSET;

            return moveDirection;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            Vector3 moveDirection = Vector3.zero;
            m_Collider = m_Collider ? m_Collider : GetComponentInChildren<CapsuleCollider>();
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
            RaycastHit[] hits = ECCColliderHelper.CapsuleCastAll(
                m_Collider,
                Vector3.zero,
                -transform.up,
                m_CharacterLayer,
                true
            );

            Vector3 overlapCorrectionOffset = Vector3.zero;
            for (int i = 0; i < hits.Length; i++)
            {
                // Draw hit point normal.
                Gizmos.color = Color.cyan;
                RaycastHit normalHit;
                if (Physics.Raycast(center, -transform.up, out normalHit, m_GroundDistance, ~m_CharacterLayer.value))
                {
                    Gizmos.DrawRay(normalHit.point, normalHit.normal * 3.0f);
                }

                RaycastHit hit = hits[i];
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

                // Draw closest point.
                Gizmos.color = Color.cyan;
                Vector3 closestPoint = m_Collider.ClosestPoint(hit.point);
                Gizmos.DrawSphere(hit.point, 0.1f);
                Gizmos.DrawWireSphere(closestPoint, 0.1f);
            }

            // Check again for ground to ensure actually grounded.
            float verticalOffset = 0.0f;
            RaycastHit groundHit;
            bool grounded = ECCColliderHelper.CapsuleCast(
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
            }

            moveDirection += overlapCorrectionOffset;
            moveDirection.y += verticalOffset;

            Gizmos.color = Color.yellow;
            DrawWireCapsule(
                (m_Collider.transform.position +
                (center - m_Collider.transform.position - (transform.up * COLLIDER_OFFSET))) +
                moveDirection,
                transform.rotation,
                m_Collider.radius,
                m_Collider.height,
                Color.yellow
            );

            m_Collider.radius -= COLLIDER_OFFSET;

            Gizmos.color = color;
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
