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
        [Header("Behaviours")]
        [SerializeField]
        private List<string> m_CharacterBehaviourNames = new List<string>();

        [Header("Character Settings")]
        [SerializeField]
        private LayerMask m_CharacterLayer;
        [SerializeField]
        private ECCBoolReference m_UseGravity = true;

        // TODO: Move ground checking to a custom behaviour?
        [Header("Ground Settings")]
        [SerializeField]
        private ECCFloatReference m_GroundRadius = 1.0f;
        [SerializeField]
        private ECCVector3Reference m_GroundOffset = Vector3.zero;

        [SerializeField]
        private ECCFloatReference m_GroundDistance = 1.0f;

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

        private void Update()
        {
            // GetGroundCheckPosition(m_Rigidbody.position);
        }

        private void FixedUpdate()
        {
            Vector3 updatePosition = m_Rigidbody.position;
            updatePosition = m_UseGravity ? GetGravityPosition(updatePosition, -transform.up) : updatePosition;

            updatePosition = GetGroundCheckPosition(updatePosition);

            m_UpdatePosition = updatePosition;
            m_Rigidbody.MovePosition(m_UpdatePosition);
        }

        private Vector3 GetGravityPosition(Vector3 position, Vector3 direction)
        {
            float gravity = -Physics.gravity.y;
            return position + (direction * gravity * Time.fixedDeltaTime);
        }

        private Vector3 GetGroundCheckPosition(Vector3 position)
        {
            Vector3 sphereCastOffset = position + transform.up; // Offseting the Y index by a value of 1 should be fine in most cases.
            RaycastHit hit;
            if (Physics.SphereCast(sphereCastOffset, m_GroundRadius, -transform.up, out hit, m_GroundDistance, ~m_CharacterLayer.value))
            {
                position.y = hit.point.y;
            }

            return position;
        }

        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            // Sphere for ground check.
            Gizmos.color = Color.blue;
            Vector3 groundedSphereStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) + m_GroundOffset;
            Gizmos.DrawWireSphere(groundedSphereStart, m_GroundRadius);

            // SphereCast for specific ground check.
            Vector3 groundedRayStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) + transform.up;
            RaycastHit hit;
            bool isHit = Physics.SphereCast(groundedRayStart, m_GroundRadius, -transform.up, out hit, m_GroundDistance, ~m_CharacterLayer.value);
            if (isHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundedRayStart + -transform.up * hit.distance, m_GroundRadius);

                Gizmos.color = Color.green;
                // Gizmos.DrawLine(hit.point, hit.transform.forward * 2.5f);
                Gizmos.DrawLine(hit.normal, hit.normal * 2);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundedRayStart + -transform.up * m_GroundDistance, m_GroundRadius);
            }

            Gizmos.color = color;
        }
    }
}
