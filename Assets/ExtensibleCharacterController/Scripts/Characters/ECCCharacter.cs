using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using ExtensibleCharacterController.Core.Variables;
using ExtensibleCharacterController.Characters.Behaviours;

namespace ExtensibleCharacterController.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class ECCCharacter : ECCBehaviour
    {
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

        private Vector3 m_UpdatePosition = Vector3.zero;
        private bool m_IsGrounded = false;

        protected override void Initialize()
        {
            // TODO: Create custom inspector that adds through dropdown rather than manual string type names.
            // TODO: Create custom inspector to drag and order the priority of each behaviour.
            m_CharacterBehaviours = FindCharacterBehaviours();

            m_Rigidbody = GetComponent<Rigidbody>();
            SetupRigidbody();
            m_UpdatePosition = m_Rigidbody.position;

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
            Vector3 updatePosition = m_Rigidbody.position;
            m_IsGrounded = CheckForGround(updatePosition);

            // Apply all required calculations to next position.
            updatePosition = m_UseGravity ? GetGravityPosition(updatePosition, -Vector3.up) : updatePosition;
            updatePosition = GetGroundCheckPosition(updatePosition);

            // TODO: Test movement. Move elsewhere sometime.
            Vector3 input = new Vector3(horizontal, 0.0f, vertical);
            updatePosition += (input * 5.0f) * Time.fixedDeltaTime;

            // Apply new updated position.
            m_UpdatePosition = updatePosition;
            m_Rigidbody.MovePosition(m_UpdatePosition);
        }

        private Vector3 GetGravityPosition(Vector3 position, Vector3 direction)
        {
            return position + (direction * m_Gravity * Time.fixedDeltaTime);
        }

        private bool CheckForGround(Vector3 position)
        {
            // TransformDirection() accounts for rotations.
            return Physics.CheckSphere(position + transform.TransformDirection(m_GroundOffset), m_GroundRadius, ~m_CharacterLayer.value);
        }

        private Vector3 GetGroundCheckPosition(Vector3 position)
        {
            Vector3 sphereCastOffset = position + transform.up;
            RaycastHit hit;
            if (Physics.SphereCast(sphereCastOffset, m_GroundRadius, -transform.up, out hit, m_GroundDistance, ~m_CharacterLayer.value))
            {
                position.y = hit.point.y;

                CapsuleCollider selfCollider = GetComponentInChildren<CapsuleCollider>();
                Collider hitCollider = hit.collider;

                Vector3 normal = Vector3.zero;
                float distance = 0.0f;
                bool overlapped = Physics.ComputePenetration(
                    selfCollider, selfCollider.transform.position, selfCollider.transform.rotation,
                    hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
                    out normal, out distance
                );

                // TODO: Figure out how to use a combination of ComputePenetration(), ClosestPoint(), and hit.point to adjust positions.
                // Vector3 closestPoint = transform.InverseTransformPoint(selfCollider.ClosestPoint(hit.point));
                // Debug.Log(closestPoint);
                // position += transform.InverseTransformPoint(hit.point) - closestPoint;
                // position += hit.point - selfCollider.ClosestPoint(hit.point);
                if (overlapped)
                {
                    // Vector3 posCorrection = normal * distance;
                    // transform.position += posCorrection;
                }
                else
                {
                    // position += transform.InverseTransformPoint(hit.point) * hit.distance;
                    // Debug.Log(hit.distance);
                }
            }

            return position;
        }

        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            // Sphere for ground check.
            Gizmos.color = Color.green;
            Vector3 groundedSphereStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) + transform.TransformDirection(m_GroundOffset);
            Gizmos.DrawWireSphere(groundedSphereStart, m_GroundRadius);

            // SphereCast for specific ground check.
            Vector3 groundedRayStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) + transform.up;
            RaycastHit hit;
            bool isHit = Physics.SphereCast(groundedRayStart, m_GroundRadius, -transform.up, out hit, m_GroundDistance, ~m_CharacterLayer.value);
            if (isHit)
            {
                // Draw SphereCast sphere.
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundedRayStart + -transform.up * hit.distance, m_GroundRadius);

                // Draw hit point normal.
                Gizmos.color = Color.cyan;
                // Gizmos.DrawRay(hit.point, hit.normal * 3.0f);

                // // TODO: Testing Physics.ComputePenetration().
                // Collider self = GetComponentInChildren<Collider>();

                // Collider[] cols = Physics.OverlapSphere(groundedSphereStart, m_GroundRadius, ~m_CharacterLayer.value);
                // for (int i = 0; i < cols.Length; i++)
                // {
                //     Collider col = cols[i];
                //     Vector3 normal = Vector3.zero;
                //     float distance = 0.0f;

                //     bool overlapped = Physics.ComputePenetration(
                //         self, self.transform.position, self.transform.rotation,
                //         col, col.transform.position, col.transform.rotation,
                //         out normal, out distance
                //     );

                //     if (overlapped)
                //     {
                //         Vector3 posCorrection = normal * distance;
                //         transform.position += posCorrection;
                //     }
                // }
            }

            Gizmos.color = color;
        }
    }
}
