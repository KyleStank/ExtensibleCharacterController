using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

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
            updatePosition = GetGroundCheckPosition(updatePosition);
            updatePosition = m_UseGravity ? GetGravityPosition(updatePosition, -Vector3.up) : updatePosition;


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
                // position = hit.point;
                position.y = hit.point.y;

                CapsuleCollider selfCollider = GetComponentInChildren<CapsuleCollider>();
                Collider hitCollider = hit.collider;

                Vector3 closestPoint = selfCollider.ClosestPoint(hit.point);
                Vector3 offsetPos = transform.position + (closestPoint - transform.position);
                Vector3 offsetHitPos = hit.point - offsetPos;
                // position += offsetHitPos;

                Vector3 direction = Vector3.zero;
                float distance = 0.0f;
                bool overlapped = Physics.ComputePenetration(
                    selfCollider, selfCollider.transform.position, selfCollider.transform.rotation,
                    hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
                    out direction, out distance
                );

                // // TODO: Figure out how to use a combination of ComputePenetration(), ClosestPoint(), and hit.point to adjust positions.
                // Vector3 closestPoint = transform.InverseTransformPoint(selfCollider.ClosestPoint(hit.point));
                // Debug.Log(closestPoint);
                // position += transform.InverseTransformPoint(hit.point) - closestPoint;
                // position += hit.point - selfCollider.ClosestPoint(hit.point);
                if (overlapped)
                {
                    // TODO: Calculate "bottom" position of character based on CapsuleCollider extends/bounds/whatever. Useful for using either Center or Pivot positions.
                    // TODO: Switch to capsule cast. Create a cast that is exactly the same dimensions as the actual CapsuleCollider component.
                    // TODO: Add "spacing" between final calculated grounding position to prevent collider from overlapping ground collider.
                    // TODO: Convert ComputePenetration() world pos to local pos and simply add that to the move direction.
                    // TODO: Create MoveDirection Vector3
                    // TODO: Use SphereCast() for collision, then Raycast for detecting actual normal
                    // TODO: Research Vector3.ProjectOnPlane() for BOTH Vertical and Horizontal movement. Seems to only be Vector3.up or -gravityDir
                    // TODO: Look at UCC for Vector3.Cross() uses. Seems to only be used for Horizontal movement, and its also used w/ Vector3.Dot()

                    // Vector3 posCorrection = direction.normalized * distance;
                    // position += posCorrection;
                    // // Debug.DrawRay(position, direction * distance, Color.magenta);
                    // Debug.Log("Collide");
                }
                else
                {
                    // position += transform.InverseTransformPoint(hit.point) * hit.distance;
                    // Debug.Log(hit.distance);
                }
            }

            return position;
        }

        [SerializeField]
        float h = 0.0f;
        [SerializeField]
        float v = 0.0f;
        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            Gizmos.color = Color.yellow;
            CapsuleCollider selfCollider = GetComponentInChildren<CapsuleCollider>();
            Vector3 center, capStart, capEnd;
            CapsuleColliderEndCaps(selfCollider, out center, out capStart, out capEnd);

            RaycastHit[] _hits = Physics.CapsuleCastAll(capStart, capEnd, selfCollider.radius, -transform.up.normalized, transform.up.magnitude, ~m_CharacterLayer.value);
            Gizmos.color = Color.cyan;
            DrawWireCapsule(center - transform.up, transform.rotation, selfCollider.radius, 2, Color.yellow);

            for (int i = 0; i < _hits.Length; i++)
            {
                RaycastHit _hit = _hits[i];
                Debug.Log(_hit.collider.name);
                Debug.Log(_hit.distance);

                // Draw closest point.
                Collider hitCollider = _hit.collider;
                Vector3 closestPoint = selfCollider.ClosestPoint(_hit.point);
                Gizmos.DrawSphere(_hit.point, 0.1f);
                Gizmos.DrawWireSphere(closestPoint, 0.1f);

                // RaycastHit normalHit;
                // if (Physics.Raycast(groundedRayStart, -transform.up, out normalHit, m_GroundDistance, ~m_CharacterLayer.value))
                // {
                //     Gizmos.DrawRay(normalHit.point, normalHit.normal * 3.0f);
                // }
            }

            // Horizontal movement. ALWAYS a perpenticular direction. That is epic.
            Vector3 dir = Vector3.ProjectOnPlane(new Vector3(h, 0.0f, v), transform.up);
            Gizmos.DrawRay(transform.position, dir * 10.0f);
            Gizmos.color = Color.blue;

            // Sphere for ground check.
            Gizmos.color = Color.green;
            Vector3 groundedSphereStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) + transform.TransformDirection(m_GroundOffset);
            // Gizmos.DrawWireSphere(groundedSphereStart, m_GroundRadius);

            // SphereCast for specific ground check.
            Vector3 groundedRayStart = (m_Rigidbody ? m_Rigidbody.position : transform.position) +
                transform.up + transform.TransformDirection(m_GroundOffset);
            Gizmos.color = Color.blue;
            // Gizmos.DrawWireSphere(groundedRayStart, m_GroundRadius);

            RaycastHit hit;
            bool isHit = Physics.SphereCast(groundedRayStart, m_GroundRadius, -transform.up, out hit, m_GroundDistance, ~m_CharacterLayer.value);
            if (isHit)
            {
                // Draw SphereCast sphere.
                Gizmos.color = Color.red;
                // Gizmos.DrawWireSphere(groundedRayStart + -transform.up * hit.distance, m_GroundRadius);

                // Draw hit point normal.
                Gizmos.color = Color.cyan;
                // RaycastHit normalHit;
                // if (Physics.Raycast(groundedRayStart, -transform.up, out normalHit, m_GroundDistance, ~m_CharacterLayer.value))
                // {
                //     Gizmos.DrawRay(normalHit.point, normalHit.normal * 3.0f);
                // }

                // // Draw closest point.
                // Collider hitCollider = hit.collider;
                // Vector3 closestPoint = selfCollider.ClosestPoint(hit.point);
                // Gizmos.DrawSphere(hit.point, 0.1f);
                // Gizmos.DrawWireSphere(closestPoint, 0.1f);

                // Vector3 normal = Vector3.zero;
                // float distance = 0.0f;
                // bool overlapped = Physics.ComputePenetration(
                //     selfCollider, selfCollider.transform.position, selfCollider.transform.rotation,
                //     hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
                //     out normal, out distance
                // );

                // if (overlapped)
                // {
                //     // Vector3 posCorrection = normal * distance;
                //     // transform.position += posCorrection;
                // }
            }
            else
            {
                Gizmos.color = Color.green;
                // Gizmos.DrawWireSphere(groundedRayStart + -transform.up * m_GroundDistance, m_GroundRadius);
            }

            Gizmos.color = color;
        }

        private void CapsuleColliderEndCaps(CapsuleCollider collider, out Vector3 center, out Vector3 capStart, out Vector3 capEnd)
        {
            // TODO: Add calculations to Account for capsule direction, scale, and rotation.
            // float heightMultiplier = 1.0f; // Retrieve based on direction of Capsule
            // float radiusMultipler = 1.0f; // Retrieve based on direction of Capsule

            center = collider.transform.TransformPoint(collider.center);
            capStart = center - (collider.transform.up * (collider.height / 2.0f - collider.radius));
            capEnd = center + (collider.transform.up * (collider.height / 2.0f - collider.radius));
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

        // private static Vector3 TransformPoint(Vector3 worldPosition, Quaternion rotation, Vector3 localPosition)
        // {
        //     return worldPosition + (rotation * localPosition);
        // }

        // private static Vector3 InverseTransformPoint(Vector3 worldPosition, Quaternion rotation, Vector3 position)
        // {
        //     var diff = position - worldPosition;
        //     return Quaternion.Inverse(rotation) * diff;
        // }

        // private static Vector3 TransformDirection(Vector3 direction, Quaternion rotation)
        // {
        //     return rotation * direction;
        // }

        // private static Vector3 InverseTransformDirection(Vector3 direction, Quaternion rotation)
        // {
        //     return Quaternion.Inverse(rotation) * direction;
        // }

        // private static Quaternion TransformQuaternion(Quaternion worldRotation, Quaternion rotation)
        // {
        //     return worldRotation * rotation;
        // }

        // private static Quaternion InverseTransformQuaternion(Quaternion worldRotation, Quaternion rotation)
        // {
        //     return Quaternion.Inverse(worldRotation) * rotation;
        // }
    }
}
